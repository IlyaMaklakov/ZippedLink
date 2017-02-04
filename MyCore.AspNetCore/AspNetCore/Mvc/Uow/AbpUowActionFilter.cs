using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Filters;

using MyCore.AspNetCore.Configuration;
using MyCore.AspNetCore.Mvc.Extensions;

using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;

namespace MyCore.AspNetCore.Mvc.Uow
{
    public class AbpUowActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpAspNetCoreConfiguration _configuration;

        public AbpUowActionFilter(
            IUnitOfWorkManager unitOfWorkManager,
            IAbpAspNetCoreConfiguration configuration)
        {
            this._unitOfWorkManager = unitOfWorkManager;
            this._configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var unitOfWorkAttr = UnitOfWorkAttribute
                .GetUnitOfWorkAttributeOrNull(context.ActionDescriptor.GetMethodInfo()) ??
                this._configuration.DefaultUnitOfWorkAttribute;

            if (unitOfWorkAttr.IsDisabled)
            {
                await next();
                return;
            }

            using (var uow = this._unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                var result = await next();
                if (result.Exception == null || result.ExceptionHandled)
                {
                    await uow.CompleteAsync();
                }
            }
        }
    }
}
