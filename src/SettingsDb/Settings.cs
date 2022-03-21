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
        public const string DefaultSettingsTable = "Settings";


        private readonly string _connectionString;

        private readonly string _settingsTable;


        public Settings(string settingsTable = DefaultSettingsTable)
        {
            _connectionString = $"Data Source={Assembly.GetEntryAssembly().GetName().Name}.db";
            _settingsTable = settingsTable;

            InitDatabase();
        }


        public Settings(string databaseName, string settingsTable = DefaultSettingsTable)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            _connectionString = $"Data Source={Path.GetFileNameWithoutExtension(databaseName)}.db";
            _settingsTable = settingsTable;

            InitDatabase();
        }

        /// <summary>
        /// Create the settings table if it doesn't exist yet and
        /// check its structure to ensure is correct.
        /// </summary>
        /// <exception cref="SettingsDbException"></exception>
        private void InitDatabase()
        {
            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand($"CREATE TABLE IF NOT EXISTS \"{_settingsTable}\" (Id INTEGER, Name TEXT UNIQUE NOT NULL, Value TEXT, PRIMARY KEY(Id))", dbConnection))
                {
                    sqlCmd.ExecuteNonQuery();
                }

                if (!IsSettingsTableOk(dbConnection))
                {
                    dbConnection.Close();
                    throw new SettingsDbException($"{_settingsTable}: Invalid table schema");
                }

                dbConnection.Close();
            }
        }

        /// <summary>
        /// Checks the schema of the settings table, to make sure its is using the correct table. 
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns>True, if the table schema is the expected</returns>.
        private bool IsSettingsTableOk(SqliteConnection dbConnection)
        {
            using (var sqlCmd = new SqliteCommand("SELECT COUNT() FROM PRAGMA_TABLE_INFO(@SettingsTable)", dbConnection))
            {
                sqlCmd.Parameters.Add(new SqliteParameter("SettingsTable", _settingsTable));
                
                if  ((long)sqlCmd.ExecuteScalar() != 3)
                    return false;

                sqlCmd.CommandText = $"SELECT COUNT() FROM PRAGMA_TABLE_INFO(@SettingsTable) WHERE name='Id'";

                if ((long)sqlCmd.ExecuteScalar() != 1)
                    return false;

                sqlCmd.CommandText = $"SELECT COUNT() FROM PRAGMA_TABLE_INFO(@SettingsTable) WHERE name='Name'";

                if ((long)sqlCmd.ExecuteScalar() != 1)
                    return false;

                sqlCmd.CommandText = $"SELECT COUNT() FROM PRAGMA_TABLE_INFO(@SettingsTable) WHERE name='Value'";

                if ((long)sqlCmd.ExecuteScalar() != 1)
                    return false;
            }

            return true;
        }


        public void Store<T>(string settingName, T value)
        {
            if (settingName == null)
                throw new ArgumentNullException(nameof(settingName));

            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand($"INSERT INTO \"{_settingsTable}\" (Name, Value) VALUES (@Name, @Value) ON CONFLICT(Name) DO UPDATE SET Value = @Value", dbConnection))
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

                using (var sqlCmd = new SqliteCommand($"SELECT Value FROM \"{_settingsTable}\" WHERE Name = @Name LIMIT 1", dbConnection))
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

                using (var sqlCmd = new SqliteCommand($"DELETE FROM \"{_settingsTable}\" WHERE Name = @Name LIMIT 1", dbConnection))
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

                using (var sqlCmd = new SqliteCommand($"DELETE FROM \"{_settingsTable}\"", dbConnection))
                    sqlCmd.ExecuteNonQuery();

                dbConnection.Close();
            }
        }


        public async Task ClearAllAsync()
        {
            await Task.Run(() => ClearAll());
        }


        public long Count()
        {
            long count = 0;

            using (var dbConnection = new SqliteConnection(_connectionString))
            {
                dbConnection.Open();

                using (var sqlCmd = new SqliteCommand($"SELECT COUNT() FROM \"{_settingsTable}\"", dbConnection))
                    count = (long)sqlCmd.ExecuteScalar();

                dbConnection.Close();
            }

            return count;
        }


        public async Task<long> CountAsync()
        {
            return await Task.Run(() => Count());
        }
    }
}
