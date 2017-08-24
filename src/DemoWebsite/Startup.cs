using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DemoWebsite.Models;
using DemoWebsite.ViewModels;
using FreshCommonUtility.Configure;
using FreshCommonUtility.CoreModel;
using FreshCommonUtility.Dapper;
using FreshCommonUtility.SqlHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DemoWebsite
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
            //SetupDB();
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// <para>This method gets called by the runtime.</para>
        /// <para>Use this method to add services to the container.</para>
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
            //Add options
            services.AddOptions();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
            services.Configure<AppSettingsModel>(Configuration.GetSection("AppSettings"));
        }

        /// <summary>
        /// <para>This method gets called by the runtime.</para>
        /// <para>Use this method to configure the HTTP request pipeline.</para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// Init db
        /// </summary>
        private static void SetupDB()
        {
            /// <summary>
            /// Db username
            /// </summary>
            string _userName = "FreshMan";

            /// <summary>
            /// Db password
            /// </summary>
            string _password = "qinxianbo";

            /// <summary>
            /// Db host name
            /// </summary>
            string _hostName = "localhost";

            /// <summary>
            /// Db port
            /// </summary>
            int _port = 3306;

            /// <summary>
            /// Db name
            /// </summary>
            string _dbName = "sys";
            var mysqlConnectionString = $"Server={_hostName};Port={_port};User Id={_userName};Password={_password};Database={_dbName};SslMode=None";
            var dbrecreated = false;
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
            using (var connection = SqlConnectionHelper.GetOpenConnection(mysqlConnectionString))
            {
                try
                {
                    connection.Execute(@" DROP DATABASE SimplecrudDemoWebsite; ");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("database drop  failed - close and reopen VS and try again:" + ex.Message);
                }
                try
                {
                    connection.Execute(@" CREATE DATABASE SimplecrudDemoWebsite; ");
                    dbrecreated = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("database create failed - close and reopen VS and try again:" + ex.Message);
                }
            }
            if (!dbrecreated) return;
            mysqlConnectionString = AppConfigurationHelper.GetAppSettings("AppSettings:MySqlConnectionString");
            using (var connection = SqlConnectionHelper.GetOpenConnection(mysqlConnectionString))
            {
                connection.Execute(@" DROP TABLE IF EXISTS `car`;
CREATE TABLE `car` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Make` varchar(100) NOT NULL,
  `ModelName` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8; ");
                connection.Insert(new CarViewModel() { Make = "Honda", ModelName = "Civic" });
                connection.Execute(@" DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `UserId` int(11) NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) NOT NULL,
  `intAge` int(11) NOT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8; ");
                connection.Insert(new UserViewModel() { Age = 42, FirstName = "Jim", LastName = "Smith" });
                connection.Execute(@" DROP TABLE IF EXISTS `guidtest`;
CREATE TABLE `guidtest` (
   `Id` int(11) Not NULL AUTO_INCREMENT,
  `guid` char(36) NULL,
  `name` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8; ");
                connection.Insert(new GUIDTestViewModel {guid=Guid.NewGuid().ToString(), name = "Example" });

                int x = 1;
                do
                {
                    connection.Insert(new User { FirstName = "Jim ", LastName = "Smith " + x, Age = x });
                    x++;
                } while (x < 101);
            }
        }
    }
}
