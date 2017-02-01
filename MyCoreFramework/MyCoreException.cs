using System;
using System.Runtime.Serialization;

namespace MyCoreFramework
{
    /// <summary>
    /// Base exception type for those are thrown by framework system for framework specific exceptions.
    /// </summary>
    [Serializable]
    public class MyCoreException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="MyCoreException"/> object.
        /// </summary>
        public MyCoreException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="MyCoreException"/> object.
        /// </summary>
        public MyCoreException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="MyCoreException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public MyCoreException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="MyCoreException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public MyCoreException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
