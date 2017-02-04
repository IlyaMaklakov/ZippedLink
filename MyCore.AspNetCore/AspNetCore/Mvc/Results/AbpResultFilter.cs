using Microsoft.AspNetCore.Mvc.Filters;

using MyCore.AspNetCore.Configuration;
using MyCore.AspNetCore.Mvc.Extensions;
using MyCore.AspNetCore.Mvc.Results.Wrapping;

using MyCoreFramework.Dependency;
using MyCoreFramework.Reflection;

namespace MyCore.AspNetCore.Mvc.Results
{
    public class AbpResultFilter : IResultFilter, ITransientDependency
    {
        private readonly IAbpAspNetCoreConfiguration _configuration;

        public AbpResultFilter(IAbpAspNetCoreConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            if (this._configuration.SetNoCacheForAjaxResponses && context.HttpContext.Request.IsAjaxRequest())
            {
                this.SetNoCache(context);
            }

            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    methodInfo,
                    this._configuration.DefaultWrapResultAttribute
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            AbpActionResultWrapperFactory
                .CreateFor(context)
                .Wrap(context);
        }

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
        
        protected virtual void SetNoCache(ResultExecutingContext context)
        {
            //Based on http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";
        }
    }
}
