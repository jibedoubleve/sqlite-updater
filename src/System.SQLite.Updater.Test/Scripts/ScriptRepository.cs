using System.Reflection;

namespace System.SQLite.Updater.Test.TestAssets;

/// <summary>
///     The sole purpose of this class is to return the
///     Assembly where it stands. This is used in the
///     <see cref="SQLiteDatabaseUpdateManager" /> to find
///     and load the SQL scripts
/// </summary>
public static class ScriptRepository
{
    #region Properties

    public static Assembly Asm => typeof(ScriptRepository).Assembly;

    #endregion

    public const string DbScriptEmbededResourcePattern = @"System\.SQLite\.Updater\.Test\.Scripts\.script-(\d{1,3}\.{0,1}\d{1,3}\.{0,1}\d{0,3}).*.sql";
}