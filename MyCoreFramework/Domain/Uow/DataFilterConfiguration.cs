using System.Collections.Generic;

namespace MyCoreFramework.Domain.Uow
{
    public class DataFilterConfiguration
    {
        public string FilterName { get; }

        public bool IsEnabled { get; }

        public IDictionary<string, object> FilterParameters { get; }

        public DataFilterConfiguration(string filterName, bool isEnabled)
        {
            this.FilterName = filterName;
            this.IsEnabled = isEnabled;
            this.FilterParameters = new Dictionary<string, object>();
        }

        internal DataFilterConfiguration(DataFilterConfiguration filterToClone, bool? isEnabled = null)
            : this(filterToClone.FilterName, isEnabled ?? filterToClone.IsEnabled)
        {
            foreach (var filterParameter in filterToClone.FilterParameters)
            {
                this.FilterParameters[filterParameter.Key] = filterParameter.Value;
            }
        }
    }
}