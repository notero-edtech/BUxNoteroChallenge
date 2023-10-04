/* Copyright © Marek Ledvina, Foriero s.r.o. */
#if UNITY_IOS && !UNITY_EDITOR

using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace ForieroEngine.MIDIUnified.Plugins
{
    internal class MidiINDeviceIOS : IMidiINDevice, IMidiEditorDevice
    {
        #region external

        private const string DllName = "__Internal";
        [DllImport(DllName)] private static extern int Init(string appId);
        [DllImport(DllName)] private static extern int MidiIn_PortOpen(int deviceIndex, bool editor);
        [DllImport(DllName)] private static extern void MidiIn_PortClose(int deviceId, bool editor);
        [DllImport(DllName)] private static extern void MidiIn_PortCloseAll(bool editor);
        [DllImport(DllName)] private static extern string MidiIn_PortName(int i);
        [DllImport(DllName)] private static extern int MidiIn_PortCount();
        [DllImport(DllName)] private static extern int MidiIn_PortOpenVirtual(string name, bool editor);
        [DllImport(DllName)] private static extern void MidiIn_PortCloseVirtual(int deviceId, bool editor);
        [DllImport(DllName)] private static extern void MidiIn_PortCloseAllVirtual(bool editor);
        [DllImport(DllName)] private static extern string MidiIn_PortNameVirtual(int i);
        [DllImport(DllName)] private static extern int MidiIn_PortCountVirtual();
        [DllImport(DllName)] private static extern int MidiIn_PopMessage(out NativeMidiMessage midiMessage, bool editor);
        [DllImport(DllName)] private static extern void MidiIn_DeleteData(IntPtr midiMessage);
        [DllImport(DllName)] public static extern int MidiIn_GetConnectedDeviceCount();
        [DllImport(DllName)] public static extern int MidiIn_GetConnectedDeviceId(int connectedDeviceIndex);
        [DllImport(DllName)] public static extern string MidiIn_GetConnectedDeviceName(int connectedDeviceIndex);
        [DllImport(DllName)] public static extern bool MidiIn_GetConnectedDeviceIsEditor(int connectedDeviceIndex);

        #endregion

        #region implementation

        public bool Init() => Init(MIDISettings.instance.appId) == 1;
        public int ConnectDevice(int deviceIndex, bool editor = false) => MidiIn_PortOpen(deviceIndex, editor);
        public void DisconnectDevice(int deviceId, bool editor = false) => MidiIn_PortClose(deviceId, editor);
        public void DisconnectDevices(bool editor = false) => MidiIn_PortCloseAll(editor);
        public string GetDeviceName(int deviceIndex) => MidiIn_PortName(deviceIndex);
        public int GetDeviceCount() => MidiIn_PortCount();
        public int OpenVirtualPort(string name, bool editor = false) => MidiIn_PortOpenVirtual(name, editor);
        public void CloseVirtualPort(int deviceId, bool editor = false) => MidiIn_PortCloseVirtual(deviceId, editor);
        public void CloseVirtualPorts(bool editor = false) => MidiIn_PortCloseAllVirtual(editor);
        public int GetVirtualPortCount() => MidiIn_PortCountVirtual();
        public string GetVirtualPortName(int portIndex) => MidiIn_PortNameVirtual(portIndex);
        public int PopMessage(out MidiMessage midiMessage, bool editor = false)
        {
            midiMessage = new MidiMessage();
            var result = MidiIn_PopMessage(out var m, editor);
            midiMessage.CopyData(m);
            midiMessage.Time = AudioSettings.dspTime;
            DeleteData(m.data);
            return result;
        }

        private static void DeleteData(IntPtr data) { if(data != IntPtr.Zero) MidiIn_DeleteData(data); }
        public int GetConnectedDeviceCount() => MidiIn_GetConnectedDeviceCount();
        public int GetConnectedDeviceId(int connectedDeviceIndex) => MidiIn_GetConnectedDeviceId(connectedDeviceIndex);
        public string GetConnectedDeviceName(int connectedDeviceIndex) => MidiIn_GetConnectedDeviceName(connectedDeviceIndex);
        public bool GetConnectedDeviceIsEditor(int connectedDeviceIndex) => MidiIn_GetConnectedDeviceIsEditor(connectedDeviceIndex);

        #endregion
    }

    internal class MidiOUTDeviceIOS : IMidiOUTDevice, IMidiEditorDevice
    {
        #region external

        private const string DllName = "__Internal";
        [DllImport(DllName)] private static extern int Init(string appId);
        [DllImport(DllName)] private static extern int MidiOut_PortOpen(int deviceIndex, bool editor);
        [DllImport(DllName)] private static extern void MidiOut_PortClose(int deviceId, bool editor);
        [DllImport(DllName)] private static extern void MidiOut_PortCloseAll(bool editor);
        [DllImport(DllName)] private static extern string MidiOut_PortName(int deviceIndex);
        [DllImport(DllName)] private static extern int MidiOut_PortCount();
        [DllImport(DllName)] private static extern int MidiOut_PortOpenVirtual(string name, bool editor);
        [DllImport(DllName)] private static extern void MidiOut_PortCloseVirtual(int deviceId, bool editor);
        [DllImport(DllName)] private static extern void MidiOut_PortCloseAllVirtual(bool editor);
        [DllImport(DllName)] private static extern string MidiOut_PortNameVirtual(int deviceIndex);
        [DllImport(DllName)] private static extern int MidiOut_PortCountVirtual();
        [DllImport(DllName)] private static extern int MidiOut_SendMessage(int command, int data1, int data2, int deviceId, bool editor);
        [DllImport(DllName)] private static extern int MidiOut_SendData(byte[] Data, int dataSize, int deviceId, bool editor);
        [DllImport(DllName)] public static extern int MidiOut_GetConnectedDeviceCount();
        [DllImport(DllName)] public static extern int MidiOut_GetConnectedDeviceId(int connectedDeviceIndex);
        [DllImport(DllName)] public static extern string MidiOut_GetConnectedDeviceName(int connectedDeviceIndex);
        [DllImport(DllName)] public static extern bool MidiOut_GetConnectedDeviceIsEditor(int connectedDeviceIndex);

        #endregion

        #region implementation

        public bool Init() => Init(MIDISettings.instance.appId) == 1;
        public int ConnectDevice(int deviceIndex, bool editor = false) => MidiOut_PortOpen(deviceIndex, editor);
        public void DisconnectDevice(int deviceId, bool editor = false) => MidiOut_PortClose(deviceId, editor);
        public void DisconnectDevices(bool editor = false) => MidiOut_PortCloseAll(editor);
        public string GetDeviceName(int deviceIndex) => MidiOut_PortName(deviceIndex);
        public int GetDeviceCount() => MidiOut_PortCount();
        public int OpenVirtualPort(string name, bool editor = false) => MidiOut_PortOpenVirtual(name, editor);
        public void CloseVirtualPort(int deviceId, bool editor = false) => MidiOut_PortCloseVirtual(deviceId, editor);
        public void CloseVirtualPorts(bool editor = false) => MidiOut_PortCloseAllVirtual(editor);
        public int GetVirtualPortCount() => MidiOut_PortCountVirtual();
        public string GetVirtualPortName(int portIndex) => MidiOut_PortNameVirtual(portIndex);
        public int SendMessage(byte command, byte data1, byte data2, int deviceId = -1, bool editor = false) => MidiOut_SendMessage(command, data1, data2, deviceId, editor);
        public int SendData(byte[] data, int deviceId = -1, bool editor = false) => MidiOut_SendData(data, data.Length, deviceId, editor);
        public int GetConnectedDeviceCount() => MidiOut_GetConnectedDeviceCount();
        public int GetConnectedDeviceId(int connectedDeviceIndex) => MidiOut_GetConnectedDeviceId(connectedDeviceIndex);
        public string GetConnectedDeviceName(int connectedDeviceIndex) => MidiOut_GetConnectedDeviceName(connectedDeviceIndex);
        public bool GetConnectedDeviceIsEditor(int connectedDeviceIndex) => MidiOut_GetConnectedDeviceIsEditor(connectedDeviceIndex);

        #endregion
    }
}

#endif
