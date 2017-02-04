using System.Collections.Generic;
using System.Reflection;

namespace MyCore.EntityFramework.Utils
{
    internal class EntityDateTimePropertiesInfo
    {
        public List<PropertyInfo> DateTimePropertyInfos { get; set; }

        public List<string> ComplexTypePropertyPaths { get; set; }

        public EntityDateTimePropertiesInfo()
        {
            this.DateTimePropertyInfos = new List<PropertyInfo>();
            this.ComplexTypePropertyPaths = new List<string>();
        }
    }
}