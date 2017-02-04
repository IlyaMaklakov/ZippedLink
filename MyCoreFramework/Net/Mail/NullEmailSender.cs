using System.Net.Mail;
using System.Threading.Tasks;

using Castle.Core.Logging;

namespace MyCoreFramework.Net.Mail
{
    /// <summary>
    /// This class is an implementation of <see cref="IEmailSender"/> as similar to null pattern.
    /// It does not send emails but logs them.
    /// </summary>
    public class NullEmailSender : EmailSenderBase
    {
        public ILogger Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="NullEmailSender"/> object.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public NullEmailSender(IEmailSenderConfiguration configuration)
            : base(configuration)
        {
            this.Logger = NullLogger.Instance;
        }

        protected override Task SendEmailAsync(MailMessage mail)
        {
            this.Logger.Warn("USING NullEmailSender!");
            this.Logger.Debug("SendEmailAsync:");
            this.LogEmail(mail);
            return Task.FromResult(0);
        }

        protected override void SendEmail(MailMessage mail)
        {
            this.Logger.Warn("USING NullEmailSender!");
            this.Logger.Debug("SendEmail:");
            this.LogEmail(mail);
        }

        private void LogEmail(MailMessage mail)
        {
            this.Logger.Debug(mail.To.ToString());
            this.Logger.Debug(mail.CC.ToString());
            this.Logger.Debug(mail.Subject);
            this.Logger.Debug(mail.Body);
        }
    }
}