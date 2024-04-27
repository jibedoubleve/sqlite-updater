# sqlite-updater

This library provides a mechanism to keep SQLite databases used by your desktop application up to date. It is designed
to manage your application's versions and apply the necessary SQL scripts to update the database accordingly.

## Features

* **Application Version Management**: The library tracks the current version of your application. Each version is
  associated with a specific set of SQL scripts that need to be executed to update the database to that version.
* **Selective Application of SQL Scripts**: Based on the current version of the application compared to what is recorded
  in the database, only the necessary SQL scripts to update the database will be executed.
* **Automated Updates**: Once configured, the library can be integrated into the startup cycle of your desktop
  application to ensure that the database is always up to date with the latest structural changes.

## Usage

1. Create an Empty .csproj Project
    * Start by creating a new C# project in your preferred IDE (e.g., Visual Studio).
    * Choose a suitable project template (e.g., Console Application).
4. Add SQL Scripts as Embedded Resources
    * Include your SQL scripts within the project and mark them as embedded resources. This allows them to be accessed
      within the assembly.
   ```xml
    <ItemGroup>
        <EmbeddedResource Include="Scripts\script-0.1.0.sql"/>
        <EmbeddedResource Include="Scripts\script-0.2.0.sql" />
    </ItemGroup>
   ```
   Ensure that the paths and patterns match the location of your SQL scripts within the project directory.
4. Initialize DatabaseUpdater
    * In your C# code, initialize the `DatabaseUpdater` by providing:
        * The assembly containing the embedded SQL resources.
        * A regular expression (regex) pattern to identify and extract version information from the SQL script
          filenames.
    ```csharp
    // Load the assembly containing the embedded SQL scripts
    var assembly = Assembly.Load("MyNamespace.Scripts");
    
    // Regular expression pattern to extract version from SQL script filenames
    var versionPattern 
        = new Regex(@"MyNamespace\.Scipts\.Sql\.script-(\d{1,3}\.{0,1}\d{1,3}\.{0,1}\d{0,3}).*.sql");
    
    // Initialize DatabaseUpdater
    var updater = new DatabaseUpdater(assembly, versionPattern);
    
    // Update the database 
    // Depending on the database version, the updater will choose what
    // scripts to apply 
    updater.UpdateDatabase();
    ```

## Additional information

To work the library needs the table `settings` if it is not in your SQLite, **it'll create it automatically**.

Here's the DDL for this table.

```sql 
create table settings (
  id      integer primary key,
  s_key   text,
  s_value text
);
```
## Licences

📚 Icon used in this library is from [Noun Project](https://thenounproject.com/icon/database-846785/).
