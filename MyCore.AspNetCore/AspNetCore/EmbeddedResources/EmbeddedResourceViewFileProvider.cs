using MyCoreFramework.Dependency;
using MyCoreFramework.Resources.Embedded;

namespace MyCore.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceViewFileProvider : EmbeddedResourceFileProvider
    {
        public EmbeddedResourceViewFileProvider(IIocResolver iocResolver) 
            : base(iocResolver)
        {
        }

        protected override bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != "cshtml";
        }
    }
}