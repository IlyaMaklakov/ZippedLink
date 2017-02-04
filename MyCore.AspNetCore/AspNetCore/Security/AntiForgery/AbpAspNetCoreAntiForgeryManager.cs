using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

using MyCore.Web.Security.AntiForgery;

namespace MyCore.AspNetCore.Security.AntiForgery
{
    public class AbpAspNetCoreAntiForgeryManager : IAbpAntiForgeryManager
    {
        public IAbpAntiForgeryConfiguration Configuration { get; }

        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AbpAspNetCoreAntiForgeryManager(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IAbpAntiForgeryConfiguration configuration)
        {
            this.Configuration = configuration;
            this._antiforgery = antiforgery;
            this._httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken()
        {
            return this._antiforgery.GetAndStoreTokens(this._httpContextAccessor.HttpContext).RequestToken;
        }
    }
}