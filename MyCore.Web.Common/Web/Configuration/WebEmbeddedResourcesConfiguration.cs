using System.Collections.Generic;

namespace MyCore.Web.Configuration
{
    internal class WebEmbeddedResourcesConfiguration : IWebEmbeddedResourcesConfiguration
    {
        public HashSet<string> IgnoredFileExtensions { get; }

        public WebEmbeddedResourcesConfiguration()
        {
            this.IgnoredFileExtensions = new HashSet<string>
            {
                "cshtml",
                "config"
            };
        }
    }
}