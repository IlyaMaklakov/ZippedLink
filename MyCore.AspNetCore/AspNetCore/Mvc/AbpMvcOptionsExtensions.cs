using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using MyCore.AspNetCore.Mvc.Auditing;
using MyCore.AspNetCore.Mvc.Authorization;
using MyCore.AspNetCore.Mvc.Conventions;
using MyCore.AspNetCore.Mvc.ExceptionHandling;
using MyCore.AspNetCore.Mvc.ModelBinding;
using MyCore.AspNetCore.Mvc.Results;
using MyCore.AspNetCore.Mvc.Uow;
using MyCore.AspNetCore.Mvc.Validation;

namespace MyCore.AspNetCore.Mvc
{
    internal static class AbpMvcOptionsExtensions
    {
        public static void AddAbp(this MvcOptions options, IServiceCollection services)
        {
            AddConventions(options, services);
            AddFilters(options);
            AddModelBinders(options);
        }

        private static void AddConventions(MvcOptions options, IServiceCollection services)
        {
            options.Conventions.Add(new AbpAppServiceConvention(services));
        }

        private static void AddFilters(MvcOptions options)
        {
            options.Filters.AddService(typeof(AbpAuthorizationFilter));
            options.Filters.AddService(typeof(AbpAuditActionFilter));
            options.Filters.AddService(typeof(AbpValidationActionFilter));
            options.Filters.AddService(typeof(AbpUowActionFilter));
            options.Filters.AddService(typeof(AbpExceptionFilter));
            options.Filters.AddService(typeof(AbpResultFilter));
        }

        private static void AddModelBinders(MvcOptions options)
        {
            options.ModelBinderProviders.Add(new AbpDateTimeModelBinderProvider());
        }
    }
}