using MyCoreFramework.Dependency;

namespace MyCoreFramework.Authorization
{
    internal class PermissionDependencyContext : IPermissionDependencyContext, ITransientDependency
    {
        public UserIdentifier User { get; set; }

        public IIocResolver IocResolver { get; }
        
        public IPermissionChecker PermissionChecker { get; set; }

        public PermissionDependencyContext(IIocResolver iocResolver)
        {
            this.IocResolver = iocResolver;
            this.PermissionChecker = NullPermissionChecker.Instance;
        }
    }
}