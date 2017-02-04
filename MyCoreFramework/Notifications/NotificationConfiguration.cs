using MyCoreFramework.Collections;

namespace MyCoreFramework.Notifications
{
    internal class NotificationConfiguration : INotificationConfiguration
    {
        public ITypeList<NotificationProvider> Providers { get; private set; }

        public NotificationConfiguration()
        {
            this.Providers = new TypeList<NotificationProvider>();
        }
    }
}