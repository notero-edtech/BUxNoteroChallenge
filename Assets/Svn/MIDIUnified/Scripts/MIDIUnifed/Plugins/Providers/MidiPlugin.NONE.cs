/* Copyright © Marek Ledvina, Foriero s.r.o. */

namespace ForieroEngine.MIDIUnified.Plugins
{
    internal class MidiINDeviceNONE : IMidiINDevice
    {
        #region implementation

        public bool Init() => true;
        public int ConnectDevice(int deviceIndex, bool editor = false) => -1;
        public void DisconnectDevice(int deviceId, bool editor = false) { }
        public void DisconnectDevices(bool editor = false) { }
        public string GetDeviceName(int deviceIndex) => "";
        public int GetDeviceCount() => 0;
        public int OpenVirtualPort(string name, bool editor = false) => 0;
        public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int PopMessage(out MidiMessage midiMessage, bool editor = false) { midiMessage = new MidiMessage(); return 0; }

        #endregion
    }

    internal class MidiOUTDeviceNONE : IMidiOUTDevice
    {

        #region implementation

        public bool Init() => true;
        public int ConnectDevice(int deviceIndex, bool editor = false) => -1;
        public void DisconnectDevice(int deviceId, bool editor = false) { }
        public void DisconnectDevices(bool editor = false) { }
        public string GetDeviceName(int deviceIndex) => "";
        public int GetDeviceCount() => 0;
        public int OpenVirtualPort(string name, bool editor = false) => 0;
        public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int SendMessage(byte command, byte data1, byte data2, int deviceId = -1, bool editor = false) => 0;
        public int SendData(byte[] data, int deviceId = -1, bool editor = false) => 0;

        #endregion
    }
}