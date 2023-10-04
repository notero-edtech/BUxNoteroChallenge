/* Copyright © Marek Ledvina, Foriero s.r.o. */
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Plugins
{
    internal class MidiINDeviceWEBGL : IMidiINDevice
    {
        #region external

        [DllImport("__Internal")] private static extern bool MidiIn_Initialized();
        [DllImport("__Internal")] private static extern int MidiIn_PortOpen(int i);
        [DllImport("__Internal")] private static extern void MidiIn_PortClose(int deviceId);
        [DllImport("__Internal")] private static extern void MidiIn_PortCloseAll();
        [DllImport("__Internal")] private static extern string MidiIn_PortName(int i);
        [DllImport("__Internal")] private static extern int MidiIn_PortCount();
        [DllImport("__Internal")] public static extern bool MidiIn_PortAdded();
        [DllImport("__Internal")] public static extern bool MidiIn_PortRemoved();
        [DllImport("__Internal")] private static extern int MidiIn_PopMessage(byte[] bytes, int size);
        //[DllImport ("__Internal")]
        //public static extern void EnableNetwork(bool enabled);

        #endregion

        #region implementation

        public bool Init() => MidiIn_Initialized();
        public int ConnectDevice(int deviceIndex, bool editor = false) => MidiIn_PortOpen(deviceIndex);
        public void DisconnectDevice(int deviceId, bool editor = false) => MidiIn_PortClose(deviceId);
        public void DisconnectDevices(bool editor = false) => MidiIn_PortCloseAll();
        public string GetDeviceName(int deviceIndex) => MidiIn_PortName(deviceIndex);
        public int OpenVirtualPort(string name, bool editor = false) => -1;
        public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int GetDeviceCount() => MidiIn_PortCount();
        private byte[] _bytes = new byte[32];
        public int PopMessage(out MidiMessage midiMessage, bool editor = false)
        {
            midiMessage = new MidiMessage();
            midiMessage.Time = AudioSettings.dspTime;
            var size = MidiIn_PopMessage(_bytes, 32);
            switch (size)
            {
                case <= 0: return 0;
                case 3: midiMessage.SetShortMessageBytes(_bytes); break;
                default: midiMessage.SetSystemMessageBytes(_bytes, size); break;
            }
            return 1;
        }

        #endregion
    }

    internal class MidiOUTDeviceWEBGL : IMidiOUTDevice
    {
        #region external

        [DllImport("__Internal")] private static extern bool MidiOut_Initialized();
        [DllImport("__Internal")] private static extern int MidiOut_PortOpen(int i);
        [DllImport("__Internal")] private static extern void MidiOut_PortClose(int deviceId);
        [DllImport("__Internal")] private static extern void MidiOut_PortCloseAll();
        [DllImport("__Internal")] private static extern string MidiOut_PortName(int i);
        [DllImport("__Internal")] private static extern int MidiOut_PortCount();
        [DllImport("__Internal")] private static extern int MidiOut_SendMessage(int Command, int Data1, int Data2);
        [DllImport("__Internal")] private static extern int MidiOut_SendData(byte[] Data, int DataSize);
        [DllImport("__Internal")] public static extern bool MidiOut_PortAdded();
        [DllImport("__Internal")] public static extern bool MidiOut_PortRemoved();

        #endregion

        #region implementation

        public bool Init() => MidiOut_Initialized();
        public int ConnectDevice(int deviceIndex, bool editor = false) => MidiOut_PortOpen(deviceIndex);
        public void DisconnectDevice(int deviceId, bool editor = false) => MidiOut_PortClose(deviceId);
        public void DisconnectDevices(bool editor = false) => MidiOut_PortCloseAll();
        public string GetDeviceName(int deviceIndex) => MidiOut_PortName(deviceIndex);
        public int OpenVirtualPort(string name, bool editor = false) => -1;
        public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int GetDeviceCount() => MidiOut_PortCount();
        public int SendMessage(byte command, byte data1, byte data2, int deviceId = -1, bool editor = false) => MidiOut_SendMessage(command, data1, data2);
        public int SendData(byte[] data, int deviceId = -1, bool editor = false) => MidiOut_SendData(data, data.Length);

        #endregion
    }
}

#endif