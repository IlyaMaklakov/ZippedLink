using System;
using System.Collections.Generic;
using System.Reflection;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Modules;

namespace MyCoreFramework.PlugIns
{
    public class AssemblyFileListPlugInSource : IPlugInSource
    {
        public string[] FilePaths { get; }

        public AssemblyFileListPlugInSource(params string[] filePaths)
        {
            this.FilePaths = filePaths ?? new string[0];
        }

        public List<Type> GetModules()
        {
            var modules = new List<Type>();

            foreach (var filePath in this.FilePaths)
            {
                var assembly = Assembly.LoadFile(filePath);

                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (AbpModule.IsAbpModule(type))
                        {
                            modules.AddIfNotContains(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new AbpInitializationException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules;
        }
    }
}