using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCoreFramework.PlugIns
{
    public class PlugInTypeListSource : IPlugInSource
    {
        private readonly Type[] _moduleTypes;

        public PlugInTypeListSource(params Type[] moduleTypes)
        {
            this._moduleTypes = moduleTypes;
        }

        public List<Type> GetModules()
        {
            return this._moduleTypes.ToList();
        }
    }
}