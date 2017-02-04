using System;

using MyCoreFramework.Localization;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class LocalizableMessageNotificationData : NotificationData
    {
        /// <summary>
        /// The message.
        /// </summary>
        public LocalizableString Message
        {
            get
            {
                return this._message ?? (this[nameof(this.Message)] as LocalizableString);
            }
            set
            {
                this[nameof(this.Message)] = value;
                this._message = value;
            }
        }

        private LocalizableString _message;

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private LocalizableMessageNotificationData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizableMessageNotificationData"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LocalizableMessageNotificationData(LocalizableString message)
        {
            this.Message = message;
        }
    }
}