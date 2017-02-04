using System;
using System.Collections.Generic;

using MyCoreFramework.Json;
using MyCoreFramework.Timing;

namespace MyCoreFramework.RealTime
{
    /// <summary>
    /// Implements <see cref="IOnlineClient"/>.
    /// </summary>
    [Serializable]
    public class OnlineClient : IOnlineClient
    {
        /// <summary>
        /// Unique connection Id for this client.
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// IP address of this client.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Tenant Id.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Connection establishment time for this client.
        /// </summary>
        public DateTime ConnectTime { get; set; }

        /// <summary>
        /// Shortcut to set/get <see cref="Properties"/>.
        /// </summary>
        public object this[string key]
        {
            get { return this.Properties[key]; }
            set { this.Properties[key] = value; }
        }

        /// <summary>
        /// Can be used to add custom properties for this client.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get { return this._properties; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this._properties = value;
            }
        }
        private Dictionary<string, object> _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClient"/> class.
        /// </summary>
        public OnlineClient()
        {
            this.ConnectTime = Clock.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClient"/> class.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public OnlineClient(string connectionId, string ipAddress, int? tenantId, long? userId)
            : this()
        {
            this.ConnectionId = connectionId;
            this.IpAddress = ipAddress;
            this.TenantId = tenantId;
            this.UserId = userId;

            this.Properties = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            return this.ToJsonString();
        }
    }
}