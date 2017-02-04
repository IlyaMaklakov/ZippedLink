using System;
using System.Linq;
using System.Threading.Tasks;

using Castle.Core.Logging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

using MyCore.AspNetCore.Mvc.Extensions;
using MyCore.AspNetCore.Mvc.Results;
using MyCore.Web.Models;

using MyCoreFramework.Authorization;
using MyCoreFramework.Dependency;
using MyCoreFramework.Events.Bus;
using MyCoreFramework.Events.Bus.Exceptions;

namespace MyCore.AspNetCore.Mvc.Authorization
{
    public class AbpAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly IEventBus _eventBus;

        public AbpAuthorizationFilter(
            IAuthorizationHelper authorizationHelper,
            IErrorInfoBuilder errorInfoBuilder,
            IEventBus eventBus)
        {
            this._authorizationHelper = authorizationHelper;
            this._errorInfoBuilder = errorInfoBuilder;
            this._eventBus = eventBus;
            this.Logger = NullLogger.Instance;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            try
            {
                //TODO: Avoid using try/catch, use conditional checking
                await this._authorizationHelper.AuthorizeAsync(context.ActionDescriptor.GetMethodInfo());
            }
            catch (AbpAuthorizationException ex)
            {
                this.Logger.Warn(ex.ToString(), ex);

                this._eventBus.Trigger(this, new AbpHandledExceptionData(ex));

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new AjaxResponse(this._errorInfoBuilder.BuildForException(ex), true))
                    {
                        StatusCode = context.HttpContext.User.Identity.IsAuthenticated
                            ? (int) System.Net.HttpStatusCode.Forbidden
                            : (int) System.Net.HttpStatusCode.Unauthorized
                    };
                }
                else
                {
                    context.Result = new ChallengeResult();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex.ToString(), ex);

                this._eventBus.Trigger(this, new AbpHandledExceptionData(ex));

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new AjaxResponse(this._errorInfoBuilder.BuildForException(ex)))
                    {
                        StatusCode = (int) System.Net.HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    //TODO: How to return Error page?
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}