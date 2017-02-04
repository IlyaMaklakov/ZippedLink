using System.Net;

using Castle.Core.Logging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using MyCore.AspNetCore.Configuration;
using MyCore.AspNetCore.Mvc.Extensions;
using MyCore.AspNetCore.Mvc.Results;
using MyCore.Web.Models;

using MyCoreFramework.Authorization;
using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Entities;
using MyCoreFramework.Events.Bus;
using MyCoreFramework.Events.Bus.Exceptions;
using MyCoreFramework.Logging;
using MyCoreFramework.Reflection;

namespace MyCore.AspNetCore.Mvc.ExceptionHandling
{
    public class AbpExceptionFilter : IExceptionFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly IAbpAspNetCoreConfiguration _configuration;

        public AbpExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration)
        {
            this._errorInfoBuilder = errorInfoBuilder;
            this._configuration = configuration;

            this.Logger = NullLogger.Instance;
            this.EventBus = NullEventBus.Instance;
        }

        public void OnException(ExceptionContext context)
        {
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    context.ActionDescriptor.GetMethodInfo(),
                    this._configuration.DefaultWrapResultAttribute
                );

            if (wrapResultAttribute.LogError)
            {
                LogHelper.LogException(this.Logger, context.Exception);
            }

            if (wrapResultAttribute.WrapOnError)
            {
                this.HandleAndWrapException(context);
            }
        }

        private void HandleAndWrapException(ExceptionContext context)
        {
            if (!ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = this.GetStatusCode(context);

            context.Result = new ObjectResult(
                new AjaxResponse(
                    this._errorInfoBuilder.BuildForException(context.Exception),
                    context.Exception is AbpAuthorizationException
                )
            );

            this.EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));

            context.Exception = null; //Handled!
        }

        private int GetStatusCode(ExceptionContext context)
        {
            if (context.Exception is AbpAuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }

            return (int)HttpStatusCode.InternalServerError;
        }
    }
}
