using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;


namespace SettingsDb
{
    public class SettingsDb<TConnection> : ISettingsDb where TConnection : DbConnection, new()
    {
        public const string DefaultSettingsTable = "Settings";


        private readonly string _connectionString;

        private readonly string _settingsTable;


        public SettingsDb(string connectionString, string settingsTable = DefaultSettingsTable)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
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
            using (var dbConnection = new TConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();

                var command = dbConnection.CreateCommand();

                using (var sqlCmd = dbConnection.CreateCommand())
                {
                    sqlCmd.CommandText = $"CREATE TABLE IF NOT EXISTS \"{_settingsTable}\" (Id INTEGER, Name TEXT UNIQUE NOT NULL, Value TEXT, PRIMARY KEY(Id))";
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
        private bool IsSettingsTableOk(TConnection dbConnection)
        {
            using (var sqlCmd = dbConnection.CreateCommand())
            {
                sqlCmd.CommandText = "SELECT COUNT() FROM PRAGMA_TABLE_INFO(@SettingsTable)";
                sqlCmd.AddParameter("SettingsTable", _settingsTable);

                //sqlCmd.Parameters.Add(CreateSqlParameter(sqlCmd, "SettingsTable", _settingsTable));

                if ((long)sqlCmd.ExecuteScalar() != 3)
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


        /*
        private DbParameter CreateSqlParameter(DbCommand sqlCommand, string name, string value)
        {
            var sqlParam = sqlCommand.CreateParameter();
            sqlParam.ParameterName = name;
            sqlParam.Value = value;
            
            return sqlParam;
        }
        */


        public void Store<T>(string settingName, T value)
        {
            if (settingName == null)
                throw new ArgumentNullException(nameof(settingName));

            using (var dbConnection = new TConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();

                using (var sqlCmd = dbConnection.CreateCommand())
                {
                    sqlCmd.CommandText = $"INSERT INTO \"{_settingsTable}\" (Name, Value) VALUES (@Name, @Value) ON CONFLICT(Name) DO UPDATE SET Value = @Value";
                    sqlCmd.AddParameter("Name", settingName);
                    sqlCmd.AddParameter("Value", JsonSerializer.Serialize(value));
                    //sqlCmd.Parameters.Add(CreateSqlParameter(sqlCmd, "Name", settingName));
                    //sqlCmd.Parameters.Add(CreateSqlParameter(sqlCmd, "Value", JsonSerializer.Serialize(value)));

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

            using (var dbConnection = new TConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();

                using (var sqlCmd = dbConnection.CreateCommand())
                {
                    sqlCmd.CommandText = $"SELECT Value FROM \"{_settingsTable}\" WHERE Name = @Name LIMIT 1";
                    sqlCmd.AddParameter("Name", settingName);
                    //sqlCmd.Parameters.Add(CreateSqlParameter(sqlCmd, "Name", settingName));

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

            using (var dbConnection = new TConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();

                using (var sqlCmd = dbConnection.CreateCommand())
                {
                    sqlCmd.CommandText = $"DELETE FROM \"{_settingsTable}\" WHERE Name = @Name LIMIT 1";
                    sqlCmd.AddParameter("Name", settingName);
                    //sqlCmd.Parameters.Add(CreateSqlParameter(sqlCmd, "Name", settingName));
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
            using (var dbConnection = new TConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();

                using (var sqlCmd = dbConnection.CreateCommand())
                {
                    sqlCmd.CommandText = $"DELETE FROM \"{_settingsTable}\"";
                    sqlCmd.ExecuteNonQuery();
                }

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

            using (var dbConnection = new TConnection())
            {
                dbConnection.ConnectionString = _connectionString;
                dbConnection.Open();

                using (var sqlCmd = dbConnection.CreateCommand())
                {
                    sqlCmd.CommandText = $"SELECT COUNT() FROM \"{_settingsTable}\"";
                    count = (long)sqlCmd.ExecuteScalar();
                }

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
