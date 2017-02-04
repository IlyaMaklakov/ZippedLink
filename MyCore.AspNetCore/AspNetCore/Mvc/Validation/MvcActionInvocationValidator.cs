using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Filters;

using MyCore.AspNetCore.Mvc.Extensions;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Runtime.Validation.Interception;

namespace MyCore.AspNetCore.Mvc.Validation
{
    public class MvcActionInvocationValidator : MethodInvocationValidator
    {
        protected ActionExecutingContext ActionContext { get; private set; }

        private bool _isValidatedBefore;

        public MvcActionInvocationValidator(IValidationConfiguration configuration, IIocResolver iocResolver)
            : base(configuration, iocResolver)
        {

        }

        public void Initialize(ActionExecutingContext actionContext)
        {
            base.Initialize(
                actionContext.ActionDescriptor.GetMethodInfo(),
                this.GetParameterValues(actionContext)
            );

            this.ActionContext = actionContext;
        }

        protected override void SetDataAnnotationAttributeErrors(object validatingObject)
        {
            if (this._isValidatedBefore || this.ActionContext.ModelState.IsValid)
            {
                return;
            }

            foreach (var state in this.ActionContext.ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    this.ValidationErrors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
                }
            }

            this._isValidatedBefore = true;
        }

        protected virtual object[] GetParameterValues(ActionExecutingContext actionContext)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfo();

            var parameters = methodInfo.GetParameters();
            var parameterValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = actionContext.ActionArguments.GetOrDefault(parameters[i].Name);
            }

            return parameterValues;
        }
    }
}