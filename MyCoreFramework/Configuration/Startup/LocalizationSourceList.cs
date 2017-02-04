using System.Collections.Generic;

using MyCoreFramework.Localization.Sources;

namespace MyCoreFramework.Configuration.Startup
{
    /// <summary>
    /// A specialized list to store <see cref="ILocalizationSource"/> object.
    /// </summary>
    internal class LocalizationSourceList : List<ILocalizationSource>, ILocalizationSourceList
    {
        public IList<LocalizationSourceExtensionInfo> Extensions { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalizationSourceList()
        {
            this.Extensions = new List<LocalizationSourceExtensionInfo>();
        }
    }
}