using System.Collections.Generic;

using MyCoreFramework.Localization;

namespace MyCoreFramework.Configuration.Startup
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    internal class LocalizationConfiguration : ILocalizationConfiguration
    {
        /// <inheritdoc/>
        public IList<LanguageInfo> Languages { get; private set; }

        /// <inheritdoc/>
        public ILocalizationSourceList Sources { get; private set; }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <inheritdoc/>
        public bool ReturnGivenTextIfNotFound { get; set; }

        /// <inheritdoc/>
        public bool WrapGivenTextIfNotFound { get; set; }

        /// <inheritdoc/>
        public bool HumanizeTextIfNotFound { get; set; }

        public LocalizationConfiguration()
        {
            this.Languages = new List<LanguageInfo>();
            this.Sources = new LocalizationSourceList();

            this.IsEnabled = true;
            this.ReturnGivenTextIfNotFound = true;
            this.WrapGivenTextIfNotFound = true;
            this.HumanizeTextIfNotFound = true;
        }
    }
}
