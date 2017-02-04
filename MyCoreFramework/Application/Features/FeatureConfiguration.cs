using MyCoreFramework.Collections;

namespace MyCoreFramework.Application.Features
{
    internal class FeatureConfiguration : IFeatureConfiguration
    {
        public ITypeList<FeatureProvider> Providers { get; private set; }

        public FeatureConfiguration()
        {
            this.Providers = new TypeList<FeatureProvider>();
        }
    }
}