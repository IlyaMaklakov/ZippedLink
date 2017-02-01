using System.Linq;
using System.Reflection;

using Castle.Core;
using Castle.MicroKernel;

using MyCoreFramework.Dependency;

namespace MyCoreFramework.Domain.Uow
{
    /// <summary>
    /// This class is used to register interceptor for needed classes for Unit Of Work mechanism.
    /// </summary>
    internal static class UnitOfWorkRegistrar
    {
        /// <summary>
        /// Initializes the registerer.
        /// </summary>
        /// <param name="iocManager">IOC manager</param>
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += ComponentRegistered;
        }

        private static void ComponentRegistered(string key, IHandler handler)
        {
            if (UnitOfWorkHelper.IsConventionalUowClass(handler.ComponentModel.Implementation))
            {
                //Intercept all methods of all repositories.
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
            else if (handler.ComponentModel.Implementation.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(UnitOfWorkHelper.HasUnitOfWorkAttribute))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
            }
        }
    }
}