using System;
using FreshCommonUtility.Configure;
using FreshCommonUtility.CoreModel;
using StackExchange.Redis;

namespace FreshCommonUtility.Cache
{
    /// <summary>
    /// FreshMan redis connection helper.
    /// </summary>
    public class RedisConnectionHelper
    {
        /// <summary>
        /// Return connection object.
        /// </summary>
        private static ConnectionMultiplexer _conn;

        /// <summary>
        /// Lock resource
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// cut put point.
        /// </summary>
        private RedisConnectionHelper() { }

        /// <summary>
        /// Get single connection object
        /// </summary>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnection()
        {
            if (_conn == null)
            {
                lock (SyncRoot)
                {
                    if (_conn == null)
                    {
                        var appSettings = AppConfigurationHelper.GetAppSettings<AppSettingsModel>("AppSettings");
                        var connectionString = appSettings?.RedisCaching?.ConnectionString;
                        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Redis connection config is Empty,please check you config.");
                        _conn = ConnectionMultiplexer.Connect(connectionString);
                    }
                }
            }
            return _conn;
        }
    }
}
