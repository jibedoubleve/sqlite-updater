using System.Reflection;
using System.SQLite.Updater.Test.Helpers;
using System.SQLite.Updater.Test.TestAssets;
using Dapper;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace System.SQLite.Updater.Test;

public class DatabaseUpdaterShould : BaseTest
{
    private const string sqlCount = "select count(*) from dummy_table;";
    
    #region Fields
    
    private static readonly Assembly Asm = Assembly.GetExecutingAssembly();
    
    #endregion
    
    #region Constructors
    
    public DatabaseUpdaterShould(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    #endregion
    
    #region Methods
    
    [Fact]
    public void UpdateDatabaseAutomaticallyFromVersionOne()
    {
        // ARRANGE
        using var db      = BuildConnection();
        var       updater = new DatabaseUpdater(db, Asm, ScriptRepository.DbScriptEmbededResourcePattern);

        //Set artificially DB to version 0.1.0
        var ver = new Version(0, 1, 0);
        var sql = ResourceHelper.GetResource(ver);
        db.Execute(sql);

        //Set database version (should be manual in this case)
        db.SetDatabaseVersion(new Version(0, 1, 0));

        // ACT
        updater.UpdateDatabase();

        // ASSERT
        sql.Should().NotBeNullOrEmpty();
        db.ExecuteScalar<int>(sqlCount).Should().Be(2);
    }
    
    [Fact]
    public void UpdateDatabaseFromScratch()
    {
        // ARRANGE
        using var db      = BuildConnection();
        var       updater = new DatabaseUpdater(db, Asm, ScriptRepository.DbScriptEmbededResourcePattern);

        // ACT
        updater.UpdateFromScratch();

        // ASSERT 
        db.ExecuteScalar<int>(sqlCount).Should().Be(2);
    }
    
    #endregion
}