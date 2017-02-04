using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;

namespace MyCoreFramework.Net.Mail.Smtp
{
    /// <summary>
    /// Used to send emails over SMTP.
    /// </summary>
    public class SmtpEmailSender : EmailSenderBase, ISmtpEmailSender, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration _configuration;

        /// <summary>
        /// Creates a new <see cref="SmtpEmailSender"/>.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public SmtpEmailSender(ISmtpEmailSenderConfiguration configuration)
            : base(configuration)
        {
            this._configuration = configuration;
        }

        public SmtpClient BuildClient()
        {
            var host = this._configuration.Host;
            var port = this._configuration.Port;

            var smtpClient = new SmtpClient(host, port);
            try
            {
                if (this._configuration.EnableSsl)
                {
                    smtpClient.EnableSsl = true;
                }

                if (this._configuration.UseDefaultCredentials)
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = false;

                    var userName = this._configuration.UserName;
                    if (!userName.IsNullOrEmpty())
                    {
                        var password = this._configuration.Password;
                        var domain = this._configuration.Domain;
                        smtpClient.Credentials = !domain.IsNullOrEmpty()
                            ? new NetworkCredential(userName, password, domain)
                            : new NetworkCredential(userName, password);
                    }
                }

                return smtpClient;
            }
            catch
            {
                smtpClient.Dispose();
                throw;
            }
        }

        protected override async Task SendEmailAsync(MailMessage mail)
        {
            using (var smtpClient = this.BuildClient())
            {
                await smtpClient.SendMailAsync(mail);
            }
        }

        protected override void SendEmail(MailMessage mail)
        {
            using (var smtpClient = this.BuildClient())
            {
                smtpClient.Send(mail);
            }
        }
    }
}