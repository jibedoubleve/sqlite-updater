using System.Reflection;

namespace System.SQLite.Updater.Test.Helpers;

public static class ResourceHelper
{
    #region Fields

    private static readonly Assembly RessourceAsm = Assembly.GetAssembly(typeof(ResourceHelper))!;

    #endregion

    #region Methods

    public static string GetResource(Version version)
    {
        var       resourceName = GetResources(version);
        using var stream       = RessourceAsm.GetManifestResourceStream(resourceName);
        if (stream == null) throw new ArgumentNullException(nameof(resourceName), $"'{nameof(resourceName)}' is null. Are you sure the resource '{resourceName}' exists in the assembly '{RessourceAsm.FullName}'");
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string GetResources(Version version)
    {
        var resourceName = $"System.SQLite.Updater.Test.Scripts.script-{version.ToString(3)}.sql";
        var result       = from s in RessourceAsm.GetManifestResourceNames() where s == resourceName select s;
        return result?.FirstOrDefault() ?? throw new InvalidDataException($"No resource found for '{resourceName}'");
    }

    #endregion
}