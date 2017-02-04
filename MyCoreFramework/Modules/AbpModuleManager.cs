using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Castle.Core.Logging;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.PlugIns;

namespace MyCoreFramework.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class AbpModuleManager : IAbpModuleManager
    {
        public AbpModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<AbpModuleInfo> Modules => this._modules.ToImmutableList();

        public ILogger Logger { get; set; }

        private AbpModuleCollection _modules;

        private readonly IIocManager _iocManager;
        private readonly IAbpPlugInManager _abpPlugInManager;

        public AbpModuleManager(IIocManager iocManager, IAbpPlugInManager abpPlugInManager)
        {
            this._iocManager = iocManager;
            this._abpPlugInManager = abpPlugInManager;

            this.Logger = NullLogger.Instance;
        }

        public virtual void Initialize(Type startupModule)
        {
            this._modules = new AbpModuleCollection(startupModule);
            this.LoadAllModules();
        }

        public virtual void StartModules()
        {
            var sortedModules = this._modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        public virtual void ShutdownModules()
        {
            this.Logger.Debug("Shutting down has been started");

            var sortedModules = this._modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            this.Logger.Debug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            this.Logger.Debug("Loading Abp modules...");

            List<Type> plugInModuleTypes;
            var moduleTypes = this.FindAllModuleTypes(out plugInModuleTypes).Distinct().ToList();

            this.Logger.Debug("Found " + moduleTypes.Count + " ABP modules in total.");

            this.RegisterModules(moduleTypes);
            this.CreateModules(moduleTypes, plugInModuleTypes);

            this._modules.EnsureKernelModuleToBeFirst();
            this._modules.EnsureStartupModuleToBeLast();

            this.SetDependencies();

            this.Logger.DebugFormat("{0} modules loaded.", this._modules.Count);
        }

        private List<Type> FindAllModuleTypes(out List<Type> plugInModuleTypes)
        {
            plugInModuleTypes = new List<Type>();

            var modules = AbpModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(this._modules.StartupModuleType);
            
            foreach (var plugInModuleType in this._abpPlugInManager.PlugInSources.GetAllModules())
            {
                if (modules.AddIfNotContains(plugInModuleType))
                {
                    plugInModuleTypes.Add(plugInModuleType);
                }
            }

            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes, List<Type> plugInModuleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = this._iocManager.Resolve(moduleType) as AbpModule;
                if (moduleObject == null)
                {
                    throw new AbpInitializationException("This type is not an ABP module: " + moduleType.AssemblyQualifiedName);
                }

                moduleObject.IocManager = this._iocManager;
                moduleObject.Configuration = this._iocManager.Resolve<IAbpStartupConfiguration>();

                var moduleInfo = new AbpModuleInfo(moduleType, moduleObject, plugInModuleTypes.Contains(moduleType));

                this._modules.Add(moduleInfo);

                if (moduleType == this._modules.StartupModuleType)
                {
                    this.StartupModule = moduleInfo;
                }

                this.Logger.DebugFormat("Loaded module: " + moduleType.AssemblyQualifiedName);
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                this._iocManager.RegisterIfNot(moduleType);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in this._modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in AbpModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = this._modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new AbpInitializationException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
