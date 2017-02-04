using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using MyCoreFramework.Application.Features;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Localization;
using MyCoreFramework.Reflection;
using MyCoreFramework.Runtime.Session;

namespace MyCoreFramework.Authorization
{
    internal class AuthorizationHelper : IAuthorizationHelper, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }
        public IPermissionChecker PermissionChecker { get; set; }
        public IFeatureChecker FeatureChecker { get; set; }
        public ILocalizationManager LocalizationManager { get; set; }

        private readonly IFeatureChecker _featureChecker;
        private readonly IAuthorizationConfiguration _configuration;

        public AuthorizationHelper(IFeatureChecker featureChecker, IAuthorizationConfiguration configuration)
        {
            this._featureChecker = featureChecker;
            this._configuration = configuration;
            this.AbpSession = NullAbpSession.Instance;
            this.PermissionChecker = NullPermissionChecker.Instance;
            this.LocalizationManager = NullLocalizationManager.Instance;
        }

        public async Task AuthorizeAsync(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes)
        {
            if (!this._configuration.IsEnabled)
            {
                return;
            }

            if (!this.AbpSession.UserId.HasValue)
            {
                throw new AbpAuthorizationException(
                    this.LocalizationManager.GetString(MyCoreConsts.LocalizationSourceName, "CurrentUserDidNotLoginToTheApplication")
                    );
            }

            foreach (var authorizeAttribute in authorizeAttributes)
            {
                await this.PermissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions, authorizeAttribute.Permissions);
            }
        }

        public async Task AuthorizeAsync(MethodInfo methodInfo)
        {
            if (!this._configuration.IsEnabled)
            {
                return;
            }

            if (AllowAnonymous(methodInfo))
            {
                return;
            }
            
            //Authorize
            await this.CheckFeatures(methodInfo);
            await this.CheckPermissions(methodInfo);
        }

        private async Task CheckFeatures(MethodInfo methodInfo)
        {
            var featureAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<RequiresFeatureAttribute>(
                    methodInfo
                    );

            if (featureAttributes.Count <= 0)
            {
                return;
            }

            foreach (var featureAttribute in featureAttributes)
            {
                await this._featureChecker.CheckEnabledAsync(featureAttribute.RequiresAll, featureAttribute.Features);
            }
        }

        private async Task CheckPermissions(MethodInfo methodInfo)
        {
            var authorizeAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType(
                    methodInfo
                ).OfType<IAbpAuthorizeAttribute>().ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            await this.AuthorizeAsync(authorizeAttributes);
        }

        private static bool AllowAnonymous(MethodInfo methodInfo)
        {
            return ReflectionHelper.GetAttributesOfMemberAndDeclaringType(methodInfo)
                .OfType<IAbpAllowAnonymousAttribute>().Any();
        }
    }
}