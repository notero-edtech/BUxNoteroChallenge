using Mirror;
using Notero.Unity.Networking.Common;
using Notero.Unity.Networking.Mirror.Message;
using System;
using System.Collections.Generic;
using System.Net;
using MirrorNetworkTime = Mirror.NetworkTime;

namespace Notero.Unity.Networking.Mirror
{
    public class Client : IClient
    {
        public int ConnectionTimeout
        {
            get => CurrentActiveTransport.ConnectionTimeout;
            set => CurrentActiveTransport.ConnectionTimeout = value;
        }

        public ulong Id => (ulong)NetworkManager.Id;

        public bool IsConnected => NetworkClient.isConnected;

        public double NetworkTime => MirrorNetworkTime.time;

        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action OnError;
        public event Action<ulong, string> OnUnnamedMessage;

        protected NetworkManagerContainer NetworkManagerContainer => m_NetworkManagerContainer ??= new NetworkManagerContainer();
        protected MirrorNetworkManager NetworkManager => NetworkManagerContainer.NetworkManager;

        protected IStandardTransport CurrentActiveTransport
        {
            get
            {
                if(Transport.activeTransport == null)
                    return null;

                if(m_PreviousActiveTransport == null || !m_PreviousActiveTransport.Equals(Transport.activeTransport))
                {
                    m_PreviousActiveTransport = Transport.activeTransport;
                    m_CurrentActiveTransport = StandardTransportFactory.Create(m_PreviousActiveTransport);
                }

                return m_CurrentActiveTransport;
            }
        }

        private bool m_IsConnectionEventsRegistered = false;
        private bool m_IsMessagesRegistered = false;

        private NetworkManagerContainer m_NetworkManagerContainer;
        private IStandardTransport m_CurrentActiveTransport;
        private Transport m_PreviousActiveTransport;

        public ushort Port
        {
            get => CurrentActiveTransport.Port;
            set => CurrentActiveTransport.Port = value;
        }

        public IPAddress ServerAddress
        {
            get => IPAddress.Parse(NetworkManager.networkAddress);
            protected set => NetworkManager.networkAddress = value.ToString();
        }

        public Client(IPAddress address, ushort port)
        {
            NetworkManagerContainer.Init();
            Port = port;
            ServerAddress = address;
        }

        public void Connect()
        {
            RegisterConnectionEvents();
            RegisterMessages();
            NetworkManager.StartClient();
        }

        public void Disconnect()
        {
            NetworkManager.StopClient();
        }

        public void SendCountdownMessage(double startTime)
        {
            throw new NotImplementedException();
        }

        public void SendUnnamedMessage(string message, IReadOnlyList<ulong> clientIds = null)
        {
            var messageObject = new UnnamedMessage(message, clientIds);
            NetworkClient.Send(messageObject);
        }

        public void SendUnnamedMessage(string message, ulong clientId)
        {
            var messageObject = new UnnamedMessage(message, clientId);
            NetworkClient.Send(messageObject);
        }

        public void Send<T>(T message) where T : struct, NetworkMessage
        {
            NetworkClient.Send(message);
        }

        public void Register<T>(Action<T> handler) where T : struct, NetworkMessage
        {
            NetworkClient.RegisterHandler(handler);
        }

        public void Unregister<T>() where T : struct, NetworkMessage
        {
            NetworkClient.UnregisterHandler<T>();
        }

        protected virtual void OnMessageRegistered()
        {
            NetworkClient.RegisterHandler<UnnamedMessage>(OnUnnamedMessageReceived);
        }

        protected virtual void OnMessageUnregistered()
        {
            NetworkClient.UnregisterHandler<UnnamedMessage>();
        }

        private void RegisterConnectionEvents()
        {
            if(m_IsConnectionEventsRegistered) return;

            m_IsConnectionEventsRegistered = true;

            NetworkManager.OnConnectedToServer += OnConnectedEvent;
            NetworkManager.OnDisconnectedFromServer += OnDisconnectedEvent;
            NetworkManager.OnClientStopped += OnDisconnectedEvent;
            NetworkManager.OnError += OnErrorEvent;
        }

        private void UnregisterConnectionEvents()
        {
            if(!m_IsConnectionEventsRegistered) return;

            m_IsConnectionEventsRegistered = false;

            NetworkManager.OnConnectedToServer -= OnConnectedEvent;
            NetworkManager.OnDisconnectedFromServer -= OnDisconnectedEvent;
            NetworkManager.OnClientStopped -= OnDisconnectedEvent;
            NetworkManager.OnError -= OnErrorEvent;
        }

        private void RegisterMessages()
        {
            if(m_IsMessagesRegistered) return;

            m_IsMessagesRegistered = true;

            OnMessageRegistered();
        }

        private void UnregisterMessage()
        {
            if(!m_IsMessagesRegistered) return;

            m_IsMessagesRegistered = false;

            OnMessageUnregistered();
        }

        private void OnConnectedEvent()
        {
            OnConnected?.Invoke();
        }

        private void OnDisconnectedEvent()
        {
            UnregisterMessage();
            UnregisterConnectionEvents();
            OnDisconnected?.Invoke();
        }

        private void OnErrorEvent(Exception exception)
        {
            OnError?.Invoke();
        }

        private void OnUnnamedMessageReceived(UnnamedMessage message)
        {
            OnUnnamedMessage?.Invoke(0, message.Message);
        }
    }
}