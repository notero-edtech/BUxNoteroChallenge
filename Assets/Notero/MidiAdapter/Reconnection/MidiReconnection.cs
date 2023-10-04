using ForieroEngine.MIDIUnified.Plugins;
using System;
using System.Linq;

namespace Notero.MidiAdapter.Reconnection
{
    public class MidiReconnection
    {
        /// <summary>
        /// true when midi connected, 
        /// false when midi disconnected
        /// </summary>
        public event Action<bool> OnMidiConnectionStatusChanged;

        public bool Enable
        {
            get => m_Enable;
            set
            {
                if(value != m_Enable)
                {
                    OnMidiConnectionStatusChanged?.Invoke(!value);
                    m_Enable = value;
                }
            }
        }

        private bool m_Enable;

        private float m_Timer = 0;
        private float m_WaitSec = 1;

        public MidiReconnection(MidiReconnectionConfig config)
        {
            m_WaitSec = config.PingDeviceInSeconds;
            Enable = true;
        }

        public void UpdateMidiReconnection(float deltaTime)
        {
            m_Timer += deltaTime;

            if(m_Timer >= m_WaitSec)
            {
                if(Enable)
                {
                    m_Timer = 0;
                    ReconnectMidiDevice();
                }

                if(HasNoConnectedMidiDevice())
                {
                    Enable = true;
                }
            }
        }

        private void ReconnectMidiDevice()
        {
            MidiINPlugin.DisconnectDevices();
            MidiAdapter.MidiInputAdapter.Instance.ConnectAllMidiDevices();
        }

        private bool HasNoConnectedMidiDevice()
        {
            MidiINPlugin.Refresh();

            int count = MidiINPlugin.deviceNames.Count(device => !string.IsNullOrEmpty(device));
            return count == 0;
        }
    }
}