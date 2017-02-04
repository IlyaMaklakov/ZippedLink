using System.Collections.Generic;
using System.Threading.Tasks;

using MyCoreFramework.Application.Features;
using MyCoreFramework.Authorization;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;
using MyCoreFramework.Localization;
using MyCoreFramework.MultiTenancy;
using MyCoreFramework.Runtime.Session;

namespace MyCoreFramework.Application.Navigation
{
    internal class UserNavigationManager : IUserNavigationManager, ITransientDependency
    {
        public IPermissionChecker PermissionChecker { get; set; }

        public IAbpSession AbpSession { get; set; }

        private readonly INavigationManager _navigationManager;
        private readonly ILocalizationContext _localizationContext;
        private readonly IIocResolver _iocResolver;

        public UserNavigationManager(
            INavigationManager navigationManager,
            ILocalizationContext localizationContext,
            IIocResolver iocResolver)
        {
            this._navigationManager = navigationManager;
            this._localizationContext = localizationContext;
            this._iocResolver = iocResolver;
            this.PermissionChecker = NullPermissionChecker.Instance;
            this.AbpSession = NullAbpSession.Instance;
        }

        public async Task<UserMenu> GetMenuAsync(string menuName, UserIdentifier user)
        {
            var menuDefinition = this._navigationManager.Menus.GetOrDefault(menuName);
            if (menuDefinition == null)
            {
                throw new AbpException("There is no menu with given name: " + menuName);
            }

            var userMenu = new UserMenu(menuDefinition, this._localizationContext);
            await this.FillUserMenuItems(user, menuDefinition.Items, userMenu.Items);
            return userMenu;
        }

        public async Task<IReadOnlyList<UserMenu>> GetMenusAsync(UserIdentifier user)
        {
            var userMenus = new List<UserMenu>();

            foreach (var menu in this._navigationManager.Menus.Values)
            {
                userMenus.Add(await this.GetMenuAsync(menu.Name, user));
            }

            return userMenus;
        }

        private async Task<int> FillUserMenuItems(UserIdentifier user, IList<MenuItemDefinition> menuItemDefinitions, IList<UserMenuItem> userMenuItems)
        {
            //TODO: Can be optimized by re-using FeatureDependencyContext.

            var addedMenuItemCount = 0;

            using (var featureDependencyContext = this._iocResolver.ResolveAsDisposable<FeatureDependencyContext>())
            {
                featureDependencyContext.Object.TenantId = user == null ? null : user.TenantId;

                foreach (var menuItemDefinition in menuItemDefinitions)
                {
                    if (menuItemDefinition.RequiresAuthentication && user == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(menuItemDefinition.RequiredPermissionName) && (user == null || !(await this.PermissionChecker.IsGrantedAsync(user, menuItemDefinition.RequiredPermissionName))))
                    {
                        continue;
                    }

                    if (menuItemDefinition.FeatureDependency != null &&
                        (this.AbpSession.MultiTenancySide == MultiTenancySides.Tenant || (user != null && user.TenantId != null)) &&
                        !(await menuItemDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object)))
                    {
                        continue;
                    }

                    var userMenuItem = new UserMenuItem(menuItemDefinition, this._localizationContext);
                    if (menuItemDefinition.IsLeaf || (await this.FillUserMenuItems(user, menuItemDefinition.Items, userMenuItem.Items)) > 0)
                    {
                        userMenuItems.Add(userMenuItem);
                        ++addedMenuItemCount;
                    }
                }
            }

            return addedMenuItemCount;
        }
    }
}
