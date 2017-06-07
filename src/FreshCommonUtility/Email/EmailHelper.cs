using System;
using System.Text.RegularExpressions;
using FreshCommonUtility.Configure;
using FreshCommonUtility.CoreModel;
using MailKit.Net.Smtp;
using MimeKit;

namespace FreshCommonUtility.Email
{
    /// <summary>
    /// Email helper.
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// Send email method.
        /// </summary>
        /// <param name="toEmailAddress">send to email address.</param>
        /// <param name="subject">email subject.</param>
        /// <param name="message">send email content,txt or html boy,if html body ,you should set isHtmlBody param is true.</param>
        /// <param name="toName">send to email name,could by null,if null will use default string.</param>
        /// <param name="isHtmlBody">flag the send email </param>
        public static void SendEmail(string toEmailAddress, string subject, string message, string toName = null, bool isHtmlBody = false)
        {
            if (string.IsNullOrEmpty(toEmailAddress)) throw new ArgumentNullException(nameof(toEmailAddress));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            var emailSereverConfigModel =
                AppConfigurationHelper.GetAppSettings<EmailServerConfigModel>("AppSettings:EmailServerConfig");
            if (string.IsNullOrEmpty(emailSereverConfigModel?.FromEmailAddress)
                || string.IsNullOrEmpty(emailSereverConfigModel.FromEmailPassword)
                || string.IsNullOrEmpty(emailSereverConfigModel.EmailSmtpServerAddress))
            {
                throw new Exception("Email config error,please check you config.FromEmailAddress,EmailSmtpServerAddress,FromEmailPassword filed is must have.");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSereverConfigModel.FromName, emailSereverConfigModel.FromEmailAddress));

            emailMessage.To.Add(new MailboxAddress(toName ?? "FreshMan Server Email", toEmailAddress));
            emailMessage.Subject = subject;
            if (isHtmlBody)
            {
                var bodyBuilder = new BodyBuilder { HtmlBody = message };
                emailMessage.Body = bodyBuilder.ToMessageBody();
            }
            else
            {
                emailMessage.Body = new TextPart("plain") { Text = message };
            }
            using (var client = new SmtpClient())
            {
                client.Connect(emailSereverConfigModel.EmailSmtpServerAddress);
                client.Authenticate(emailSereverConfigModel.FromEmailAddress, emailSereverConfigModel.FromEmailPassword);
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Send email method.
        /// </summary>
        /// <param name="toEmailAddress">send to email address.</param>
        /// <param name="subject">email subject.</param>
        /// <param name="message">send email content,txt or html boy,if html body ,you should set isHtmlBody param is true.</param>
        /// <param name="toName">send to email name,could by null,if null will use default string.</param>
        /// <param name="isHtmlBody">flag the send email </param>
        public static async void SendEmailAsync(string toEmailAddress, string subject, string message,
            string toName = null, bool isHtmlBody = false)
        {
            if (string.IsNullOrEmpty(toEmailAddress)) throw new ArgumentNullException(nameof(toEmailAddress));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            var emailSereverConfigModel =
                AppConfigurationHelper.GetAppSettings<EmailServerConfigModel>("AppSettings:EmailServerConfig");
            if (string.IsNullOrEmpty(emailSereverConfigModel?.FromEmailAddress)
                || string.IsNullOrEmpty(emailSereverConfigModel.FromEmailPassword)
                || string.IsNullOrEmpty(emailSereverConfigModel.EmailSmtpServerAddress))
            {
                throw new Exception("Email config error,please check you config.FromEmailAddress,EmailSmtpServerAddress,FromEmailPassword filed is must have.");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSereverConfigModel.FromName, emailSereverConfigModel.FromEmailAddress));

            emailMessage.To.Add(new MailboxAddress(toName ?? "FreshMan Server Email", toEmailAddress));
            emailMessage.Subject = subject;
            if (isHtmlBody)
            {
                var bodyBuilder = new BodyBuilder { HtmlBody = message };
                emailMessage.Body = bodyBuilder.ToMessageBody();
            }
            else
            {
                emailMessage.Body = new TextPart("plain") { Text = message };
            }
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSereverConfigModel.EmailSmtpServerAddress);
                await client.AuthenticateAsync(emailSereverConfigModel.FromEmailAddress, emailSereverConfigModel.FromEmailPassword);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Chech email address.
        /// </summary>
        /// <param name="emailAddress">email address.</param>
        /// <returns>this email address is valid.</returns>
        public static bool IsEmailAddress(string emailAddress)
        {
            var regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
            return regex.IsMatch(emailAddress);
        }
    }
}
