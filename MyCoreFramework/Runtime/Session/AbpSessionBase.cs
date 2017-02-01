using System;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.MultiTenancy;

namespace MyCoreFramework.Runtime.Session
{
    public abstract class AbpSessionBase : ISession
    {
        public const string SessionOverrideContextKey = "MyCoreFramework.Session.Override";

        public IMultiTenancyConfig MultiTenancy { get; }

        public abstract long? UserId { get; }

        public abstract int? TenantId { get; }

        public abstract long? ImpersonatorUserId { get; }

        public abstract int? ImpersonatorTenantId { get; }

        public virtual MultiTenancySides MultiTenancySide
        {
            get
            {
                return this.MultiTenancy.IsEnabled && !this.TenantId.HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }
        }

        protected SessionOverride OverridedValue => this.SessionOverrideScopeProvider.GetValue(SessionOverrideContextKey);
        protected IAmbientScopeProvider<SessionOverride> SessionOverrideScopeProvider { get; }

        protected AbpSessionBase(IMultiTenancyConfig multiTenancy, IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider)
        {
            this.MultiTenancy = multiTenancy;
            this.SessionOverrideScopeProvider = sessionOverrideScopeProvider;
        }

        public IDisposable Use(int? tenantId, long? userId)
        {
            return this.SessionOverrideScopeProvider.BeginScope(SessionOverrideContextKey, new SessionOverride(tenantId, userId));
        }
    }
}
