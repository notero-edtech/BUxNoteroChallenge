using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForieroEngine.Collections.NonBlocking;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ForieroEngine.MIDIUnified.Plugins
{
    public static partial class MidiOUTPlugin
    {
        public static IMidiOUTDevice iMidiOUTDevice;
        public static IMidiEditorDevice iMidiEditorDevice;

#if UNITY_EDITOR
        [InitializeOnLoad] private class InitEditor { static InitEditor() { if (!Application.isPlaying) { Init(); } } }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitPlayer()
        {
            var stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
            if (Application.isPlaying) { Init(); }
            if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (MidiPlugin.OUT - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

        public static void Init()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            var device = new MidiOUTDeviceSTANDALONE();
#elif UNITY_IOS
			var device = new MidiOUTDeviceIOS();
#elif UNITY_WSA
            var device = new MidiOUTDeviceWSA();
#elif UNITY_WEBGL
			var device = new MidiOUTDeviceWEBGL();
#elif UNITY_ANDROID
            AndroidJNI.AttachCurrentThread();
            var device = new MidiOUTDeviceANDROID();
#else
            var device = new MidiOUTDeviceNONE();
#endif
            iMidiDevice = device as IMidiDevice;
            iMidiOUTDevice = device as IMidiOUTDevice;
            iMidiEditorDevice = device as IMidiEditorDevice;

            initialized = iMidiDevice.Init();

            if (!initialized)
            {
                Debug.LogError("MidiOUTDevice not initialized");
                return;
            }
#if UNITY_EDITOR
            if(EditorApplication.isPlaying)
#endif
            { MidiThread.Initialise(); }

            Refresh();

#if UNITY_ANDROID && !UNITY_EDITOR
            //AndroidJNI.DetachCurrentThread();
#endif
        }

        public static volatile bool initialized = false;

        public static List<MidiDevice> connectedDevices = new List<MidiDevice>();
        public static List<string> deviceNames = new List<string>();
        static IMidiDevice iMidiDevice;

        public static List<MidiDevice> connectedEditorDevices = new List<MidiDevice>();

        public static Action<MidiDevice> OnDeviceConnected;
        public static Action<MidiDevice> OnEditorDeviceConnected;

        public static Action<MidiDevice> OnDeviceDisconnected;
        public static Action<MidiDevice> OnEditorDeviceDisconnected;

#if UNITY_EDITOR
        public static void StoreEditorConnections()
        {
            var connectionNames = new List<string>();
            foreach (var device in connectedEditorDevices) { connectionNames.Add(device.name); }
            var s = string.Join(";", connectionNames.ToArray());
            EditorPrefs.SetString("MIDI_OUT_NAMES", s);
        }

        public static void RestoreEditorConnections()
        {
            var names = EditorPrefs.GetString("MIDI_OUT_NAMES", "").Trim();
            var midiInNames = string.IsNullOrEmpty(names) ? Array.Empty<string>() : names.Split(';');

            foreach (var midiInName in midiInNames)
            {
                if (ConnectDeviceByName(midiInName, true) == null) { Debug.LogError("Could not conned midi in device : " + midiInName); }
            }
        }
#endif

        public static void StoreConnections()
        {
            var connectionNames = new List<string>();
            foreach (var device in connectedDevices) { connectionNames.Add(device.name); }
            var s = string.Join(";", connectionNames.ToArray());
            PlayerPrefs.SetString("MIDI_OUT_NAMES", s);
        }

        public static void RestoreConnections()
        {
            var names = PlayerPrefs.GetString("MIDI_OUT_NAMES", "").Trim();
            var midiInNames = string.IsNullOrEmpty(names) ? Array.Empty<string>() : names.Split(';');

            foreach (string midiInName in midiInNames)
            {
                if (ConnectDeviceByName(midiInName) == null) { Debug.LogError("Could not conned midi in device : " + midiInName); }
            }
        }

        public static void Refresh()
        {
            if (iMidiDevice == null) return;

            deviceNames = new List<string>();
            for (var i = 0; i < iMidiDevice.GetDeviceCount(); i++) { deviceNames.Add(iMidiDevice.GetDeviceName(i)); }

            if (iMidiEditorDevice == null) return;

            connectedEditorDevices = new List<MidiDevice>();
            for (var i = 0; i < iMidiEditorDevice.GetConnectedDeviceCount(); i++)
            {
                if (iMidiEditorDevice.GetConnectedDeviceIsEditor(i))
                {
                    var md = new MidiDevice();
                    md.name = iMidiEditorDevice.GetConnectedDeviceName(i);
                    md.deviceId = iMidiEditorDevice.GetConnectedDeviceId(i);
                    md.isEditor = true;
                    connectedEditorDevices.Add(md);
                }
            }
        }

        public static bool Initialized() => iMidiDevice != null && initialized;

        public static MidiDevice ConnectDevice(int deviceIndex, bool editor = false)
        {
            if (iMidiDevice == null) return null;

            foreach (MidiDevice md in MidiINPlugin.connectedDevices)
            {
                if (md.name == iMidiDevice.GetDeviceName(deviceIndex) && MIDISettings.instance.midiInOutExclusive)
                {
                    Debug.LogError("Preventing infinite loop. To have same device connected as IN and OUT is not allowed.");
                    return null;
                }
            }

            int deviceId = iMidiDevice.ConnectDevice(deviceIndex, editor);

            if (editor)
            {
                foreach (var cd in connectedEditorDevices)
                {
                    if (deviceId == cd.deviceId)
                    {
                        Debug.LogError("Editor device already connected");
                        return cd;
                    }
                }
            }
            else
            {
                foreach (var cd in connectedDevices)
                {
                    if (deviceId == cd.deviceId)
                    {
                        Debug.LogError("Device already connected");
                        return cd;
                    }
                }
            }

            var connectedDevice = new MidiDevice
            {
                deviceId = deviceId,
                name = GetDeviceName(deviceIndex),
                isEditor = editor
            };

            if (editor)
            {
                connectedEditorDevices.Add(connectedDevice);
                OnEditorDeviceConnected?.Invoke(connectedDevice);
            }
            else
            {
                connectedDevices.Add(connectedDevice);
                OnDeviceConnected?.Invoke(connectedDevice);
            }

            return connectedDevice;
        }

        public static void DisconnectDevices(bool editor = false)
        {
            if (iMidiDevice == null) return;
            if (editor) { connectedEditorDevices = new List<MidiDevice>(); }
            else { connectedDevices = new List<MidiDevice>(); }
            iMidiDevice.DisconnectDevices(editor);
        }


        public static MidiDevice ConnectDeviceByName(string deviceName, bool editor = false)
        {
            if (iMidiDevice == null) return null;
            foreach (var md in MidiINPlugin.connectedDevices)
            {
                if (md.name == deviceName && MIDISettings.instance.midiInOutExclusive)
                {
                    Debug.LogError("Preventing infinite loop. To have same device connected as IN and OUT is not allowed.");
                    return null;
                }
            }
            for (var i = 0; i < GetDeviceCount(); i++) { if (deviceName == GetDeviceName(i)) { return ConnectDevice(i, editor); } }
            return null;
        }

        public static void DisconnectDevice(MidiDevice connectedDevice)
        {
            if (iMidiDevice == null || connectedDevice == null) return;

            if (connectedDevice.isEditor)
            {
                for (var i = connectedEditorDevices.Count - 1; i >= 0; i--)
                {
                    if (connectedDevice.deviceId == connectedEditorDevices[i].deviceId)
                    {
                        connectedEditorDevices.RemoveAt(i);
                        OnEditorDeviceDisconnected?.Invoke(connectedDevice);
                        break;
                    }
                }
            }
            else
            {
                for (var i = connectedDevices.Count - 1; i >= 0; i--)
                {
                    if (connectedDevice.deviceId == connectedDevices[i].deviceId)
                    {
                        connectedDevices.RemoveAt(i);
                        OnDeviceDisconnected?.Invoke(connectedDevice);
                        break;
                    }
                }
            }

            iMidiDevice.DisconnectDevice(connectedDevice.deviceId, connectedDevice.isEditor);
        }

        public static void DisconnectDeviceByName(string deviceName, bool editor = false)
        {
            if (iMidiDevice == null) return;

            MidiDevice disconnectDevice = null;

            if (editor)
            {
                for (var i = connectedEditorDevices.Count - 1; i >= 0; i--)
                {
                    if (deviceName == connectedEditorDevices[i].name)
                    {
                        disconnectDevice = connectedEditorDevices[i];
                        break;
                    }
                }
            }
            else
            {
                for (var i = connectedDevices.Count - 1; i >= 0; i--)
                {
                    if (deviceName == connectedDevices[i].name)
                    {
                        disconnectDevice = connectedDevices[i];
                        break;
                    }
                }
            }

            if (disconnectDevice != null) { DisconnectDevice(disconnectDevice); }
        }

        public static string GetDeviceName(int deviceIndex) => iMidiDevice == null ? "" : iMidiDevice.GetDeviceName(deviceIndex);
        public static int GetDeviceCount() => iMidiDevice?.GetDeviceCount() ?? 0;
        public static int OpenVirtualPort(string name, bool editor = false) => iMidiDevice.OpenVirtualPort(name, editor);
        public static async Task<int> OpenVirtualPortAsync(string name, bool editor = false) => await Task.Run(() => iMidiDevice.OpenVirtualPort(name, editor));
        public static void CloseVirtualPort(int deviceId, bool editor = false) { iMidiDevice.CloseVirtualPort(deviceId, editor); }
        public static void CloseVirtualPorts(bool editor = false) { iMidiDevice.CloseVirtualPorts(editor); }
        public static int GetVirtualPortCount() => iMidiDevice.GetVirtualPortCount();
        public static string GetVirtualPortName(int portIndex) => iMidiDevice.GetVirtualPortName(portIndex);
        public static int SendMidiMessage(MidiMessage m, int deviceId = -1, bool editor = false) => SendData(m.Data, deviceId, editor);
        public static int SendShortMessage(byte command, byte data1, byte data2, int deviceId = -1, bool editor = false)
        {
            if (iMidiOUTDevice == null) return -1;

            if (MIDISettings.instance.outputSettings.logAll || MIDISettings.instance.outputSettings.logShortMessages)
            {
                new byte[] { command, data1, data2 }.Log(deviceId, "MIDI OUT : ");
            }

#if UNITY_EDITOR
            if(threaded && Application.isPlaying)
#else
            if (threaded)
#endif
            {
                Messages.Enqueue(new MidiMessage { CommandAndChannel = command, Data1 = data1, Data2 = data2, DeviceId = deviceId, DataSize = 3 });
                return 1;
            }

            return iMidiOUTDevice.SendMessage(command, data1, data2, deviceId, editor);
        }

        public static int SendData(byte[] data, int deviceId = -1, bool editor = false)
        {
            if (iMidiOUTDevice == null) return -1;

            if (MIDISettings.instance.outputSettings.logAll) data.Log(deviceId, "MIDI OUT : ");

            if (threaded)
            {
                Messages.Enqueue(new MidiMessage { CommandAndChannel = 0xF0, Data = data, DeviceId = deviceId, DataSize = data.Length, Editor = editor });
                return 1;
            }

            return iMidiOUTDevice.SendData(data, deviceId, editor);
        }

        public static int GetConnectedDeviceCount() => iMidiEditorDevice?.GetConnectedDeviceCount() ?? 0;
    }
}