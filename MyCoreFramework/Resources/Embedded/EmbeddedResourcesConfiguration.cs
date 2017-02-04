using System.Collections.Generic;

namespace MyCoreFramework.Resources.Embedded
{
    public class EmbeddedResourcesConfiguration : IEmbeddedResourcesConfiguration
    {
        public List<EmbeddedResourceSet> Sources { get; }

        public EmbeddedResourcesConfiguration()
        {
            this.Sources = new List<EmbeddedResourceSet>();
        }
    }
}