/* Copyright © Marek Ledvina, Foriero s.r.o. */
#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using ForieroEngine.Extensions;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Plugins
{
    internal class MidiINDeviceANDROID : IMidiINDevice
    {
        public bool Init() { MidiPlugin.Init(); return true; }
        public int ConnectDevice(int deviceIndex, bool editor = false) => MidiPlugin.MIDIIN_ConnectDevice(deviceIndex);
        public void DisconnectDevice(int id, bool editor = false) => MidiPlugin.MIDIIN_DisconnectDevice(id);
        public void DisconnectDevices(bool editor = false) => MidiPlugin.MIDIIN_DisconnectDevices();
        public string GetDeviceName(int deviceIndex) => MidiPlugin.MIDIIN_DeviceName(deviceIndex);
        public int GetDeviceCount() => MidiPlugin.MIDIIN_DeviceCount();
        public int OpenVirtualPort(string name, bool editor = false) => 0;
        public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int PopMessage(out MidiMessage midiMessage, bool editor = false)
        {
            var result = MidiPlugin.MIDIIN_PopMessage(out var m);
            midiMessage = new MidiMessage(m);
            midiMessage.Time = AudioSettings.dspTime;
            return result;
        }
    }

    internal class MidiOUTDeviceANDROID : IMidiOUTDevice
    {
        public bool Init() { MidiPlugin.Init(); return true; }
        public int ConnectDevice(int deviceIndex, bool editor = false) => MidiPlugin.MIDIOUT_ConnectDevice(deviceIndex);
        public void DisconnectDevice(int id, bool editor = false) => MidiPlugin.MIDIOUT_DisconnectDevice(id);
        public void DisconnectDevices(bool editor = false) => MidiPlugin.MIDIOUT_DisconnectDevices();
        public string GetDeviceName(int deviceIndex) => MidiPlugin.MIDIOUT_DeviceName(deviceIndex);
        public int GetDeviceCount() => MidiPlugin.MIDIOUT_DeviceCount();
        public int OpenVirtualPort(string name, bool editor = false) => 0;
        public void CloseVirtualPort(int deviceId, bool editor = false) { }
        public void CloseVirtualPorts(bool editor = false) { }
        public int GetVirtualPortCount() => 0;
        public string GetVirtualPortName(int portIndex) => "";
        public int SendMessage(byte command, byte data1, byte data2, int deviceId = -1, bool editor = false) => MidiPlugin.MIDIOUT_SendData(new byte[] { command, data1, data2 }, deviceId);
        public int SendData(byte[] data, int deviceId = -1, bool editor = false)
        {
            if (data == null || data.Length == 0){ return -1; }
            else{ return MidiPlugin.MIDIOUT_SendData(data, deviceId); }
        }
    }

    internal static class MidiPlugin
    {
        private const string MIDIUnifiedFragmentClass = "com.foriero.midiunifiedplugin.MIDIUnifiedFragment";
        private static bool _initCalled = false;
        private static AndroidJavaObject _midiPlugin = null;
        private static bool _isInitialized = false;

        public static bool Init()
        {
            if (_initCalled) { return _isInitialized; }
            _initCalled = true;
            var jc = new AndroidJavaClass(MIDIUnifiedFragmentClass);
            _midiPlugin = jc.CallStatic<AndroidJavaObject>("Init");
            _isInitialized = _midiPlugin != null;
            return _isInitialized;
        }

        // MIDI IN //

        private const string CMIDIIN_ConnectDevice = "MIDIIN_ConnectDevice";
        private const string CMIDIIN_DisconnectDevice = "MIDIIN_DisconnectDevice";
        private const string CMIDIIN_DisconnectDevices = "MIDIIN_DisconnectDevices";
        private const string CMIDIIN_DeviceName = "MIDIIN_DeviceName";
        private const string CMIDIIN_DeviceCount = "MIDIIN_DeviceCount";
        private const string CMIDIIN_PopMidiMessage = "MIDIIN_PopMidiMessage";
        private const string CMIDIIN_MidiMessage = "MidiMessage";
        
        private static sbyte[] _midiMessage = null;
        private static byte[] _midiMessageByte = null;
        public static int MIDIIN_ConnectDevice(int deviceIndex)
        {
            if (!_isInitialized) return -1;
            return _midiPlugin.Call<int>(CMIDIIN_ConnectDevice, new object[] { deviceIndex });
        }

        public static void MIDIIN_DisconnectDevice(int id)
        {
            if (!_isInitialized) return;
            _midiPlugin.Call(CMIDIIN_DisconnectDevice, new object[] { id });
        }

        public static void MIDIIN_DisconnectDevices()
        {
            if (!_isInitialized) return;
            _midiPlugin.Call(CMIDIIN_DisconnectDevices);
        }

        public static string MIDIIN_DeviceName(int deviceIndex)
        {
            if (!_isInitialized) return "";
            return _midiPlugin.Call<String>(CMIDIIN_DeviceName, new object[] { deviceIndex });
        }

        public static int MIDIIN_DeviceCount()
        {
            if (!_isInitialized) return 0;
            return _midiPlugin.Call<int>(CMIDIIN_DeviceCount);
        }
        
        public static int MIDIIN_PopMessage(out MidiMessage aMidiMessage)
        {
            aMidiMessage = new MidiMessage();
			if (!_isInitialized) return 0;

            if (_midiPlugin.CallStatic<int>(CMIDIIN_PopMidiMessage) == 0) return 0;
            _midiMessage = _midiPlugin.GetStatic<sbyte[]>(CMIDIIN_MidiMessage);
            if (_midiMessage == null) return 0;
            
            _midiMessageByte = _midiMessage.ToByte();
            
            if (_midiMessageByte.Length <= 1) return 0;
            
            aMidiMessage.DeviceId = _midiMessageByte[0];
            if(_midiMessageByte.Length == 4)
            {
                aMidiMessage.CommandAndChannel = _midiMessageByte[1];
                aMidiMessage.Data1 = _midiMessageByte[2];
                aMidiMessage.Data2 = _midiMessageByte[3];
            }
            aMidiMessage.DataSize = _midiMessageByte.Length - 1;
            aMidiMessage.Data = new byte[aMidiMessage.DataSize];
            for (var i = 1; i < _midiMessageByte.Length; i++) { aMidiMessage.Data[i - 1] = _midiMessageByte[i]; }
            
            return 1;
        }

        // MIDI OUT //

        private const string CMIDIOUT_ConnectDevice = "MIDIOUT_ConnectDevice";
        private const string CMIDIOUT_DisconnectDevice = "MIDIOUT_DisconnectDevice";
        private const string CMIDIOUT_DisconnectDevices = "MIDIOUT_DisconnectDevices";
        private const string CMIDIOUT_DeviceName = "MIDIOUT_DeviceName";
        private const string CMIDIOUT_DeviceCount = "MIDIOUT_DeviceCount";
        private const string CMIDIOUT_SendData = "MIDIOUT_SendData";
        public static int MIDIOUT_ConnectDevice(int deviceIndex)
        {
            if (!_isInitialized) return -1;
            return _midiPlugin.Call<int>(CMIDIOUT_ConnectDevice, new object[] { deviceIndex });
        }

        public static void MIDIOUT_DisconnectDevice(int id)
        {
            if (!_isInitialized) return;
            _midiPlugin.Call(CMIDIOUT_DisconnectDevice, new object[] { id });
        }

        public static void MIDIOUT_DisconnectDevices()
        {
            if (!_isInitialized) return;
            _midiPlugin.Call(CMIDIOUT_DisconnectDevices);
        }

        public static string MIDIOUT_DeviceName(int deviceIndex)
        {
            if (!_isInitialized) return "";
            return _midiPlugin.Call<String>(CMIDIOUT_DeviceName, new object[] { deviceIndex });
        }

        public static int MIDIOUT_DeviceCount()
        {
            if (!_isInitialized) return 0;
            return _midiPlugin.Call<int>(CMIDIOUT_DeviceCount);
        }

        public static int MIDIOUT_SendData(byte[] data, int deviceId = -1)
        {
            if (!_isInitialized) return -1;

            int[] dst = new int[data.Length];

            for (var i = 0; i < data.Length;i++){ dst[i] = (int)data[i]; }

            _midiPlugin.Call(CMIDIOUT_SendData, new object[] { dst, deviceId });
            return 1;
        }
    }
}
#endif
