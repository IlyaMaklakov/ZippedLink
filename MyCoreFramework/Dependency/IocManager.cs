using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace MyCoreFramework.Dependency
{
    /// <summary>
    /// This class is used to directly perform dependency injection tasks.
    /// </summary>
    public class IocManager : IIocManager
    {
        /// <summary>
        /// The Singleton instance.
        /// </summary>
        public static IocManager Instance { get; private set; }

        /// <summary>
        /// Reference to the Castle Windsor Container.
        /// </summary>
        public IWindsorContainer IocContainer { get; private set; }

        /// <summary>
        /// List of all registered conventional registrars.
        /// </summary>
        private readonly List<IConventionalDependencyRegistrar> _conventionalRegistrars;

        static IocManager()
        {
            Instance = new IocManager();
        }

        /// <summary>
        /// Creates a new <see cref="IocManager"/> object.
        /// Normally, you don't directly instantiate an <see cref="IocManager"/>.
        /// This may be useful for test purposes.
        /// </summary>
        public IocManager()
        {
            this.IocContainer = new WindsorContainer();
            this._conventionalRegistrars = new List<IConventionalDependencyRegistrar>();

            //Register self!
            this.IocContainer.Register(
                Component.For<IocManager, IIocManager, IIocRegistrar, IIocResolver>().UsingFactoryMethod(() => this)
                );
        }

        /// <summary>
        /// Adds a dependency registrar for conventional registration.
        /// </summary>
        /// <param name="registrar">dependency registrar</param>
        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar)
        {
            this._conventionalRegistrars.Add(registrar);
        }

        /// <summary>
        /// Registers types of given assembly by all conventional registrars. See <see cref="AddConventionalRegistrar"/> method.
        /// </summary>
        /// <param name="assembly">Assembly to register</param>
        public void RegisterAssemblyByConvention(Assembly assembly)
        {
            this.RegisterAssemblyByConvention(assembly, new ConventionalRegistrationConfig());
        }

        /// <summary>
        /// Registers types of given assembly by all conventional registrars. See <see cref="AddConventionalRegistrar"/> method.
        /// </summary>
        /// <param name="assembly">Assembly to register</param>
        /// <param name="config">Additional configuration</param>
        public void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config)
        {
            var context = new ConventionalRegistrationContext(assembly, this, config);

            foreach (var registerer in this._conventionalRegistrars)
            {
                registerer.RegisterAssembly(context);
            }

            if (config.InstallInstallers)
            {
                this.IocContainer.Install(FromAssembly.Instance(assembly));
            }
        }

        /// <summary>
        /// Registers a type as self registration.
        /// </summary>
        /// <typeparam name="TType">Type of the class</typeparam>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register<TType>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class
        {
            this.IocContainer.Register(ApplyLifestyle(Component.For<TType>(), lifeStyle));
        }

        /// <summary>
        /// Registers a type as self registration.
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            this.IocContainer.Register(ApplyLifestyle(Component.For(type), lifeStyle));
        }

        /// <summary>
        /// Registers a type with it's implementation.
        /// </summary>
        /// <typeparam name="TType">Registering type</typeparam>
        /// <typeparam name="TImpl">The type that implements <see cref="TType"/></typeparam>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register<TType, TImpl>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            this.IocContainer.Register(ApplyLifestyle(Component.For<TType, TImpl>().ImplementedBy<TImpl>(), lifeStyle));
        }

        /// <summary>
        /// Registers a type with it's implementation.
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="impl">The type that implements <paramref name="type"/></param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            this.IocContainer.Register(ApplyLifestyle(Component.For(type, impl).ImplementedBy(impl), lifeStyle));
        }

        /// <summary>
        /// Checks whether given type is registered before.
        /// </summary>
        /// <param name="type">Type to check</param>
        public bool IsRegistered(Type type)
        {
            return this.IocContainer.Kernel.HasComponent(type);
        }

        /// <summary>
        /// Checks whether given type is registered before.
        /// </summary>
        /// <typeparam name="TType">Type to check</typeparam>
        public bool IsRegistered<TType>()
        {
            return this.IocContainer.Kernel.HasComponent(typeof(TType));
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IIocResolver.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The instance object</returns>
        public T Resolve<T>()
        {
            return this.IocContainer.Resolve<T>();
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to cast</typeparam>
        /// <param name="type">Type of the object to resolve</param>
        /// <returns>The object instance</returns>
        public T Resolve<T>(Type type)
        {
            return (T)this.IocContainer.Resolve(type);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IIocResolver.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public T Resolve<T>(object argumentsAsAnonymousType)
        {
            return this.IocContainer.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IIocResolver.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <returns>The instance object</returns>
        public object Resolve(Type type)
        {
            return this.IocContainer.Resolve(type);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IIocResolver.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return this.IocContainer.Resolve(type, argumentsAsAnonymousType);
        }

        ///<inheritdoc/>
        public T[] ResolveAll<T>()
        {
            return this.IocContainer.ResolveAll<T>();
        }

        ///<inheritdoc/>
        public T[] ResolveAll<T>(object argumentsAsAnonymousType)
        {
            return this.IocContainer.ResolveAll<T>(argumentsAsAnonymousType);
        }

        ///<inheritdoc/>
        public object[] ResolveAll(Type type)
        {
            return this.IocContainer.ResolveAll(type).Cast<object>().ToArray();
        }

        ///<inheritdoc/>
        public object[] ResolveAll(Type type, object argumentsAsAnonymousType)
        {
            return this.IocContainer.ResolveAll(type, argumentsAsAnonymousType).Cast<object>().ToArray();
        }

        /// <summary>
        /// Releases a pre-resolved object. See Resolve methods.
        /// </summary>
        /// <param name="obj">Object to be released</param>
        public void Release(object obj)
        {
            this.IocContainer.Release(obj);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.IocContainer.Dispose();
        }

        private static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration, DependencyLifeStyle lifeStyle)
            where T : class
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Transient:
                    return registration.LifestyleTransient();
                case DependencyLifeStyle.Singleton:
                    return registration.LifestyleSingleton();
                default:
                    return registration;
            }
        }
    }
}