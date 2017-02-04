using System;
using System.Collections.Generic;

namespace MyCore.Web.Api.ProxyScripting.Configuration
{
    public class ApiProxyScriptingConfiguration : IApiProxyScriptingConfiguration
    {
        public IDictionary<string, Type> Generators { get; }

        public ApiProxyScriptingConfiguration()
        {
            this.Generators = new Dictionary<string, Type>();
        }
    }
}