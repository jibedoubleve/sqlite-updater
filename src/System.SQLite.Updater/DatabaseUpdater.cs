using System.Data;
using System.Reflection;
using System.SQLite.Updater.Core;

namespace System.SQLite.Updater;

public class DatabaseUpdater
{
    #region Fields

    private readonly IDbConnection _db;

    private readonly ScriptManager _scriptManager;

    #endregion

    #region Constructors

    public DatabaseUpdater(IDbConnection db, Assembly assembly, string pattern)
    {
        ArgumentException.ThrowIfNullOrEmpty(pattern);
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(db);

        _db            = db;
        _scriptManager = new ScriptManager(assembly, pattern);
    }

    #endregion

    #region Methods

    public Version MaxVersion() { return _scriptManager.GetScripts().MaxVersion(); }

    public void UpdateDatabase()
    {
        var currentVersion = _db.GetCurrentDbVersion();
        UpdateFrom(currentVersion);
    }

    public void UpdateFromScratch()
    {
        var allScripts = new List<string>();
        var scripts    = _scriptManager.GetScripts();

        allScripts.Add(SqlQueries.CreateSettingsTable);
        allScripts.AddRange(scripts);

        _db.ExecuteMany(allScripts);
    }

    private void UpdateFrom(Version version)
    {
        var scripts = _scriptManager.GetScripts();
        _db.ExecuteMany(scripts.After(version));
    }

    #endregion
}