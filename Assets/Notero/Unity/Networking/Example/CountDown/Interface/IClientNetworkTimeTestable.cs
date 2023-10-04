using System;
using System.Collections.Generic;
using System.Net;

namespace Notero.Unity.Networking.Example.Countdown.Interface
{
    public interface IClientNetworkTimeTestable
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

        public event Action<ulong, double> OnCountdown;
        public event Action<ulong, string> OnUnnamedMessage;

        public void SendUnnamedMessage(string message, IReadOnlyList<ulong> clientIds = null);

        public void SendUnnamedMessage(string message, ulong clientId);
    }
}
