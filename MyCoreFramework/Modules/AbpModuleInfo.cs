using System;
using System.Collections.Generic;
using System.Reflection;

using MyCoreFramework.JetBrains.Annotations;

namespace MyCoreFramework.Modules
{
    /// <summary>
    /// Used to store all needed information for a module.
    /// </summary>
    public class AbpModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public AbpModule Instance { get; }

        /// <summary>
        /// Is this module loaded as a plugin.
        /// </summary>
        public bool IsLoadedAsPlugIn { get; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<AbpModuleInfo> Dependencies { get; }

        /// <summary>
        /// Creates a new AbpModuleInfo object.
        /// </summary>
        public AbpModuleInfo([NotNull] Type type, [NotNull] AbpModule instance, bool isLoadedAsPlugIn)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(instance, nameof(instance));

            this.Type = type;
            this.Instance = instance;
            this.IsLoadedAsPlugIn = isLoadedAsPlugIn;
            this.Assembly = this.Type.Assembly;

            this.Dependencies = new List<AbpModuleInfo>();
        }

        public override string ToString()
        {
            return this.Type.AssemblyQualifiedName ??
                   this.Type.FullName;
        }
    }
}