using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;
using MyCoreFramework.JetBrains.Annotations;

namespace MyCoreFramework.RealTime
{
    /// <summary>
    /// Implements <see cref="IOnlineClientManager"/>.
    /// </summary>
    public class OnlineClientManager : IOnlineClientManager, ISingletonDependency
    {
        public event EventHandler<OnlineClientEventArgs> ClientConnected;
        public event EventHandler<OnlineClientEventArgs> ClientDisconnected;
        public event EventHandler<OnlineUserEventArgs> UserConnected;
        public event EventHandler<OnlineUserEventArgs> UserDisconnected;

        /// <summary>
        /// Online clients.
        /// </summary>
        protected ConcurrentDictionary<string, IOnlineClient> Clients { get; }

        protected readonly object SyncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClientManager"/> class.
        /// </summary>
        public OnlineClientManager()
        {
            this.Clients = new ConcurrentDictionary<string, IOnlineClient>();
        }

        public virtual void Add(IOnlineClient client)
        {
            lock (this.SyncObj)
            {
                var userWasAlreadyOnline = false;
                var user = client.ToUserIdentifierOrNull();

                if (user != null)
                {
                    userWasAlreadyOnline = this.IsOnline(user);
                }

                this.Clients[client.ConnectionId] = client;

                this.ClientConnected.InvokeSafely(this, new OnlineClientEventArgs(client));

                if (user != null && !userWasAlreadyOnline)
                {
                    this.UserConnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
                }
            }
        }

        public virtual bool Remove(string connectionId)
        {
            lock (this.SyncObj)
            {
                IOnlineClient client;
                var isRemoved = this.Clients.TryRemove(connectionId, out client);

                if (isRemoved)
                {
                    var user = client.ToUserIdentifierOrNull();

                    if (user != null && !this.IsOnline(user))
                    {
                        this.UserDisconnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
                    }

                    this.ClientDisconnected.InvokeSafely(this, new OnlineClientEventArgs(client));
                }

                return isRemoved;
            }
        }

        public virtual IOnlineClient GetByConnectionIdOrNull(string connectionId)
        {
            lock (this.SyncObj)
            {
                return this.Clients.GetOrDefault(connectionId);
            }
        }
        
        public virtual IReadOnlyList<IOnlineClient> GetAllClients()
        {
            lock (this.SyncObj)
            {
                return this.Clients.Values.ToImmutableList();
            }
        }

        [NotNull]
        public virtual IReadOnlyList<IOnlineClient> GetAllByUserId([NotNull] IUserIdentifier user)
        {
            Check.NotNull(user, nameof(user));

            return this.GetAllClients()
                 .Where(c => (c.UserId == user.UserId && c.TenantId == user.TenantId))
                 .ToImmutableList();
        }
    }
}