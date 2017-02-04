using System;
using System.Runtime.Serialization;

using MyCoreFramework.Logging;

namespace MyCoreFramework.UI
{
    /// <summary>
    /// This exception type is directly shown to the user.
    /// </summary>
    [Serializable]
    public class UserFriendlyException : AbpException, IHasLogSeverity, IHasErrorCode
    {
        /// <summary>
        /// Additional information about the exception.
        /// </summary>
        public string Details { get; private set; }

        /// <summary>
        /// An arbitrary error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Severity of the exception.
        /// Default: Warn.
        /// </summary>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UserFriendlyException()
        {
            this.Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public UserFriendlyException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public UserFriendlyException(string message)
            : base(message)
        {
            this.Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="severity">Exception severity</param>
        public UserFriendlyException(string message, LogSeverity severity)
            : base(message)
        {
            this.Severity = severity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Exception message</param>
        public UserFriendlyException(int code, string message)
            : this(message)
        {
            this.Code = code;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="details">Additional information about the exception</param>
        public UserFriendlyException(string message, string details)
            : this(message)
        {
            this.Details = details;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Exception message</param>
        /// <param name="details">Additional information about the exception</param>
        public UserFriendlyException(int code, string message, string details)
            : this(message, details)
        {
            this.Code = code;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.Severity = LogSeverity.Warn;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="details">Additional information about the exception</param>
        /// <param name="innerException">Inner exception</param>
        public UserFriendlyException(string message, string details, Exception innerException)
            : this(message, innerException)
        {
            this.Details = details;
        }
    }
}