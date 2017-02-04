using System;
using System.Collections.Generic;
using System.Linq;

using MyCoreFramework;

namespace MyCore.Web.Api.Modeling
{
    [Serializable]
    public class ControllerApiDescriptionModel
    {
        public string Name { get; }

        public IDictionary<string,  ActionApiDescriptionModel> Actions { get; }

        private ControllerApiDescriptionModel()
        {

        }

        public ControllerApiDescriptionModel(string name)
        {
            this.Name = name;

            this.Actions = new Dictionary<string, ActionApiDescriptionModel>();
        }

        public ActionApiDescriptionModel AddAction(ActionApiDescriptionModel action)
        {
            if (this.Actions.ContainsKey(action.Name))
            {
                throw new AbpException(
                    $"Can not add more than one action with same name to the same controller. Controller: {this.Name}, Action: {action.Name}."
                    );
            }

            return this.Actions[action.Name] = action;
        }

        public ControllerApiDescriptionModel CreateSubModel(string[] actions)
        {
            var subModel = new ControllerApiDescriptionModel(this.Name);

            foreach (var action in this.Actions.Values)
            {
                if (actions == null || actions.Contains(action.Name))
                {
                    subModel.AddAction(action);
                }
            }

            return subModel;
        }
    }
}