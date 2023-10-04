using UnityEngine;

namespace Notero.MidiAdapter.Reconnection
{
    [CreateAssetMenu(fileName = "NewMidiReconnectionConfig", menuName = "Template/MidiReconnectionConfig")]
    public class MidiReconnectionConfig : ScriptableObject
    {
        [Header("System will find midi devices in every x seconds")]
        [SerializeField]
        private float m_PingDeviceInSeconds;

        public float PingDeviceInSeconds => m_PingDeviceInSeconds;
    }
}