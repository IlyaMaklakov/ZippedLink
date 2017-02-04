using System;

using Castle.Core.Logging;

using Microsoft.AspNetCore.Http;

using MyCoreFramework.Auditing;

namespace MyCore.AspNetCore.Mvc.Auditing
{
    public class HttpContextClientInfoProvider : IClientInfoProvider
    {
        public string BrowserInfo => this.GetBrowserInfo();

        public string ClientIpAddress => this.GetClientIpAddress();

        public string ComputerName => this.GetComputerName();

        public ILogger Logger { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly HttpContext _httpContext;

        /// <summary>
        /// Creates a new <see cref="HttpContextClientInfoProvider"/>.
        /// </summary>
        public HttpContextClientInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._httpContext = httpContextAccessor.HttpContext;

            this.Logger = NullLogger.Instance;
        }

        protected virtual string GetBrowserInfo()
        {
            var httpContext = this._httpContextAccessor.HttpContext ?? this._httpContext;
            return httpContext?.Request?.Headers?["User-Agent"];
        }

        protected virtual string GetClientIpAddress()
        {
            try
            {
                var httpContext = this._httpContextAccessor.HttpContext ?? this._httpContext;
                return httpContext?.Connection?.RemoteIpAddress?.ToString();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex.ToString());
            }

            return null;
        }

        protected virtual string GetComputerName()
        {
            return null; //TODO: Implement!
        }
    }
}
