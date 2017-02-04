using System;
using System.Collections.Generic;

namespace MyCoreFramework.Auditing
{
    internal class AuditingConfiguration : IAuditingConfiguration
    {
        public bool IsEnabled { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public IAuditingSelectorList Selectors { get; }

        public List<Type> IgnoredTypes { get; }

        public AuditingConfiguration()
        {
            this.IsEnabled = true;
            this.Selectors = new AuditingSelectorList();
            this.IgnoredTypes = new List<Type>();
        }
    }
}