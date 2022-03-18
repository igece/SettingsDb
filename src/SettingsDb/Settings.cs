using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

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


        public Settings(string databaseName)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            _connectionString = $"Data Source={Path.GetFileNameWithoutExtension(databaseName)}.db";
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
            if (settingName == null)
                throw new ArgumentNullException(nameof(settingName));

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


        public async Task StoreAsync<T>(string settingName, T value)
        {
            await Task.Run(() => Store(settingName, value));
        }


        public T Read<T>(string settingName, T defaultValue = default)
        {
            if (settingName == null)
                throw new ArgumentNullException(nameof(settingName));

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


        public async Task<T> ReadAsync<T>(string settingName, T defaultValue = default)
        {
            return await Task.Run(() => Read(settingName, defaultValue));
        }


        public void Clear(string settingName)
        {
            if (settingName == null)
                throw new ArgumentNullException(nameof(settingName));

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


        public async Task ClearAsync(string settingName)
        {
            await Task.Run(() => Clear(settingName));
        }


        public void ClearAll()
        {
            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand("DELETE FROM Settings", dbConnection))
                    sqlCmd.ExecuteNonQuery();

                dbConnection.Close();
            }
        }


        public async Task ClearAllAsync()
        {
            await Task.Run(() => ClearAll());
        }
    }
}
