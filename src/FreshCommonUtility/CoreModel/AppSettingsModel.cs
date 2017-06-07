namespace FreshCommonUtility.CoreModel
{
    /// <summary>
    /// App setting model.
    /// </summary>
    public class AppSettingsModel
    {
        /// <summary>
        /// Redis cach config setting.
        /// </summary>
        public RedisCaching RedisCaching { get; set; }

        /// <summary>
        /// Mysql connection string.
        /// </summary>
        public string MySqlConnectionString { get; set; }

        /// <summary>
        /// Email config model.
        /// </summary>
        public EmailServerConfigModel EmailServerConfig { get; set; }
    }

    /// <summary>
    /// Redis
    /// </summary>
    public class RedisCaching
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>  
        /// 链接信息
        /// </summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// Email server config class.
    /// </summary>
    public class EmailServerConfigModel
    {
        /// <summary>
        /// Email from name
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// Email from address.
        /// </summary>
        public string FromEmailAddress { get; set; }
        /// <summary>
        /// Email smtp server address.
        /// </summary>
        public string EmailSmtpServerAddress { get; set; }
        /// <summary>
        /// Email send password.
        /// </summary>
        public string FromEmailPassword { get; set; }
        /// <summary>
        /// Enalble secret password
        /// </summary>
        public bool PasswordEnabledSecret { get; set; }
    }
}
