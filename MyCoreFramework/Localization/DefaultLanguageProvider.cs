using System.Collections.Generic;
using System.Collections.Immutable;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;

namespace MyCoreFramework.Localization
{
    public class DefaultLanguageProvider : ILanguageProvider, ITransientDependency
    {
        private readonly ILocalizationConfiguration _configuration;

        public DefaultLanguageProvider(ILocalizationConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            return this._configuration.Languages.ToImmutableList();
        }
    }
}