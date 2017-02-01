using System.Collections.Generic;

using MyCoreFramework.MultiTenancy;

namespace MyCoreFramework.Domain.Uow
{
    public class ConnectionStringResolveArgs : Dictionary<string, object>
    {
        public MultiTenancySides? MultiTenancySide { get; set; }

        public ConnectionStringResolveArgs(MultiTenancySides? multiTenancySide = null)
        {
            this.MultiTenancySide = multiTenancySide;
        }
    }
}