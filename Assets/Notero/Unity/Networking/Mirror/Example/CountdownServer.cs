using Mirror;
using Notero.Unity.Networking.Example.Countdown.Interface;
using System;

namespace Notero.Unity.Networking.Mirror.Example
{
    public class CountdownServer : Server, IServerNetworkTimeTestable
    {
        public CountdownServer(ushort port) : base(port) { }

        public event Action<ulong, double> OnCountdown;

        public void SendCountdownMessage(double startTime)
        {
            var messageObject = new CountdownMessage();
            messageObject.StartTime = startTime;
            NetworkServer.SendToAll(messageObject);
        }

        protected override void OnMessageRegistered()
        {
            base.OnMessageRegistered();
            NetworkServer.RegisterHandler<CountdownMessage>(OnCountdownMessageReceived);
        }

        protected override void OnMessageUnregistered()
        {
            base.OnMessageUnregistered();
            NetworkServer.UnregisterHandler<CountdownMessage>();
        }

        private void OnCountdownMessageReceived(NetworkConnectionToClient senderConnection, CountdownMessage message)
        {
            OnCountdown?.Invoke(0, message.StartTime);
        }
    }
}
