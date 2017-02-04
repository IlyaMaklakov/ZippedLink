using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

using MyCore.AspNetCore.Mvc.Extensions;
using MyCore.Web.Models;

using MyCoreFramework;
using MyCoreFramework.Auditing;
using MyCoreFramework.Configuration;
using MyCoreFramework.Localization;
using MyCoreFramework.Runtime.Session;
using MyCoreFramework.Timing;

namespace MyCore.AspNetCore.Mvc.Controllers
{
    public class AbpLocalizationController : AbpController
    {
        [DisableAuditing]
        public virtual ActionResult ChangeCulture(string cultureName, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(cultureName))
            {
                throw new AbpException("Unknown language: " + cultureName + ". It must be a valid culture!");
            }

            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName, cultureName));

            this.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                new CookieOptions {Expires = Clock.Now.AddYears(2)}
            );

            if (this.AbpSession.UserId.HasValue)
            {
                this.SettingManager.ChangeSettingForUser(
                    this.AbpSession.ToUserIdentifier(),
                    LocalizationSettingNames.DefaultLanguage,
                    cultureName
                );
            }

            if (this.Request.IsAjaxRequest())
            {
                return this.Json(new AjaxResponse());
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.Redirect("/"); //TODO: Go to app root
        }
    }
}
