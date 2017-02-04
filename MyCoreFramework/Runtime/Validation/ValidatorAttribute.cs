using System;

namespace MyCoreFramework.Runtime.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidatorAttribute : Attribute
    {
        public string Name { get; set; }

        public ValidatorAttribute(string name)
        {
            this.Name = name;
        }
    }
}