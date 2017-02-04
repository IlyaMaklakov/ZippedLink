using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.MultiTenancy;
using MyCoreFramework.Runtime.Security;

namespace MyCoreFramework.Runtime.Session
{
    /// <summary>
    /// Implements <see cref="IAbpSession"/> to get session properties from claims of <see cref="Thread.CurrentPrincipal"/>.
    /// </summary>
    public class ClaimsAbpSession : AbpSessionBase, ISingletonDependency
    {
        public override long? UserId
        {
            get
            {
                if (this.OverridedValue != null)
                {
                    return this.OverridedValue.UserId;
                }

                var userIdClaim = this.PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                long userId;
                if (!long.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public override int? TenantId
        {
            get
            {
                if (!this.MultiTenancy.IsEnabled)
                {
                    return MultiTenancyConsts.DefaultTenantId;
                }

                if (this.OverridedValue != null)
                {
                    return this.OverridedValue.TenantId;
                }

                var tenantIdClaim = this.PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId);
                if (!string.IsNullOrEmpty(tenantIdClaim?.Value))
                {
                    return Convert.ToInt32(tenantIdClaim.Value);
                }

                if (this.UserId == null)
                {
                    //Resolve tenant id from request only if user has not logged in!
                    return this.TenantResolver.ResolveTenantId();
                }
                
                return null;
            }
        }

        public override long? ImpersonatorUserId
        {
            get
            {
                var impersonatorUserIdClaim = this.PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.ImpersonatorUserId);
                if (string.IsNullOrEmpty(impersonatorUserIdClaim?.Value))
                {
                    return null;
                }

                return Convert.ToInt64(impersonatorUserIdClaim.Value);
            }
        }

        public override int? ImpersonatorTenantId
        {
            get
            {
                if (!this.MultiTenancy.IsEnabled)
                {
                    return MultiTenancyConsts.DefaultTenantId;
                }

                var impersonatorTenantIdClaim = this.PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.ImpersonatorTenantId);
                if (string.IsNullOrEmpty(impersonatorTenantIdClaim?.Value))
                {
                    return null;
                }

                return Convert.ToInt32(impersonatorTenantIdClaim.Value);
            }
        }

        protected IPrincipalAccessor PrincipalAccessor { get; }
        protected ITenantResolver TenantResolver { get; }

        public ClaimsAbpSession(
            IPrincipalAccessor principalAccessor,
            IMultiTenancyConfig multiTenancy,
            ITenantResolver tenantResolver,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider)
            : base(
                  multiTenancy, 
                  sessionOverrideScopeProvider)
        {
            this.TenantResolver = tenantResolver;
            this.PrincipalAccessor = principalAccessor;
        }
    }
}