using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Localization;
using MyCoreFramework.UI.Inputs;

namespace MyCoreFramework.Application.Features
{
    /// <summary>
    /// Base for implementing <see cref="IFeatureDefinitionContext"/>.
    /// </summary>
    public abstract class FeatureDefinitionContextBase : IFeatureDefinitionContext
    {
        protected readonly FeatureDictionary Features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureDefinitionContextBase"/> class.
        /// </summary>
        protected FeatureDefinitionContextBase()
        {
            this.Features = new FeatureDictionary();
        }

        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="displayName">Display name of the feature</param>
        /// <param name="description">A brief description for this feature</param>
        /// <param name="scope">Feature scope</param>
        /// <param name="inputType">Input type</param>
        public Feature Create(string name, string defaultValue, ILocalizableString displayName = null,
            ILocalizableString description = null, FeatureScopes scope = FeatureScopes.All, IInputType inputType = null)
        {
            if (this.Features.ContainsKey(name))
            {
                throw new AbpException("There is already a feature with name: " + name);
            }

            var feature = new Feature(name, defaultValue, displayName, description, scope, inputType);
            this.Features[feature.Name] = feature;
            return feature;

        }

        /// <summary>
        /// Gets a feature with given name or null if can not find.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <returns>
        ///   <see cref="Feature" /> object or null
        /// </returns>
        public Feature GetOrNull(string name)
        {
            return this.Features.GetOrDefault(name);
        }
    }
}