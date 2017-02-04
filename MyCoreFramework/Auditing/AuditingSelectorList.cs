using System.Collections.Generic;

namespace MyCoreFramework.Auditing
{
    internal class AuditingSelectorList : List<NamedTypeSelector>, IAuditingSelectorList
    {
        public bool RemoveByName(string name)
        {
            return this.RemoveAll(s => s.Name == name) > 0;
        }
    }
}