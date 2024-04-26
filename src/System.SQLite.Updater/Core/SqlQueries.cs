namespace System.SQLite.Updater.Core;

internal class SqlQueries
{
    public const string DbVersionCount = @"select count(*) from settings where lower(s_key) = 'db_version'";
    public const string UpdateDbVersion = "update settings set s_value = '{0}' where lower(s_key) = 'db_version'";

    public const string CreateSettingsTable = """
                                              create table settings (
                                                  id      integer primary key,
                                                  s_key   text,
                                                  s_value text
                                              );
                                              """;

    public const string CheckSettingsTable = """
                                             select count(*)
                                             from sqlite_schema
                                             where name = 'settings'
                                             """;
}