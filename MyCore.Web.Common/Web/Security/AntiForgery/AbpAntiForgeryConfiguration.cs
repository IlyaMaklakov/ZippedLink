namespace MyCore.Web.Security.AntiForgery
{
    public class AbpAntiForgeryConfiguration : IAbpAntiForgeryConfiguration
    {
        public string TokenCookieName { get; set; }

        public string TokenHeaderName { get; set; }

        public AbpAntiForgeryConfiguration()
        {
            this.TokenCookieName = "XSRF-TOKEN";
            this.TokenHeaderName = "X-XSRF-TOKEN";
        }
    }
}