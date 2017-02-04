namespace MyCoreFramework.Notifications
{
    internal class NotificationDefinitionContext : INotificationDefinitionContext
    {
        public INotificationDefinitionManager Manager { get; private set; }

        public NotificationDefinitionContext(INotificationDefinitionManager manager)
        {
            this.Manager = manager;
        }
    }
}