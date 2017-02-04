using System.Linq;

using MyCore.Web.Api.ProxyScripting;
using MyCore.Web.Api.ProxyScripting.Generators.JQuery;

using MyCoreFramework.Extensions;
using MyCoreFramework.Runtime.Validation;

namespace MyCore.AspNetCore.Mvc.Proxying
{
    public class ApiProxyGenerationModel : IShouldNormalize
    {
        public string Type { get; set; }

        public bool UseCache { get; set; }

        public string Modules { get; set; }

        public string Controllers { get; set; }

        public string Actions { get; set; }

        public ApiProxyGenerationModel()
        {
            this.UseCache = true;
        }

        public void Normalize()
        {
            if (this.Type.IsNullOrEmpty())
            {
                this.Type = JQueryProxyScriptGenerator.Name;
            }
        }

        public ApiProxyGenerationOptions CreateOptions()
        {
            var options = new ApiProxyGenerationOptions(this.Type, this.UseCache);

            if (!this.Modules.IsNullOrEmpty())
            {
                options.Modules = this.Modules.Split('|').Select(m => m.Trim()).ToArray();
            }

            if (!this.Controllers.IsNullOrEmpty())
            {
                options.Controllers = this.Controllers.Split('|').Select(m => m.Trim()).ToArray();
            }

            if (!this.Actions.IsNullOrEmpty())
            {
                options.Actions = this.Actions.Split('|').Select(m => m.Trim()).ToArray();
            }

            return options;
        }
    }
}