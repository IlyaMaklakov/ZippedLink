using System;
using System.Collections.Generic;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;

namespace MyCoreFramework.Resources.Embedded
{
    public class EmbeddedResourceManager : IEmbeddedResourceManager, ISingletonDependency
    {
        private readonly IEmbeddedResourcesConfiguration _configuration;
        private readonly Lazy<Dictionary<string, EmbeddedResourceItem>> _resources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmbeddedResourceManager(IEmbeddedResourcesConfiguration configuration)
        {
            this._configuration = configuration;
            this._resources = new Lazy<Dictionary<string, EmbeddedResourceItem>>(
                this.CreateResourcesDictionary,
                true
            );
        }

        /// <inheritdoc/>
        public EmbeddedResourceItem GetResource(string fullPath)
        {
            return this._resources.Value.GetOrDefault(fullPath);
        }

        private Dictionary<string, EmbeddedResourceItem> CreateResourcesDictionary()
        {
            var resources = new Dictionary<string, EmbeddedResourceItem>();

            foreach (var source in this._configuration.Sources)
            {
                source.AddResources(resources);
            }

            return resources;
        }
    }
}