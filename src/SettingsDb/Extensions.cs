using System.Data.Common;


namespace SettingsDb
{
    internal static class Extensions
    {
        public static DbCommand AddParameter(this DbCommand sqlCommand, string name, string value)
        {
            var sqlParam = sqlCommand.CreateParameter();
            sqlParam.ParameterName = name;
            sqlParam.Value = value;

            sqlCommand.Parameters.Add(sqlParam);

            return sqlCommand;
        }
    }
}
