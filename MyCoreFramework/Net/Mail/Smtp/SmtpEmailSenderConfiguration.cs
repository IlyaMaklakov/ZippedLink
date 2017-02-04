using MyCoreFramework.Configuration;
using MyCoreFramework.Dependency;

namespace MyCoreFramework.Net.Mail.Smtp
{
    /// <summary>
    /// Implementation of <see cref="ISmtpEmailSenderConfiguration"/> that reads settings
    /// from <see cref="ISettingManager"/>.
    /// </summary>
    public class SmtpEmailSenderConfiguration : EmailSenderConfiguration, ISmtpEmailSenderConfiguration, ITransientDependency
    {
        /// <summary>
        /// SMTP Host name/IP.
        /// </summary>
        public virtual string Host
        {
            get { return this.GetNotEmptySettingValue(EmailSettingNames.Smtp.Host); }
        }

        /// <summary>
        /// SMTP Port.
        /// </summary>
        public virtual int Port
        {
            get { return this.SettingManager.GetSettingValue<int>(EmailSettingNames.Smtp.Port); }
        }

        /// <summary>
        /// User name to login to SMTP server.
        /// </summary>
        public virtual string UserName
        {
            get { return this.GetNotEmptySettingValue(EmailSettingNames.Smtp.UserName); }
        }

        /// <summary>
        /// Password to login to SMTP server.
        /// </summary>
        public virtual string Password
        {
            get { return this.GetNotEmptySettingValue(EmailSettingNames.Smtp.Password); }
        }

        /// <summary>
        /// Domain name to login to SMTP server.
        /// </summary>
        public virtual string Domain
        {
            get { return this.SettingManager.GetSettingValue(EmailSettingNames.Smtp.Domain); }
        }

        /// <summary>
        /// Is SSL enabled?
        /// </summary>
        public virtual bool EnableSsl
        {
            get { return this.SettingManager.GetSettingValue<bool>(EmailSettingNames.Smtp.EnableSsl); }
        }

        /// <summary>
        /// Use default credentials?
        /// </summary>
        public virtual bool UseDefaultCredentials
        {
            get { return this.SettingManager.GetSettingValue<bool>(EmailSettingNames.Smtp.UseDefaultCredentials); }
        }

        /// <summary>
        /// Creates a new <see cref="SmtpEmailSenderConfiguration"/>.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        public SmtpEmailSenderConfiguration(ISettingManager settingManager)
            : base(settingManager)
        {

        }
    }
}