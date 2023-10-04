using Mirror;
using Notero.Unity.Networking.Example.Countdown.Interface;
using System;
using System.Net;

namespace Notero.Unity.Networking.Mirror.Example
{
    public class CountdownClient : Client, IClientNetworkTimeTestable
    {
        public CountdownClient(IPAddress address, ushort port) : base(address, port) { }

        public event Action<ulong, double> OnCountdown;

        protected override void OnMessageRegistered()
        {
            base.OnMessageRegistered();
            NetworkClient.RegisterHandler<CountdownMessage>(OnCountdownMessageReceived);
        }

        protected override void OnMessageUnregistered()
        {
            base.OnMessageUnregistered();
            NetworkClient.UnregisterHandler<CountdownMessage>();
        }

        private void OnCountdownMessageReceived(CountdownMessage message)
        {
            OnCountdown?.Invoke(0, message.StartTime);
        }
    }
}
