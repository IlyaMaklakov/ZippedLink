using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MyCoreFramework.Modules;

namespace MyCoreFramework.Reflection
{
    public class AbpAssemblyFinder : IAssemblyFinder
    {
        private readonly IAbpModuleManager _moduleManager;

        public AbpAssemblyFinder(IAbpModuleManager moduleManager)
        {
            this._moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in this._moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            return assemblies.Distinct().ToList();
        }
    }
}