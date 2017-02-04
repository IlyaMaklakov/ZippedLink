using System;
using System.Collections.Generic;

namespace MyCoreFramework.Configuration.Startup
{
    public class ValidationConfiguration : IValidationConfiguration
    {
        public List<Type> IgnoredTypes { get; }

        public ValidationConfiguration()
        {
            this.IgnoredTypes = new List<Type>();
        }
    }
}