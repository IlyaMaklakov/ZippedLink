using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using MyCoreFramework.Dependency;

namespace MyCoreFramework.Application.Features
{
    /// <summary>
    /// Implements <see cref="IFeatureManager"/>.
    /// </summary>
    internal class FeatureManager : FeatureDefinitionContextBase, IFeatureManager, ISingletonDependency
    {
        private readonly IIocManager _iocManager;
        private readonly IFeatureConfiguration _featureConfiguration;

        public FeatureManager(IIocManager iocManager, IFeatureConfiguration featureConfiguration)
        {
            this._iocManager = iocManager;
            this._featureConfiguration = featureConfiguration;
        }

        public void Initialize()
        {
            foreach (var providerType in this._featureConfiguration.Providers)
            {
                using (var provider = this.CreateProvider(providerType))
                {
                    provider.Object.SetFeatures(this);
                }
            }

            this.Features.AddAllFeatures();
        }

        public Feature Get(string name)
        {
            var feature = this.GetOrNull(name);
            if (feature == null)
            {
                throw new AbpException("There is no feature with name: " + name);
            }

            return feature;
        }

        public IReadOnlyList<Feature> GetAll()
        {
            return this.Features.Values.ToImmutableList();
        }

        private IDisposableDependencyObjectWrapper<FeatureProvider> CreateProvider(Type providerType)
        {
            this._iocManager.RegisterIfNot(providerType); //TODO: Needed?
            return this._iocManager.ResolveAsDisposable<FeatureProvider>(providerType);
        }
    }
}
