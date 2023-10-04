using System;
using System.Net;

namespace Notero.Unity.Networking.Common
{
    public interface IClient
    {
        /// <summary>
        /// An amount of time in milliseconds to cancel the connection after Connect () has been called
        /// </summary>
        public abstract int ConnectionTimeout
        {
            get; set;
        }

        public double NetworkTime
        {
            get;
        }

        public ulong Id
        {
            get;
        }

        public bool IsConnected
        {
            get;
        }

        public event Action OnConnected;
        public event Action OnDisconnected;

        public abstract void Connect();
        public abstract void Disconnect();
    }
}
