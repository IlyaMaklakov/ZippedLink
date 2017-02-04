using System.Collections.Concurrent;

using MyCore.Web.Api.Modeling;
using MyCore.Web.Api.ProxyScripting.Configuration;
using MyCore.Web.Api.ProxyScripting.Generators;

using MyCoreFramework;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;
using MyCoreFramework.Json;

namespace MyCore.Web.Api.ProxyScripting
{
    public class ApiProxyScriptManager : IApiProxyScriptManager, ISingletonDependency
    {
        private readonly IApiDescriptionModelProvider _modelProvider;
        private readonly IApiProxyScriptingConfiguration _configuration;
        private readonly IIocResolver _iocResolver;

        private readonly ConcurrentDictionary<string, string> _cache;

        public ApiProxyScriptManager(
            IApiDescriptionModelProvider modelProvider, 
            IApiProxyScriptingConfiguration configuration,
            IIocResolver iocResolver)
        {
            this._modelProvider = modelProvider;
            this._configuration = configuration;
            this._iocResolver = iocResolver;

            this._cache = new ConcurrentDictionary<string, string>();
        }

        public string GetScript(ApiProxyGenerationOptions options)
        {
            if (options.UseCache)
            {
                return this._cache.GetOrAdd(CreateCacheKey(options), (key) => this.CreateScript(options));
            }

            return this._cache[CreateCacheKey(options)] = this.CreateScript(options);
        }

        private string CreateScript(ApiProxyGenerationOptions options)
        {
            var model = this._modelProvider.CreateModel();

            if (options.IsPartialRequest())
            {
                model = model.CreateSubModel(options.Modules, options.Controllers, options.Actions);
            }

            var generatorType = this._configuration.Generators.GetOrDefault(options.GeneratorType);
            if (generatorType == null)
            {
                throw new AbpException($"Could not find a proxy script generator with given name: {options.GeneratorType}");
            }

            using (var generator = this._iocResolver.ResolveAsDisposable<IProxyScriptGenerator>(generatorType))
            {
                return generator.Object.CreateScript(model);
            }
        }

        private static string CreateCacheKey(ApiProxyGenerationOptions options)
        {
            return options.ToJsonString().ToMd5();
        }
    }
}
