using System.Data;
using System.SQLite.Updater.Test.TestAssets;
using Dapper;
using Microsoft.Data.Sqlite;
using Xunit.Abstractions;

namespace System.SQLite.Updater.Test;

public class BaseTest
{
    #region Fields
    
    private const string ConnectionString = "Data Source =:memory: ";
    
    #endregion
    
    #region Constructors
    
    protected BaseTest(ITestOutputHelper outputHelper) => OutputHelper = outputHelper;
    
    #endregion
    
    #region Properties
    
    private ITestOutputHelper OutputHelper { get; }
    
    #endregion
    
    #region Methods
    
    protected IDbConnection BuildConnection()
    {
        var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        return conn;
    }
    
    protected IDbConnection BuildFreshDb(string sql)
    {
        var db      = BuildConnection();
        var updater = new DatabaseUpdater(db, ScriptRepository.Asm, ScriptRepository.DbScriptEmbededResourcePattern);
        updater.UpdateFromScratch();
        
        OutputHelper.WriteLine($"Creating fresh DB.{Environment.NewLine}{sql}");
        db.Execute(sql);
        return db;
    }
    
    protected void CreateVersion(IDbConnection db, string version)
    {
        db.SetSetting("db_version", version);
        OutputHelper.WriteLine($"Inserting version '{version}'");
    }
    
    #endregion
}