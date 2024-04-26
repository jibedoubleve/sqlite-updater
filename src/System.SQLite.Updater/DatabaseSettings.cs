using System.Data;
using System.SQLite.Updater.Core;
using Dapper;

namespace System.SQLite.Updater;

public static class DatabaseSettings
{
    #region Methods

    public static string? GetSetting(this IDbConnection db, string key)
    {
        if (!db.CheckTableExists()) return null;

        const string sql = "select s_value from settings where s_key = @key";
        return db.Query<string>(sql, new { key }).FirstOrDefault();
    }

    public static void SetSetting(this IDbConnection db, string key, string value)
    {
        db.TryCreateSettingsTable();

        const string sqlDelete = "delete from settings where s_key = @key;";
        db.Execute(sqlDelete, new { key });

        const string sqlInsert = "insert into settings (s_key, s_value) values (@key, @value)";
        db.Execute(sqlInsert, new { key, value });
    }

    internal static void SetDatabaseVersion(this IDbConnection db, Version version) { db.SetSetting("db_version", version.ToString()); }

    #endregion
}