///////////////////////////////////////////////////////////////////////////////
/// TOOLS & ADDINS
///////////////////////////////////////////////////////////////////////////////
#tool nuget:?package=GitVersion.CommandLine&version=5.10.1
#tool nuget:?package=GitReleaseManager&version=0.13.0

#addin nuget:?package=Cake.Git&version=4.0.0
#addin nuget:?package=Cake.FileHelpers&version=7.0.0

///////////////////////////////////////////////////////////////////////////////
/// USINGS & NAMESPACES
///////////////////////////////////////////////////////////////////////////////
#r "Spectre.Console"
using Spectre.Console

///////////////////////////////////////////////////////////////////////////////
/// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var repoName = "osteostudio";

// Paths
var solution = "./sqlite-updater.sln";
var project  = "./src/System.SQLite.Updater/System.SQLite.Updater.csproj";

// Arguments
var target   = Argument("target", "Default");

GitVersion gitVersion;

///////////////////////////////////////////////////////////////////////////////
/// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx => {
    StartProcess("git", "stash");
    // https://gitversion.net/docs/usage/cli/arguments
    // https://cakebuild.net/api/Cake.Core.Tooling/ToolSettings/50AAB3A8
    gitVersion = GitVersion(new GitVersionSettings 
    { 
        OutputType            = GitVersionOutput.Json,
        Verbosity             = GitVersionVerbosity.Verbose,        
        ArgumentCustomization = args => args.Append("/updateprojectfiles")
    });
    var branchName = gitVersion.BranchName;
    var workingDirectory = System.IO.Directory.GetCurrentDirectory();

    AnsiConsole.Write(
        new FigletText($"{repoName}")
            .LeftJustified()
            .Color(Color.Red));

    Information("Working directory         : {0}", workingDirectory);  
    Information("Branch                    : {0}", branchName);
    Information("Informational      Version: {0}", gitVersion.InformationalVersion);
    Information("SemVer             Version: {0}", gitVersion.SemVer);
    Information("AssemblySemVer     Version: {0}", gitVersion.AssemblySemVer);
    Information("AssemblySemFileVer Version: {0}", gitVersion.AssemblySemFileVer);
    Information("MajorMinorPatch    Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet              Version: {0}", gitVersion.NuGetVersion);
});

///////////////////////////////////////////////////////////////////////////////
/// TASKS
///////////////////////////////////////////////////////////////////////////////
Task("info").Does(()=> { 
        /*Does nothing but specifying information of the build*/ 
});

Task("clean").Does(()=> {
    var dirToDelete = GetDirectories("./**/obj")
                        .Concat(GetDirectories("./**/bin"))
                        .Concat(GetDirectories("./**/Output"))
                        .Concat(GetDirectories("./**/Publish"));
    
    Information("Deleting directories (bin, Output, Publish)...");
    DeleteDirectories(dirToDelete, new DeleteDirectorySettings{ Recursive = true, Force = true});

    Information("Dotnet clean the solution...");
    DotNetTool(solution, "clean" );
});

Task("restore").Does(() => DotNetTool(solution, "restore"));

Task("build").Does(() => {   
    DotNetTool(
        solution,
        "build",
        "--no-restore -c release"
    );
});

Task("test").Does(() => {
    DotNetTool(
        solution,
        "test",
        "--no-restore --no-build -c release"
    );
});

Task("post-clean").Does(() => {
    GitReset(".", GitResetMode.Hard);
    StartProcess("git", "stash pop");
});

Task("nuget-pack").Does(() => {
    DotNetTool(
    solution,
    "pack",
    $"{project} --output Publish -c release");
});

Task("nuget-push").Does(() => {
        var version = gitVersion.NuGetVersion;
        var apiKey  = EnvironmentVariable("NUGET_TOKEN");
        var source  = "https://api.nuget.org/v3/index.json";
        
        var parameters = $"push \"./Publish/SQLiteUpdater.{version}.nupkg\" --api-key {apiKey} --source {source}";

        DotNetTool(
            solution,
            "nuget",
            parameters 
        );
});

///////////////////////////////////////////////////////////////////////////////
/// DEPENDENCIES
///////////////////////////////////////////////////////////////////////////////

Task("default")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .IsDependentOn("build")
    .IsDependentOn("test")
    .IsDependentOn("nuget-pack");

Task("local")
    .IsDependentOn("default")
    .IsDependentOn("post-clean");

Task("ci")
    .IsDependentOn("default")
    .IsDependentOn("nuget-push")
    .IsDependentOn("post-clean");

RunTarget(target);
