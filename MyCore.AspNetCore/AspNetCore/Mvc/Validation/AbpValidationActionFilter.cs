using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Filters;

using MyCore.AspNetCore.Configuration;

using MyCoreFramework.Aspects;
using MyCoreFramework.Dependency;

namespace MyCore.AspNetCore.Mvc.Validation
{
    public class AbpValidationActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IAbpAspNetCoreConfiguration _configuration;

        public AbpValidationActionFilter(IIocResolver iocResolver, IAbpAspNetCoreConfiguration configuration)
        {
            this._iocResolver = iocResolver;
            this._configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!this._configuration.IsValidationEnabledForControllers)
            {
                await next();
                return;
            }

            using (AbpCrossCuttingConcerns.Applying(context.Controller, AbpCrossCuttingConcerns.Validation))
            {
                using (var validator = this._iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
                {
                    validator.Object.Initialize(context);
                    validator.Object.Validate();
                }

                await next();
            }
        }
    }
}
