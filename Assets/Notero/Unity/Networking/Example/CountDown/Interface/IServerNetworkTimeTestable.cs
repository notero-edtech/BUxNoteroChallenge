using System;
using System.Collections.Generic;

namespace Notero.Unity.Networking.Example.Countdown.Interface
{
    public interface IServerNetworkTimeTestable
    {
        public bool IsStarted
        {
            get;
        }

        public double NetworkTime
        {
            get;
        }

        public ulong Id
        {
            get;
        }

        public event Action OnStarted;
        public event Action OnStopped;
        public event Action<ulong> OnClientConnected;
        public event Action<ulong> OnClientDisconnected;

        public abstract void Start();
        public abstract void Stop();

        public event Action<ulong, string> OnUnnamedMessage;

        public void SendCountdownMessage(double startTime);

        /// <summary>
        /// Send unnamed message to specific clients
        /// </summary>
        /// <param name="message">message to be sent</param>
        /// <param name="clientIds">list of target client id, sent to everyone if null</param>
        public void SendUnnamedMessage(string message, IReadOnlyList<ulong> clientIds = null);

        /// <summary>
        /// Send unnamed message to specific client
        /// </summary>
        /// <param name="message">message to be sent</param>
        /// <param name="clientId">target client id</param>
        public void SendUnnamedMessage(string message, ulong clientId);
    }
}
