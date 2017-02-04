using System;
using System.Collections.Generic;
using System.Linq;

using MyCoreFramework;
using MyCoreFramework.Collections.Extensions;

namespace MyCore.Web.Api.Modeling
{
    [Serializable]
    public class ModuleApiDescriptionModel
    {
        public string Name { get; set; }

        public IDictionary<string, ControllerApiDescriptionModel> Controllers { get; }

        private ModuleApiDescriptionModel()
        {
            
        }

        public ModuleApiDescriptionModel(string name)
        {
            this.Name = name;

            this.Controllers = new Dictionary<string, ControllerApiDescriptionModel>();
        }

        public ControllerApiDescriptionModel AddController(ControllerApiDescriptionModel controller)
        {
            if (this.Controllers.ContainsKey(controller.Name))
            {
                throw new AbpException($"There is already a controller with name: {controller.Name} in module: {this.Name}");
            }

            return this.Controllers[controller.Name] = controller;
        }

        public ControllerApiDescriptionModel GetOrAddController(string name)
        {
            return this.Controllers.GetOrAdd(name, () => new ControllerApiDescriptionModel(name));
        }
        
        public ModuleApiDescriptionModel CreateSubModel(string[] controllers, string[] actions)
        {
            var subModel = new ModuleApiDescriptionModel(this.Name);

            foreach (var controller in this.Controllers.Values)
            {
                if (controllers == null || controllers.Contains(controller.Name))
                {
                    subModel.AddController(controller.CreateSubModel(actions));
                }
            }

            return subModel;
        }
    }
}