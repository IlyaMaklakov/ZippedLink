using System;
using System.Reflection;

using MyCoreFramework.Reflection.Extensions;

namespace MyCoreFramework.Application.Services
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class RemoteServiceAttribute : Attribute
    {
        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsMetadataEnabled { get; set; }

        public RemoteServiceAttribute(bool isEnabled = true)
        {
            this.IsEnabled = isEnabled;
            this.IsMetadataEnabled = true;
        }

        public virtual bool IsEnabledFor(Type type)
        {
            return this.IsEnabled;
        }

        public virtual bool IsEnabledFor(MethodInfo method)
        {
            return this.IsEnabled;
        }

        public virtual bool IsMetadataEnabledFor(Type type)
        {
            return this.IsMetadataEnabled;
        }

        public virtual bool IsMetadataEnabledFor(MethodInfo method)
        {
            return this.IsMetadataEnabled;
        }

        public static bool IsExplicitlyEnabledFor(Type type)
        {
            var remoteServiceAttr = type.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && remoteServiceAttr.IsEnabledFor(type);
        }

        public static bool IsExplicitlyDisabledFor(Type type)
        {
            var remoteServiceAttr = type.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(type);
        }

        public static bool IsMetadataExplicitlyEnabledFor(Type type)
        {
            var remoteServiceAttr = type.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && remoteServiceAttr.IsMetadataEnabledFor(type);
        }

        public static bool IsMetadataExplicitlyDisabledFor(Type type)
        {
            var remoteServiceAttr = type.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && !remoteServiceAttr.IsMetadataEnabledFor(type);
        }

        public static bool IsMetadataExplicitlyDisabledFor(MethodInfo method)
        {
            var remoteServiceAttr = method.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && !remoteServiceAttr.IsMetadataEnabledFor(method);
        }

        public static bool IsMetadataExplicitlyEnabledFor(MethodInfo method)
        {
            var remoteServiceAttr = method.GetSingleAttributeOrNull<RemoteServiceAttribute>();
            return remoteServiceAttr != null && remoteServiceAttr.IsMetadataEnabledFor(method);
        }
    }
}