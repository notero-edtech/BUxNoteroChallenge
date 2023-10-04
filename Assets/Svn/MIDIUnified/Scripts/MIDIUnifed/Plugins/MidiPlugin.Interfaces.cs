namespace ForieroEngine.MIDIUnified.Plugins
{
    public interface IMidiDevice
    {
        bool Init();

        // returns deviceId that can be used for filtering //
        int ConnectDevice(int deviceIndex, bool editor = false);
        void DisconnectDevice(int deviceId, bool editor = false);
        void DisconnectDevices(bool editor = false);

        int GetDeviceCount();
        string GetDeviceName(int deviceIndex);

        int OpenVirtualPort(string name, bool editor = false);
        void CloseVirtualPort(int deviceId, bool editor = false);
        void CloseVirtualPorts(bool editor = false);

        int GetVirtualPortCount();
        string GetVirtualPortName(int portIndex);
    }

    public interface IMidiEditorDevice
    {
        int GetConnectedDeviceCount();

        int GetConnectedDeviceId(int connectedDeviceIndex);

        string GetConnectedDeviceName(int connectedDeviceIndex);

        bool GetConnectedDeviceIsEditor(int connectedDeviceIndex);
    }

    public interface IMidiINDevice : IMidiDevice
    {
        int PopMessage(out MidiMessage midiMessage, bool editor = false);
    }

    public interface IMidiOUTDevice : IMidiDevice
    {
        int SendMessage(byte command, byte data1, byte data2, int deviceId = -1, bool editor = false);

        int SendData(byte[] data, int deviceId = -1, bool editor = false);
    }  
}
