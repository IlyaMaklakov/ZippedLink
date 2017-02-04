using System;
using System.Collections.Generic;

namespace MyCoreFramework.PlugIns
{
    public interface IPlugInSource
    {
        List<Type> GetModules();
    }
}