using System;
using System.Collections.Generic;

namespace MyCore.Web.Api.Modeling
{
    [Serializable]
    public class ActionApiDescriptionModel
    {
        public string Name { get; }

        public string HttpMethod { get; }

        public string Url { get; }

        public IList<ParameterApiDescriptionModel> Parameters { get; }

        public ReturnValueApiDescriptionModel ReturnValue { get; }

        private ActionApiDescriptionModel()
        {

        }

        public ActionApiDescriptionModel(string name, ReturnValueApiDescriptionModel returnValue, string url, string httpMethod = null)
        {
            this.Name = name;
            this.ReturnValue = returnValue;
            this.Url = url;
            this.HttpMethod = httpMethod;

            this.Parameters = new List<ParameterApiDescriptionModel>();
        }

        public ParameterApiDescriptionModel AddParameter(ParameterApiDescriptionModel parameter)
        {
            this.Parameters.Add(parameter);
            return parameter;
        }
    }
}