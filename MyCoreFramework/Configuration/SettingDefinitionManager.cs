using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;

namespace MyCoreFramework.Configuration
{
    /// <summary>
    /// Implements <see cref="ISettingDefinitionManager"/>.
    /// </summary>
    internal class SettingDefinitionManager : ISettingDefinitionManager, ISingletonDependency
    {
        private readonly IIocManager _iocManager;
        private readonly ISettingsConfiguration _settingsConfiguration;
        private readonly IDictionary<string, SettingDefinition> _settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingDefinitionManager(IIocManager iocManager, ISettingsConfiguration settingsConfiguration)
        {
            this._iocManager = iocManager;
            this._settingsConfiguration = settingsConfiguration;
            this._settings = new Dictionary<string, SettingDefinition>();
        }

        public void Initialize()
        {
            var context = new SettingDefinitionProviderContext(this);

            foreach (var providerType in this._settingsConfiguration.Providers)
            {
                using (var provider = this.CreateProvider(providerType))
                {
                    foreach (var settings in provider.Object.GetSettingDefinitions(context))
                    {
                        this._settings[settings.Name] = settings;
                    }
                }
            }
        }

        public SettingDefinition GetSettingDefinition(string name)
        {
            SettingDefinition settingDefinition;
            if (!this._settings.TryGetValue(name, out settingDefinition))
            {
                throw new AbpException("There is no setting defined with name: " + name);
            }

            return settingDefinition;
        }

        public IReadOnlyList<SettingDefinition> GetAllSettingDefinitions()
        {
            return this._settings.Values.ToImmutableList();
        }

        private IDisposableDependencyObjectWrapper<SettingProvider> CreateProvider(Type providerType)
        {
            this._iocManager.RegisterIfNot(providerType, DependencyLifeStyle.Transient); //TODO: Needed?
            return this._iocManager.ResolveAsDisposable<SettingProvider>(providerType);
        }
    }
}