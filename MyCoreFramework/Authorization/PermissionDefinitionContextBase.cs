using MyCoreFramework.Application.Features;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Localization;
using MyCoreFramework.MultiTenancy;

namespace MyCoreFramework.Authorization
{
    internal abstract class PermissionDefinitionContextBase : IPermissionDefinitionContext
    {
        protected readonly PermissionDictionary Permissions;

        protected PermissionDefinitionContextBase()
        {
            this.Permissions = new PermissionDictionary();
        }

        public Permission CreatePermission(
            string name, 
            ILocalizableString displayName = null, 
            ILocalizableString description = null, 
            MultiTenancySides multiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
            IFeatureDependency featureDependency = null)
        {
            if (this.Permissions.ContainsKey(name))
            {
                throw new AbpException("There is already a permission with name: " + name);
            }

            var permission = new Permission(name, displayName, description, multiTenancySides, featureDependency);
            this.Permissions[permission.Name] = permission;
            return permission;
        }

        public Permission GetPermissionOrNull(string name)
        {
            return this.Permissions.GetOrDefault(name);
        }
    }
}
