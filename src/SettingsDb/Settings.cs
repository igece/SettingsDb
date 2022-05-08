using System.IO;
using System.Reflection;

using Microsoft.Data.Sqlite;


namespace SettingsDb
{
    public class Settings : SettingsDb<SqliteConnection>
    {
        public Settings()
            : base($"Data Source={Assembly.GetEntryAssembly().GetName().Name}.Settings.db")
        {
        }


        public Settings(string databaseName, string settingsTable = DefaultSettingsTable)
            : base($"Data Source={Path.GetFileNameWithoutExtension(databaseName)}.db", settingsTable)
        {
        }
    }
}
