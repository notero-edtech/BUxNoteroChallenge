using Mirror;
using Notero.Unity.Networking.Common;
using Notero.Unity.Networking.Mirror.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using MirrorNetworkTime = Mirror.NetworkTime;

namespace Notero.Unity.Networking.Mirror
{
    public class Server : IServer
    {
        public bool IsStarted => NetworkServer.active;

        public ulong Id => 0;

        public string IPAddress => GetLocalIPAddress().IPv4;

        public bool IsIPAddressLoopback => GetLocalIPAddress().IsLoopback;

        public double NetworkTime => MirrorNetworkTime.time;

        public int MaxConnections { get; set; }

        public int NumberOfConnection => m_Sessions.Count;

        public event Action OnStarted;
        public event Action OnStopped;
        public event Action<ulong> OnClientConnected;
        public event Action<ulong> OnClientDisconnected;
        public event Action<ulong, string> OnUnnamedMessage;

        private Dictionary<ulong /*sessionId*/, NetworkConnectionToClient> m_Sessions = new();

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

        protected NetworkManagerContainer NetworkManagerContainer => m_NetworkManagerContainer ??= new NetworkManagerContainer();
        protected MirrorNetworkManager NetworkManager => NetworkManagerContainer.NetworkManager;

        private bool m_IsConnectionEventsRegistered;
        private bool m_IsMessagesRegistered;

        private NetworkManagerContainer m_NetworkManagerContainer;
        private IStandardTransport m_CurrentActiveTransport;
        private Transport m_PreviousActiveTransport;

        public ushort Port
        {
            get => CurrentActiveTransport.Port;
            set => CurrentActiveTransport.Port = value;
        }

        protected bool IsActiveTransportReady => Transport.activeTransport != null;

        public Server(ushort port)
        {
            NetworkManagerContainer.Init();
            Port = port;
        }

        public void Start()
        {
            RegisterConnectionEvents();
            RegisterMessages();
            NetworkManager.StartServer();
        }

        public void Stop()
        {
            NetworkManager.StopServer();
        }

        public void Broadcast<T>(T message, IReadOnlyList<ulong> sessionIds = null) where T : struct, NetworkMessage
        {
            sessionIds ??= m_Sessions.Select(connection => connection.Key).ToList();

            foreach(var sessionId in sessionIds)
            {
                if(m_Sessions.ContainsKey(sessionId))
                {
                    m_Sessions[sessionId].Send(message);
                }
            }
        }

        public void SendTo<T>(T message, ulong sessionId) where T : struct, NetworkMessage
        {
            if(!m_Sessions.ContainsKey(sessionId)) return;

            m_Sessions[sessionId].Send(message);
        }

        public void Register<T>(Action<NetworkConnectionToClient, T> handler) where T : struct, NetworkMessage
        {
            NetworkServer.RegisterHandler<T>(handler);
        }

        public void Unregister<T>() where T : struct, NetworkMessage
        {
            NetworkServer.UnregisterHandler<T>();
        }

        public void SendUnnamedMessage(string message, IReadOnlyList<ulong> sessionIds = null)
        {
            sessionIds ??= m_Sessions.Select(connection => connection.Key).ToList();

            var messageObject = new UnnamedMessage(message, sessionIds);

            foreach(var sessionId in sessionIds)
            {
                if(m_Sessions.ContainsKey(sessionId))
                {
                    m_Sessions[sessionId].Send(messageObject);
                }
            }
        }

        public void SendUnnamedMessage(string message, ulong sessionId)
        {
            if(!m_Sessions.ContainsKey(sessionId)) return;

            var messageObject = new UnnamedMessage(message, sessionId);
            m_Sessions[sessionId].Send(messageObject);
        }

        protected virtual void OnMessageRegistered()
        {
            NetworkServer.RegisterHandler<UnnamedMessage>(OnUnnamedMessageReceived);
        }

        protected virtual void OnMessageUnregistered()
        {
            NetworkServer.UnregisterHandler<UnnamedMessage>();
        }

        private void RegisterConnectionEvents()
        {
            if(m_IsConnectionEventsRegistered) return;

            m_IsConnectionEventsRegistered = true;

            NetworkManager.OnServerStarted += OnServerStarted;
            NetworkManager.OnServerStopped += OnServerStopped;

            NetworkManager.OnClientConnected += OnClientConnectedCallback;
            NetworkManager.OnClientDisconnected += OnClientDisconnectedCallback;
        }

        private void UnregisterConnectionEvents()
        {
            if(!m_IsConnectionEventsRegistered) return;

            m_IsConnectionEventsRegistered = false;

            NetworkManager.OnServerStarted -= OnServerStarted;
            NetworkManager.OnServerStopped -= OnServerStopped;

            NetworkManager.OnClientConnected -= OnClientConnectedCallback;
            NetworkManager.OnClientDisconnected -= OnClientDisconnectedCallback;
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

        private void OnServerStarted()
        {
            OnStarted?.Invoke();
        }

        private void OnServerStopped()
        {
            UnregisterMessage();
            UnregisterConnectionEvents();
            OnStopped?.Invoke();
        }

        private void OnClientConnectedCallback(NetworkConnectionToClient connection)
        {
            var sessionId = (ulong)connection.connectionId;

            if(m_Sessions.ContainsKey(sessionId))
            {
                m_Sessions[sessionId] = connection;
            }
            else
            {
                m_Sessions.Add(sessionId, connection);
            }

            OnClientConnected?.Invoke(sessionId);
        }

        private void OnClientDisconnectedCallback(NetworkConnectionToClient connection)
        {
            var sessionId = (ulong)connection.connectionId;
            m_Sessions.Remove(sessionId);

            OnClientDisconnected?.Invoke(sessionId);
        }

        private (string IPv4, bool IsLoopback) GetLocalIPAddress()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = hostEntry.AddressList.First(address => address.AddressFamily == AddressFamily.InterNetwork);

            return (ipAddress.ToString(), System.Net.IPAddress.IsLoopback(ipAddress));
        }

        private void OnUnnamedMessageReceived(NetworkConnectionToClient senderConnection, UnnamedMessage message)
        {
            OnUnnamedMessage?.Invoke((ulong)senderConnection.connectionId, message.Message);
        }
    }
}