using System.Collections.Generic;

using MyCoreFramework.Collections.Extensions;

namespace MyCore.Web.Api.ProxyScripting
{
    public class ApiProxyGenerationOptions
    {
        public string GeneratorType { get; set; }

        public bool UseCache { get; set; }

        public string[] Modules { get; set; }

        public string[] Controllers { get; set; }

        public string[] Actions { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public ApiProxyGenerationOptions(string generatorType, bool useCache = true)
        {
            this.GeneratorType = generatorType;
            this.UseCache = useCache;

            this.Properties = new Dictionary<string, string>();
        }

        public bool IsPartialRequest()
        {
            return !(this.Modules.IsNullOrEmpty() && this.Controllers.IsNullOrEmpty() && this.Actions.IsNullOrEmpty());
        }
    }
}