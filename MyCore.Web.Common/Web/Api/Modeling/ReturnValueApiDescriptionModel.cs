using System;

namespace MyCore.Web.Api.Modeling
{
    [Serializable]
    public class ReturnValueApiDescriptionModel
    {
        public Type Type { get; }
        public string TypeAsString { get; }

        public ReturnValueApiDescriptionModel(Type type)
        {
            this.Type = type;
            this.TypeAsString = type.FullName;
        }
    }
}