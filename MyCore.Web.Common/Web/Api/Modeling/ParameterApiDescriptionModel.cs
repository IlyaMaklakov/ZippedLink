using System;

namespace MyCore.Web.Api.Modeling
{
    [Serializable]
    public class ParameterApiDescriptionModel
    {
        public string NameOnMethod { get; }

        public string Name { get; }

        public Type Type { get; }

        public string TypeAsString { get; }

        public bool IsOptional { get;  }

        public object DefaultValue { get;  }

        public string[] ConstraintTypes { get; }

        public string BindingSourceId { get; }

        private ParameterApiDescriptionModel()
        {
            
        }

        public ParameterApiDescriptionModel(string name, string nameOnMethod, Type type, bool isOptional = false, object defaultValue = null, string[] constraintTypes = null, string bindingSourceId = null)
        {
            this.Name = name;
            this.NameOnMethod = nameOnMethod;
            this.Type = type;
            this.TypeAsString = type.FullName;
            this.IsOptional = isOptional;
            this.DefaultValue = defaultValue;
            this.ConstraintTypes = constraintTypes;
            this.BindingSourceId = bindingSourceId;
        }
    }
}