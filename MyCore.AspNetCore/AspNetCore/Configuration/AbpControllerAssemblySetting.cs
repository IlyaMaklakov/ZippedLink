using System;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace MyCore.AspNetCore.Configuration
{
    public class AbpControllerAssemblySetting
    {
        /// <summary>
        /// "app".
        /// </summary>
        public const string DefaultServiceModuleName = "app";

        public string ModuleName { get; }

        public Assembly Assembly { get; }

        public bool UseConventionalHttpVerbs { get; }

        public Func<Type, bool> TypePredicate { get; set; }

        public Action<ControllerModel> ControllerModelConfigurer { get; set; }

        public AbpControllerAssemblySetting(string moduleName, Assembly assembly, bool useConventionalHttpVerbs)
        {
            this.ModuleName = moduleName;
            this.Assembly = assembly;
            this.UseConventionalHttpVerbs = useConventionalHttpVerbs;

            this.TypePredicate = type => true;
            this.ControllerModelConfigurer = controller => { };
        }
    }
}