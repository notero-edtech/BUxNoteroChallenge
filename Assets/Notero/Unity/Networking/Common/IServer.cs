using System;

namespace Notero.Unity.Networking.Common
{
    public interface IServer
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
    }
}
