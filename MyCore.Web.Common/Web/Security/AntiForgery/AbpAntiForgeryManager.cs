using System;

using Castle.Core.Logging;

using MyCoreFramework.Dependency;

namespace MyCore.Web.Security.AntiForgery
{
    public class AbpAntiForgeryManager : IAbpAntiForgeryManager, IAbpAntiForgeryValidator, ITransientDependency
    {
        public ILogger Logger { protected get; set; }

        public IAbpAntiForgeryConfiguration Configuration { get; }

        public AbpAntiForgeryManager(IAbpAntiForgeryConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Logger = NullLogger.Instance;
        }

        public virtual string GenerateToken()
        {
            return Guid.NewGuid().ToString("D");
        }

        public virtual bool IsValid(string cookieValue, string tokenValue)
        {
            return cookieValue == tokenValue;
        }
    }
}