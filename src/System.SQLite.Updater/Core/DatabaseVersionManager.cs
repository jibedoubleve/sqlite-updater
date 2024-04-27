using System.Data;
using Dapper;

namespace System.SQLite.Updater.Core;

internal static class DatabaseVersionManager
{
    #region Methods

    public static Version GetCurrentDbVersion(this IDbConnection db)
    {
        db.TryCreateSettingsTable();
        var version = db.GetSetting("db_version");

        return string.IsNullOrEmpty(version) ? new Version() : new Version(version);
    }

    public static void SetCurrentDbVersion(this IDbConnection db, Version version)
    {
        db.TryCreateSettingsTable();
        db.SetDatabaseVersion(version);
    }

    internal static bool CheckTableExists(this IDbConnection db) { return db.ExecuteScalar<int>(SqlQueries.CheckSettingsTable) > 0; }

    internal static void TryCreateSettingsTable(this IDbConnection db)
    {
        var count = db.ExecuteScalar<int>(SqlQueries.CheckSettingsTable);
        if (count > 0) return;

        db.Execute(SqlQueries.CreateSettingsTable);
    }

    #endregion
}