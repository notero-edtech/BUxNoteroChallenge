/* Copyright © Marek Ledvina, Foriero s.r.o. */
#if UNITY_WSA && !UNITY_EDITOR

using ForieroEngine.MIDIUnified.Plugins.Internal;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Plugins
{
	internal class MidiINDeviceWSA : IMidiINDevice
	{
		public bool Init () => MidiInPluginWSA.Initialized();
		public int ConnectDevice (int i, bool editor = false) => MidiInPluginWSA.ConnectDevice (i);
		public void DisconnectDevice (int deviceId, bool editor = false) => MidiInPluginWSA.DisconnectDevice (deviceId);
		public void DisconnectDevices (bool editor = false) => MidiInPluginWSA.DisconnectDevices ();
		public string GetDeviceName (int deviceIndex) => MidiInPluginWSA.GetDeviceName (deviceIndex);
		public int GetDeviceCount () => MidiInPluginWSA.GetDeviceCount ();
		public int OpenVirtualPort(string name, bool editor = false) => 0;
		public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int PopMessage (out MidiMessage midiMessage, bool editor = false)
		{
            midiMessage = new MidiMessage();
            var result = MidiInPluginWSA.PopMessage(out var m);
            midiMessage.CopyData(m);
            m.Time = AudioSettings.dspTime;
            MidiInPluginWSA.DeleteData(m.data);
            return result;
		}
	}

	internal class MidiOUTDeviceWSA : IMidiOUTDevice
	{
		public bool Init () => MidiOutPluginWSA.Initialized();
		public int ConnectDevice (int i, bool editor = false) => MidiOutPluginWSA.ConnectDevice (i);
		public void DisconnectDevice (int deviceId, bool editor = false) => MidiOutPluginWSA.DisconnectDevice (deviceId);
		public void DisconnectDevices (bool editor = false) => MidiOutPluginWSA.DisconnectDevices ();
		public string GetDeviceName (int deviceIndex) => MidiOutPluginWSA.GetDeviceName (deviceIndex);
		public int GetDeviceCount () => MidiOutPluginWSA.GetDeviceCount ();
		public int OpenVirtualPort(string name, bool editor = false) => 0;
		public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int SendMessage (byte command, byte data1, byte data2, int deviceId = -1, bool editor = false) => MidiOutPluginWSA.SendMessage (command, data1, data2, deviceId);
        public int SendData (byte[] data, int deviceId = -1, bool editor = false) => MidiOutPluginWSA.SendData (data, deviceId);
	}
}

#endif
