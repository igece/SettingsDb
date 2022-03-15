using System.Reflection;
using System.Text.Json;

using Microsoft.Data.Sqlite;


namespace SettingsDb
{
    public class Settings
    {
        private readonly string _connectionString;


        public Settings()
        {
            _connectionString = $"Data Source={Assembly.GetEntryAssembly().GetName().Name}.db";
            InitDatabase();
        }


        public Settings(string name)
        {
            _connectionString = $"Data Source={name}.db";
            InitDatabase();
        }


        private void InitDatabase()
        {
            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS Settings (Id INTEGER, Name TEXT UNIQUE NOT NULL, Value TEXT, PRIMARY KEY(Id));", dbConnection))
                {
                    sqlCmd.ExecuteNonQuery();
                }

                dbConnection.Close();
            }
        }


        public void Store<T>(string settingName, T value)
        {
            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand("INSERT INTO Settings (Name, Value) VALUES (@Name, @Value) ON CONFLICT(Name) DO UPDATE SET Value = @Value", dbConnection))
                {
                    sqlCmd.Parameters.Add(new SqliteParameter("Name", settingName));
                    sqlCmd.Parameters.Add(new SqliteParameter("Value", JsonSerializer.Serialize(value)));
                    sqlCmd.ExecuteNonQuery();
                }

                dbConnection.Close();
            }
        }


        public T Read<T>(string settingName, T defaultValue = default)
        {
            T value = defaultValue;

            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand("SELECT Value FROM Settings WHERE Name = @Name LIMIT 1", dbConnection))
                {
                    sqlCmd.Parameters.Add(new SqliteParameter("Name", settingName));

                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        if (reader.Read())
                            value = JsonSerializer.Deserialize<T>(reader.GetString(0));
                    }
                }

                dbConnection.Close();
            }

            return value;
        }


        public void Delete(string settingName)
        {
            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand("DELETE FROM Settings WHERE Name = @Name LIMIT 1", dbConnection))
                {
                    sqlCmd.Parameters.Add(new SqliteParameter("Name", settingName));
                    sqlCmd.ExecuteNonQuery();
                }

                dbConnection.Close();
            }
        }


        public void Clear()
        {
            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand("DELETE FROM Settings", dbConnection))
                    sqlCmd.ExecuteNonQuery();

                dbConnection.Close();
            }
        }
    }
}
