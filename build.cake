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
var repoName = "sqlite-updater";

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

Teardown(context =>
{
    GitReset(".", GitResetMode.Hard);
    StartProcess("git", "stash pop");
});
///////////////////////////////////////////////////////////////////////////////
/// TASKS
///////////////////////////////////////////////////////////////////////////////
Task("info").Does(()=> { 
        /*Does nothing but specifying information of the build*/ 
});

Task("dn-clean").Does(()=> {
    var dirToDelete = GetDirectories("./**/obj")
                        .Concat(GetDirectories("./**/bin"))
                        .Concat(GetDirectories("./**/Output"))
                        .Concat(GetDirectories("./**/Publish"));
    
    Information("Deleting directories (bin, Output, Publish)...");
    DeleteDirectories(dirToDelete, new DeleteDirectorySettings{ Recursive = true, Force = true});

    Information("Dotnet clean the solution...");
    DotNetTool(solution, "clean" );
});

Task("dn-restore").Does(() => DotNetTool(solution, "restore"));
Task("dn-build").Does(() => DotNetTool(solution, "build", "--no-restore -c release"));
Task("dn-test").Does(() => DotNetTool(solution, "test", "--no-restore --no-build -c release"));

Task("nuget-pack").Does(() => DotNetTool(solution, "pack", $"{project} --output Publish -c release"));
Task("nuget-push").Does(() => {
        var version = gitVersion.NuGetVersion;
        var apiKey  = EnvironmentVariable("NUGET_TOKEN");
        var source  = "https://api.nuget.org/v3/index.json";
        
        var parameters = $"push \"./Publish/SQLiteUpdater.{version}.nupkg\" --api-key {apiKey} --source {source}";

        DotNetTool(solution, "nuget", parameters);
});

Task("github-release").Does(()=> {
    //https://stackoverflow.com/questions/42761777/hide-services-passwords-in-cake-build
    var token = EnvironmentVariable("CAKE_PUBLIC_GITHUB_TOKEN");
    var owner = EnvironmentVariable("CAKE_PUBLIC_GITHUB_USERNAME");

    var alphaVersions = new[] { "alpha", "beta" };
    var isPrerelease = alphaVersions.Any(x => gitVersion.SemVer.ToLower().Contains(x));

    if(isPrerelease) { Information("This is a prerelease"); }        
    Information("Has token      : {0}", !string.IsNullOrEmpty(token));
    Information("Has owner      : {0}", !string.IsNullOrEmpty(owner));

    var parameters = $"create --token {token} -o {owner} -r {repoName} " +
                     $"--milestone {gitVersion.MajorMinorPatch} --name {gitVersion.SemVer} " +
                     $"{(isPrerelease ? "--pre" : "")} " +
                     $"--targetDirectory {Environment.CurrentDirectory} " 
                     // + "--debug --verbose"
                     ;
    DotNetTool( solution, "gitreleasemanager", parameters);
});
///////////////////////////////////////////////////////////////////////////////
/// DEPENDENCIES
///////////////////////////////////////////////////////////////////////////////

Task("default")
    .IsDependentOn("dn-clean")
    .IsDependentOn("dn-restore")
    .IsDependentOn("dn-build")
    .IsDependentOn("dn-test")
    .IsDependentOn("nuget-pack");

Task("ci-dev")
    .IsDependentOn("default")
    .IsDependentOn("github-release");

Task("ci")
    .IsDependentOn("default")
    .IsDependentOn("github-release")
    .IsDependentOn("nuget-push");

RunTarget(target);
