using System.SQLite.Updater.Core;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace System.SQLite.Updater.Test;

public class DatabaseVersionManagerShould : BaseTest
{
    #region Constructors
    
    public DatabaseVersionManagerShould(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    #endregion
    
    #region Methods
    
    [Theory]
    [InlineData("1.0")]
    [InlineData("1.1")]
    [InlineData("2.0")]
    [InlineData("2.1")]
    [InlineData("2.1.1")]
    public void ReturnVersionOfDatabase(string version)
    {
        using var db = BuildConnection();
        CreateVersion(db, version);

        var expected = new Version(version);

        db.GetCurrentDbVersion().Should().Be(expected);
    }
    
    [Theory]
    [InlineData("1.2")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4")]
    public void SetDatabaseVersion(string ver)
    {
        var version = new Version(ver);

        using var db = BuildConnection();

        db.SetCurrentDbVersion(version);
        db.GetCurrentDbVersion().Should().Be(version);
    }
    
    #endregion
}