using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyCoreFramework.Application.Features;
using MyCoreFramework.Dependency;
using MyCoreFramework.Runtime.Session;

namespace MyCore.Web.Features
{
    public class FeaturesScriptManager : IFeaturesScriptManager, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IFeatureManager _featureManager;
        private readonly IFeatureChecker _featureChecker;

        public FeaturesScriptManager(IFeatureManager featureManager, IFeatureChecker featureChecker)
        {
            this._featureManager = featureManager;
            this._featureChecker = featureChecker;

            this.AbpSession = NullAbpSession.Instance;
        }

        public async Task<string> GetScriptAsync()
        {
            var allFeatures = this._featureManager.GetAll().ToList();
            var currentValues = new Dictionary<string, string>();

            if (this.AbpSession.TenantId.HasValue)
            {
                var currentTenantId = this.AbpSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    currentValues[feature.Name] = await this._featureChecker.GetValueAsync(currentTenantId, feature.Name);
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    currentValues[feature.Name] = feature.DefaultValue;
                }
            }

            var script = new StringBuilder();

            script.AppendLine("(function() {");

            script.AppendLine();

            script.AppendLine("    abp.features = abp.features || {};");

            script.AppendLine();

            script.AppendLine("    abp.features.allFeatures = {");

            for (var i = 0; i < allFeatures.Count; i++)
            {
                var feature = allFeatures[i];
                script.AppendLine("        '" + feature.Name.Replace("'", @"\'") + "': {");
                script.AppendLine("             value: '" + currentValues[feature.Name].Replace(@"\", @"\\").Replace("'", @"\'") + "'");
                script.Append("        }");

                if (i < allFeatures.Count - 1)
                {
                    script.AppendLine(",");
                }
                else
                {
                    script.AppendLine();
                }
            }

            script.AppendLine("    };");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}