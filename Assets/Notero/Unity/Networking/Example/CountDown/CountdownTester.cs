using Notero.Unity.Networking.Example.Countdown.Interface;
using Notero.Unity.Networking.Mirror.Example;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.Networking.Example.Countdown
{
    public class CountdownTester : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField]
        private Button m_StartHostButton;
        [SerializeField]
        private Button m_ConnectToHostButton;
        [SerializeField]
        private Button m_ShutdownButton;
        [SerializeField]
        private Button m_StartCoundownButton;

        [Header("InputFields")]
        [SerializeField]
        private InputField m_IPAddress;

        [Header("Log")]
        [SerializeField]
        private InputField m_MessageField;

        [Header("Status")]
        [SerializeField]
        private Text m_StatusText;

        [Header("Countdown")]
        [SerializeField]
        private CountDownPanel m_CountdownPanel;

        private IServerNetworkTimeTestable m_Server;
        private IClientNetworkTimeTestable m_Client;

        private const double timeBuffer = 0.600;

        public void Awake()
        {
            ClearLog();
        }

        public void OnDestroy()
        {
        }

        private void Update()
        {
            m_StartHostButton.interactable = m_Server == null && m_Client == null;
            m_ConnectToHostButton.interactable = m_Server == null && m_Client == null;
            m_ShutdownButton.interactable = !(m_Server == null && m_Client == null);
            m_StartCoundownButton.interactable = m_Server != null;

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            string statusText = string.Empty;

            if(m_Server != null)
            {
                statusText += $"NetworkTime:\t{m_Server.NetworkTime:0.000}\n";
                statusText += $"IsStarted:\t{m_Server.IsStarted}\n";
            }

            if(m_Client != null)
            {
                statusText += $"NetworkTime:\t{m_Client.NetworkTime:0.000}\n";
                statusText += $"IsConnected:\t{m_Client.IsConnected}\n";
                statusText += $"ID:\t{m_Client.Id}\n";
            }

            m_StatusText.text = statusText;
        }

        private void Log(string message)
        {
            Debug.Log(message);
            m_MessageField.text = message + '\n' + m_MessageField.text;
        }

        public void ClearLog()
        {
            m_MessageField.text = "Hi!";
        }

        public void StartHost()
        {
            Log("StartHost");

            var server = new CountdownServer(7777);
            m_Server = server;
            m_Server.OnStarted += OnServerStarted;
            m_Server.OnStopped += OnStopped;
            m_Server.OnClientConnected += OnClientConnected;
            m_Server.OnClientDisconnected += OnClientDisconnected;

            m_Server.Start();
            RegisterEvents();
        }

        public void StartClient()
        {
            Log("StartClient");

            if(!IPAddress.TryParse(m_IPAddress.text, out var address))
                return;

            var client = new CountdownClient(address, 7777);
            m_Client = client;
            m_Client.OnConnected += OnConnected;
            m_Client.OnDisconnected += OnDisconnected;

            m_Client.ConnectionTimeout = 7000;
            m_Client.Connect();
            RegisterEvents();
        }

        public void Shutdown()
        {
            Log("Shutdown");
            if(m_Server != null)
            {
                m_Server.Stop();
                m_Server.OnStarted -= OnServerStarted;
                m_Server.OnStopped -= OnStopped;
                m_Server.OnClientConnected -= OnClientConnected;
                m_Server.OnClientDisconnected -= OnClientDisconnected;
                m_Server = null;
            }

            if(m_Client != null)
            {
                m_Client.Disconnect();
                m_Client = null;
            }

            UnregisterEvents();
        }

        public void SendUnnamedMessageTo0()
        {
            Log("SendUnnamedMessageTo0");
            SendSimpleMessage(0);
        }

        public void SendUnnamedMessageTo1()
        {
            Log("SendUnnamedMessageTo1");
            SendSimpleMessage(1);
        }

        public void SendUnnamedMessageTo2()
        {
            Log("SendUnnamedMessageTo2");
            SendSimpleMessage(2);
        }

        public void SendUnnamedMessageTo3()
        {
            Log("SendUnnamedMessageTo3");
            SendSimpleMessage(3);
        }

        public void BroadcastUnnamedMessage()
        {
            Log("BroadcastUnnamedMessage");
            SendSimpleMessage();
        }

        public void Countdown()
        {
            if(m_Server != null)
            {
                Log($"Countdown");
                double startTime = timeBuffer + m_Server.NetworkTime;
                m_Server.SendCountdownMessage(startTime);
                m_CountdownPanel.Countdown((float)(startTime - m_Server.NetworkTime));
            }
        }

        private void SendSimpleMessage()
        {
            if(m_Server != null)
            {
                string message = $"Message from ID:{m_Server.Id} NetworkTime:{m_Server.NetworkTime:0.000}";
                m_Server.SendUnnamedMessage(message);
            }
            else if(m_Client != null)
            {
                string message = $"Message from ID:{m_Client.Id} NetworkTime:{m_Client.NetworkTime:0.000}";
                m_Client.SendUnnamedMessage(message);
            }
        }

        private void SendSimpleMessage(ulong clientId)
        {
            if(m_Server != null)
            {
                string message = $"Message from ID:{m_Server.Id} NetworkTime:{m_Server.NetworkTime:0.000}";
                m_Server.SendUnnamedMessage(message, clientId);
            }
            else if(m_Client != null)
            {
                string message = $"Message from ID:{m_Client.Id} NetworkTime:{m_Client.NetworkTime:0.000}";
                m_Client.SendUnnamedMessage(message, clientId);
            }
        }

        bool m_IsUnnamedMessageRegistered = false;
        private void RegisterEvents()
        {
            if(m_IsUnnamedMessageRegistered)
                return;

            if(m_Server != null)
            {
                m_Server.OnUnnamedMessage += OnUnnamedMessageReceived;
            }
            else if(m_Client != null)
            {
                m_Client.OnUnnamedMessage += OnUnnamedMessageReceived;
                m_Client.OnCountdown += OnCountdown;
            }
        }

        private void UnregisterEvents()
        {
            if(!m_IsUnnamedMessageRegistered)
                return;

            if(m_Server != null)
            {
                m_Server.OnUnnamedMessage -= OnUnnamedMessageReceived;
            }
            else if(m_Client != null)
            {
                m_Client.OnUnnamedMessage -= OnUnnamedMessageReceived;
                m_Client.OnCountdown -= OnCountdown;
            }
        }

        private void OnUnnamedMessageReceived(ulong senderId, string message)
        {
            double time = 0;
            if(m_Server != null)
            {
                time = m_Server.NetworkTime;
            }
            else if(m_Client != null)
            {
                time = m_Client.NetworkTime;
            }

            Log($"OnUnnamedMessageReceived(senderId:{senderId},message:{message}) NetworkTime:{time:0.000}");
        }

        private void OnCountdown(ulong senderId, double startTime)
        {
            double time = 0;
            if(m_Server != null)
            {
                time = m_Server.NetworkTime;
            }
            else if(m_Client != null)
            {
                time = m_Client.NetworkTime;
            }

            Log($"OnCountdown startTime:{startTime}");
            m_CountdownPanel.Countdown((float)(startTime - time));
        }

        private void OnServerStarted()
        {
            Log($"OnServerStarted ID:{m_Server.Id}");
        }

        private void OnStopped()
        {
            Log($"OnStopped ID:{m_Server.Id}");
        }

        private void OnClientConnected(ulong clientId)
        {
            Log($"OnClientConnected({clientId})");
        }

        private void OnClientDisconnected(ulong clientId)
        {
            Log($"OnClientDisconnected({clientId})");
        }

        private void OnConnected()
        {
            Log($"OnConnected ID:{m_Client.Id}");
        }

        private void OnDisconnected()
        {
            Log("OnDisconnected");
            UnregisterEvents();
            m_Client = null;
        }
    } 
}
