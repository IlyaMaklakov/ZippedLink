using System;

namespace MyCoreFramework.RealTime
{
    public class OnlineClientEventArgs : EventArgs
    {
        public IOnlineClient Client { get; }

        public OnlineClientEventArgs(IOnlineClient client)
        {
            this.Client = client;
        }
    }
}