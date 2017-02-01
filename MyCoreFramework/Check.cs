using System;
using System.Diagnostics;

using MyCoreFramework.JetBrains.Annotations;

namespace MyCoreFramework
{
    [DebuggerStepThrough]
    internal static class Check
    {
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(T value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}
