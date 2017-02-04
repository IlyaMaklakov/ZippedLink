﻿using MyCoreFramework.Configuration;
using MyCoreFramework.Extensions;

namespace MyCoreFramework.Net.Mail
{
    /// <summary>
    /// Implementation of <see cref="IEmailSenderConfiguration"/> that reads settings
    /// from <see cref="ISettingManager"/>.
    /// </summary>
    public abstract class EmailSenderConfiguration : IEmailSenderConfiguration
    {
        public virtual string DefaultFromAddress
        {
            get { return this.GetNotEmptySettingValue(EmailSettingNames.DefaultFromAddress); }
        }

        public virtual string DefaultFromDisplayName
        {
            get { return this.SettingManager.GetSettingValue(EmailSettingNames.DefaultFromDisplayName); }
        }

        protected readonly ISettingManager SettingManager;

        /// <summary>
        /// Creates a new <see cref="EmailSenderConfiguration"/>.
        /// </summary>
        protected EmailSenderConfiguration(ISettingManager settingManager)
        {
            this.SettingManager = settingManager;
        }

        /// <summary>
        /// Gets a setting value by checking. Throws <see cref="AbpException"/> if it's null or empty.
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <returns>Value of the setting</returns>
        protected string GetNotEmptySettingValue(string name)
        {
            var value = this.SettingManager.GetSettingValue(name);

            if (value.IsNullOrEmpty())
            {
                throw new AbpException($"Setting value for '{name}' is null or empty!");
            }

            return value;
        }
    }
}