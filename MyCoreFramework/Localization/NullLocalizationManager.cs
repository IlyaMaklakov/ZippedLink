using System.Collections.Generic;
using System.Threading;

using MyCoreFramework.Localization.Sources;

namespace MyCoreFramework.Localization
{
    public class NullLocalizationManager : ILocalizationManager
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullLocalizationManager Instance { get { return SingletonInstance; } }
        private static readonly NullLocalizationManager SingletonInstance = new NullLocalizationManager();

        public LanguageInfo CurrentLanguage { get { return new LanguageInfo(Thread.CurrentThread.CurrentUICulture.Name, Thread.CurrentThread.CurrentUICulture.DisplayName); } }

        private readonly IReadOnlyList<LanguageInfo> _emptyLanguageArray = new LanguageInfo[0];

        private readonly IReadOnlyList<ILocalizationSource> _emptyLocalizationSourceArray = new ILocalizationSource[0];

        private NullLocalizationManager()
        {
            
        }

        public IReadOnlyList<LanguageInfo> GetAllLanguages()
        {
            return this._emptyLanguageArray;
        }

        public ILocalizationSource GetSource(string name)
        {
            return NullLocalizationSource.Instance;
        }

        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return this._emptyLocalizationSourceArray;
        }
    }
}