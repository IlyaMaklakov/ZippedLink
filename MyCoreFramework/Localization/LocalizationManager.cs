using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Castle.Core.Logging;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Localization.Dictionaries;
using MyCoreFramework.Localization.Sources;

namespace MyCoreFramework.Localization
{
    internal class LocalizationManager : ILocalizationManager
    {
        public ILogger Logger { get; set; }

        private readonly ILanguageManager _languageManager;
        private readonly ILocalizationConfiguration _configuration;
        private readonly IIocResolver _iocResolver;
        private readonly IDictionary<string, ILocalizationSource> _sources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalizationManager(
            ILanguageManager languageManager,
            ILocalizationConfiguration configuration, 
            IIocResolver iocResolver)
        {
            this.Logger = NullLogger.Instance;
            this._languageManager = languageManager;
            this._configuration = configuration;
            this._iocResolver = iocResolver;
            this._sources = new Dictionary<string, ILocalizationSource>();
        }

        public void Initialize()
        {
            this.InitializeSources();
        }

        private void InitializeSources()
        {
            if (!this._configuration.IsEnabled)
            {
                this.Logger.Debug("Localization disabled.");
                return;
            }

            this.Logger.Debug(string.Format("Initializing {0} localization sources.", this._configuration.Sources.Count));
            foreach (var source in this._configuration.Sources)
            {
                if (this._sources.ContainsKey(source.Name))
                {
                    throw new AbpException("There are more than one source with name: " + source.Name + "! Source name must be unique!");
                }

                this._sources[source.Name] = source;
                source.Initialize(this._configuration, this._iocResolver);

                //Extending dictionaries
                if (source is IDictionaryBasedLocalizationSource)
                {
                    var dictionaryBasedSource = source as IDictionaryBasedLocalizationSource;
                    var extensions = this._configuration.Sources.Extensions.Where(e => e.SourceName == source.Name).ToList();
                    foreach (var extension in extensions)
                    {
                        extension.DictionaryProvider.Initialize(source.Name);
                        foreach (var extensionDictionary in extension.DictionaryProvider.Dictionaries.Values)
                        {
                            dictionaryBasedSource.Extend(extensionDictionary);
                        }
                    }
                }

                this.Logger.Debug("Initialized localization source: " + source.Name);
            }
        }

        /// <summary>
        /// Gets a localization source with name.
        /// </summary>
        /// <param name="name">Unique name of the localization source</param>
        /// <returns>The localization source</returns>
        public ILocalizationSource GetSource(string name)
        {
            if (!this._configuration.IsEnabled)
            {
                return NullLocalizationSource.Instance;
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            ILocalizationSource source;
            if (!this._sources.TryGetValue(name, out source))
            {
                throw new AbpException("Can not find a source with name: " + name);
            }

            return source;
        }

        /// <summary>
        /// Gets all registered localization sources.
        /// </summary>
        /// <returns>List of sources</returns>
        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return this._sources.Values.ToImmutableList();
        }
    }
}