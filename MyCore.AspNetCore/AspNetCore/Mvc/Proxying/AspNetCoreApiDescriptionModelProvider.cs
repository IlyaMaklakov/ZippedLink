using System.Linq;
using System.Reflection;

using Castle.Core.Logging;

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using MyCore.AspNetCore.Configuration;
using MyCore.AspNetCore.Mvc.Extensions;
using MyCore.AspNetCore.Mvc.Proxying.Utils;
using MyCore.Web.Api.Modeling;

using MyCoreFramework.Application.Services;
using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;

namespace MyCore.AspNetCore.Mvc.Proxying
{
    public class AspNetCoreApiDescriptionModelProvider : IApiDescriptionModelProvider, ISingletonDependency
    {
        public ILogger Logger { get; set; }

        private readonly IApiDescriptionGroupCollectionProvider _descriptionProvider;
        private readonly AbpAspNetCoreConfiguration _configuration;

        public AspNetCoreApiDescriptionModelProvider(
            IApiDescriptionGroupCollectionProvider descriptionProvider,
            AbpAspNetCoreConfiguration configuration)
        {
            this._descriptionProvider = descriptionProvider;
            this._configuration = configuration;

            this.Logger = NullLogger.Instance;
        }

        public ApplicationApiDescriptionModel CreateModel()
        {
            var model = new ApplicationApiDescriptionModel();

            foreach (var descriptionGroupItem in this._descriptionProvider.ApiDescriptionGroups.Items)
            {
                foreach (var apiDescription in descriptionGroupItem.Items)
                {
                    this.AddApiDescriptionToModel(apiDescription, model);
                }
            }

            return model;
        }

        private void AddApiDescriptionToModel(ApiDescription apiDescription, ApplicationApiDescriptionModel model)
        {
            var moduleModel = model.GetOrAddModule(this.GetModuleName(apiDescription));
            var controllerModel = moduleModel.GetOrAddController(apiDescription.GroupName.RemovePostFix(ApplicationService.CommonPostfixes));
            var method = apiDescription.ActionDescriptor.GetMethodInfo();

            if (controllerModel.Actions.ContainsKey(method.Name))
            {
                this.Logger.Warn($"Controller '{controllerModel.Name}' contains more than one action with name '{method.Name}' for module '{moduleModel.Name}'. Ignored: " + apiDescription.ActionDescriptor.GetMethodInfo());
                return;
            }

            var returnValue = new ReturnValueApiDescriptionModel(method.ReturnType);

            var actionModel = controllerModel.AddAction(new ActionApiDescriptionModel(
                method.Name,
                returnValue,
                apiDescription.RelativePath,
                apiDescription.HttpMethod
            ));

            this.AddParameterDescriptionsToModel(actionModel, method, apiDescription);
        }

        private void AddParameterDescriptionsToModel(ActionApiDescriptionModel actionModel, MethodInfo method, ApiDescription apiDescription)
        {
            if (!apiDescription.ParameterDescriptions.Any())
            {
                return;
            }

            var matchedMethodParamNames = ArrayMatcher.Match(
                apiDescription.ParameterDescriptions.Select(p => p.Name).ToArray(),
                method.GetParameters().Select(this.GetMethodParamName).ToArray()
            );

            for (var i = 0; i < apiDescription.ParameterDescriptions.Count; i++)
            {
                var parameterDescription = apiDescription.ParameterDescriptions[i];
                var matchedMethodParamName = matchedMethodParamNames.Length > i
                                                 ? matchedMethodParamNames[i]
                                                 : parameterDescription.Name;

                actionModel.AddParameter(new ParameterApiDescriptionModel(
                        parameterDescription.Name,
                        matchedMethodParamName,
                        parameterDescription.Type,
                        parameterDescription.RouteInfo?.IsOptional ?? false,
                        parameterDescription.RouteInfo?.DefaultValue,
                        parameterDescription.RouteInfo?.Constraints?.Select(c => c.GetType().Name).ToArray(),
                        parameterDescription.Source.Id
                    )
                );
            }
        }

        public string GetMethodParamName(ParameterInfo parameterInfo)
        {
            var modelNameProvider = parameterInfo.GetCustomAttributes()
                .OfType<IModelNameProvider>()
                .FirstOrDefault();

            if (modelNameProvider == null)
            {
                return parameterInfo.Name;
            }

            return modelNameProvider.Name;
        }

        private string GetModuleName(ApiDescription apiDescription)
        {
            var controllerType = apiDescription.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType();
            if (controllerType == null)
            {
                return AbpControllerAssemblySetting.DefaultServiceModuleName;
            }

            foreach (var controllerSetting in this._configuration.ControllerAssemblySettings)
            {
                if (controllerType.Assembly == controllerSetting.Assembly)
                {
                    return controllerSetting.ModuleName;
                }
            }

            return AbpControllerAssemblySetting.DefaultServiceModuleName;
        }
    }
}
