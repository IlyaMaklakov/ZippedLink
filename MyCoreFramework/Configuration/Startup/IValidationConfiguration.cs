using System;
using System.Collections.Generic;

namespace MyCoreFramework.Configuration.Startup
{
    public interface IValidationConfiguration
    {
        List<Type> IgnoredTypes { get; }
    }
}