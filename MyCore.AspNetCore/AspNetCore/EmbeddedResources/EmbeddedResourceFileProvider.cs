using System;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using MyCore.Web.Configuration;

using MyCoreFramework.Dependency;
using MyCoreFramework.Resources.Embedded;

namespace MyCore.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceFileProvider : IFileProvider
    {
        private readonly Lazy<IEmbeddedResourceManager> _embeddedResourceManager;
        private readonly Lazy<IWebEmbeddedResourcesConfiguration> _configuration;

        public EmbeddedResourceFileProvider(IIocResolver iocResolver)
        {
            this._embeddedResourceManager = new Lazy<IEmbeddedResourceManager>(
                () => iocResolver.Resolve<IEmbeddedResourceManager>(),
                true
            );

            this._configuration = new Lazy<IWebEmbeddedResourcesConfiguration>(
                () => iocResolver.Resolve<IWebEmbeddedResourcesConfiguration>(),
                true
            );
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var resource = this._embeddedResourceManager.Value.GetResource(subpath);

            if (resource == null || this.IsIgnoredFile(resource))
            {
                return new NotFoundFileInfo(subpath);
            }

            return new EmbeddedResourceItemFileInfo(resource);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            //TODO: Implement...?

            return new NotFoundDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        protected virtual bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null && this._configuration.Value.IgnoredFileExtensions.Contains(resource.FileExtension);
        }
    }
}