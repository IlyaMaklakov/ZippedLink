using System;

namespace MyCoreFramework.Notifications
{
    /// <summary>
    /// Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class MessageNotificationData : NotificationData
    {
        /// <summary>
        /// The message.
        /// </summary>
        public string Message
        {
            get { return this._message ?? (this[nameof(this.Message)] as string); }
            set
            {
                this[nameof(this.Message)] = value;
                this._message = value;
            }
        }
        private string _message;

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private MessageNotificationData()
        {
            
        }

        public MessageNotificationData(string message)
        {
            this.Message = message;
        }
    }
}