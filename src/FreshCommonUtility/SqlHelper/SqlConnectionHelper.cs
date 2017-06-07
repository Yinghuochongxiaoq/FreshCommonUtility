using System.Data;
using System.Data.SqlClient;
using FreshCommonUtility.Configure;
using FreshCommonUtility.CoreModel;
using FreshCommonUtility.Dapper;
using MySql.Data.MySqlClient;
using Npgsql;

namespace FreshCommonUtility.SqlHelper
{
    /// <summary>
    /// Sql connection helper class.Use this helper class must first use InitConnectionServer function.
    /// </summary>
    public static class SqlConnectionHelper
    {
        /// <summary>
        /// Get connection string.
        /// </summary>
        private static string ConnectionString { get; set; }

        /// <summary>
        /// Return connection object.
        /// </summary>
        private static MySqlConnection _conn;

        /// <summary>
        /// Lock resource
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// cut put point.
        /// </summary>
        static SqlConnectionHelper()
        {
            var appSettings = AppConfigurationHelper.GetAppSettings<AppSettingsModel>("AppSettings");
            ConnectionString = appSettings?.MySqlConnectionString;
        }

        /// <summary>
        /// Get single MySQL connection object
        /// </summary>
        /// <param name="connectionString">you need new connection object.</param>
        /// <returns></returns>
        public static MySqlConnection GetMySqlConnectionConnection(string connectionString = null)
        {
            if (!string.IsNullOrEmpty(connectionString) && connectionString.GetHashCode() != ConnectionString.GetHashCode())
            {
                _conn = new MySqlConnection(connectionString);
            }
            if (_conn == null)
            {
                lock (SyncRoot)
                {
                    if (_conn == null)
                    {
                        _conn = new MySqlConnection(ConnectionString);
                    }
                }
            }
            return _conn;
        }

        /// <summary>
        /// Get connection string.
        /// </summary>
        public static string GetConnectionString() => ConnectionString;

        /// <summary>
        /// set connection type
        /// </summary>
        /// <param name="dbtype"></param>
        public static void SetConnectionType(SimpleCRUD.Dialect dbtype)
        {
            _dbtype = dbtype;
        }

        /// <summary>
        /// connection type
        /// </summary>
        private static SimpleCRUD.Dialect _dbtype;

        /// <summary>
        /// Get open connection
        /// </summary>
        /// <param name="connectionString">DIV you connection string</param>
        /// <returns>IDbConection object</returns>
        public static IDbConnection GetOpenConnection(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;
            }
            IDbConnection connection;
            if (_dbtype == SimpleCRUD.Dialect.PostgreSQL)
            {
                connection = new NpgsqlConnection(ConnectionString);
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
            }
            else if (_dbtype == SimpleCRUD.Dialect.MySQL)
            {
                connection = new MySqlConnection(ConnectionString);
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
            }
            else
            {
                connection = new SqlConnection(ConnectionString);
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
            }
            connection.Open();
            return connection;
        }
    }
}
