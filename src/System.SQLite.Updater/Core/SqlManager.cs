using System.Data;
using Dapper;

namespace System.SQLite.Updater.Core;

internal static class SqlManager
{
    #region Methods

    public static void ExecuteMany(this IDbConnection db, IEnumerable<string> sqlScripts)
    {
        foreach (var script in sqlScripts) db.Execute(script);
    }

    #endregion
}