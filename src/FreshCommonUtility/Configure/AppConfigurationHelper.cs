using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FreshCommonUtility.Configure
{
    /// <summary>
    /// Get app config string helper.
    /// </summary>
    public class AppConfigurationHelper
    {
        /// <summary>
        /// Get key of appsettings.json file.Which could be include in bin/debug catalog
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="filePath">config key-value file name(default is appsettings.json)</param>
        /// <returns></returns>
        public static T GetAppSettings<T>(string key, string filePath = "appsettings.json") where T : class, new()
        {
            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = filePath, ReloadOnChange = true })
                .Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }

        /// <summary>
        /// Get app settings config value
        /// </summary>
        /// <param name="key">config key</param>
        /// <param name="filePath">you need get file path,which must have this file.</param>
        /// <returns></returns>
        public static string GetAppSettings(string key, string filePath = "appsettings.json")
        {
            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = filePath, ReloadOnChange = true })
                .Build();
            var appconfig = config.GetSection(key);
            return appconfig.Value;
        }
    }
}
