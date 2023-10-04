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
    public static partial class MidiINPlugin
    {
        public static IMidiINDevice iMidiInDevice;
        public static IMidiEditorDevice iMidiEditorDevice;
        
#if UNITY_EDITOR
        [InitializeOnLoad] private class InitEditor { static InitEditor() { if (!Application.isPlaying) { Init(); } } }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitPlayer()
        {
            var stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;            
            if (Application.isPlaying) { Init(); }
            if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (MidiPlugin.IN - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

        public static void Init()
        {
            DSP.Init();
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            var device = new MidiINDeviceSTANDALONE();
#elif UNITY_IOS
			var device = new MidiINDeviceIOS();
#elif UNITY_WSA
			var device = new MidiINDeviceWSA();
#elif UNITY_WEBGL
			var device = new MidiINDeviceWEBGL();
#elif UNITY_ANDROID
            AndroidJNI.AttachCurrentThread();
            var device = new MidiINDeviceANDROID();
#else
			var device = new MidiINDeviceNONE ();
#endif
            iMidiDevice = device as IMidiDevice;
            iMidiInDevice = device as IMidiINDevice;
            iMidiEditorDevice = device as IMidiEditorDevice;

            initialized = iMidiDevice.Init();

            if (!initialized) { Debug.LogError("MidiINDevice not initialized"); return; }

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
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
            EditorPrefs.SetString("MIDI_IN_NAMES", s);
        }

        public static void RestoreEditorConnections()
        {
            var names = EditorPrefs.GetString("MIDI_IN_NAMES", "").Trim();
            var midiInNames = string.IsNullOrEmpty(names) ? Array.Empty<string>() : names.Split(';');

            foreach (var midiInName in midiInNames)
            {
                if (ConnectDeviceByName(midiInName, true) != null) continue;
                if (Debug.isDebugBuild) { Debug.LogError("Could not conned midi in device : " + midiInName); }
            }
        }
#endif

        public static void StoreConnections()
        {
            var connectionNames = new List<string>();
            foreach (var device in connectedDevices) { connectionNames.Add(device.name); }
            var s = string.Join(";", connectionNames.ToArray());
            PlayerPrefs.SetString("MIDI_IN_NAMES", s);
        }

        public static void RestoreConnections()
        {
            var names = PlayerPrefs.GetString("MIDI_IN_NAMES", "").Trim();
            var midiInNames = string.IsNullOrEmpty(names) ? new string[0] : names.Split(';');

            foreach (var midiInName in midiInNames)
            {
                if (ConnectDeviceByName(midiInName) != null) continue;
                if (Debug.isDebugBuild) { Debug.LogError("Could not connect midi in device : " + midiInName); }
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
                if (!iMidiEditorDevice.GetConnectedDeviceIsEditor(i)) continue;
                var md = new MidiDevice
                {
                    name = iMidiEditorDevice.GetConnectedDeviceName(i),
                    deviceId = iMidiEditorDevice.GetConnectedDeviceId(i),
                    isEditor = true
                };
                connectedEditorDevices.Add(md);
            }
        }

        public static bool Initialized() => iMidiDevice != null && initialized;

        public static MidiDevice ConnectDevice(int deviceIndex, bool editor = false)
        {
            if (iMidiDevice == null) return null;

            foreach (var md in MidiOUTPlugin.connectedDevices)
            {
                if (md.name == iMidiDevice.GetDeviceName(deviceIndex) && MIDISettings.instance.midiInOutExclusive)
                {
                    Debug.LogError("Preventing infinite loop. To have same device connected as IN and OUT is not allowed.");
                    return null;
                }
            }

            var deviceId = iMidiDevice.ConnectDevice(deviceIndex, editor);

            if (editor)
            {
                foreach (var cd in connectedEditorDevices)
                {
                    if (deviceId != cd.deviceId) continue;
                    Debug.LogError("Editor device already connected");
                    return cd;
                }
            }
            else
            {
                foreach (var cd in connectedDevices)
                {
                    if (deviceId != cd.deviceId) continue;
                    Debug.LogError("Device already connected");
                    return cd;
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

        public static MidiDevice ConnectDeviceByName(string deviceName, bool editor = false)
        {
            if (iMidiDevice == null) return null;

            foreach (var md in MidiOUTPlugin.connectedDevices)
            {
                if (md.name == deviceName && MIDISettings.instance.midiInOutExclusive)
                {
                    Debug.LogError("Preventing infinite loop. To have same device connected as IN and OUT is not allowed.");
                    return null;
                }
            }

            for (int i = 0; i < GetDeviceCount(); i++) { if (deviceName == GetDeviceName(i)) { return ConnectDevice(i, editor); } }
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

        public static void DisconnectDevices(bool editor = false)
        {
            if (iMidiDevice == null) return;
            if (editor) { connectedEditorDevices = new List<MidiDevice>(); }
            else { connectedDevices = new List<MidiDevice>(); }
            iMidiDevice.DisconnectDevices(editor);
        }

        public static void DisconnectDeviceByName(string deviceName, bool editor = false)
        {
            if (iMidiDevice == null) return;

            MidiDevice disconnectDevice = null;
            if (editor)
            {
                for (var i = connectedEditorDevices.Count - 1; i >= 0; i--)
                {
                    if (deviceName != connectedEditorDevices[i].name) continue;
                    disconnectDevice = connectedEditorDevices[i];
                    break;
                }
            }
            else
            {
                for (var i = connectedDevices.Count - 1; i >= 0; i--)
                {
                    if (deviceName != connectedDevices[i].name) continue;
                    disconnectDevice = connectedDevices[i];
                    break;
                }
            }

            if (disconnectDevice != null) { DisconnectDevice(disconnectDevice); }
        }

        public static string GetDeviceName(int deviceIndex) => iMidiDevice?.GetDeviceName(deviceIndex) ?? "";
        public static int GetDeviceCount() =>  iMidiDevice?.GetDeviceCount() ?? 0;
        public static int OpenVirtualPort(string name, bool editor = false) => iMidiDevice?.OpenVirtualPort(name, editor) ?? -1;
        public static async Task<int> OpenVirtualPortAsync(string name, bool editor = false) => await Task.Run(() => iMidiDevice.OpenVirtualPort(name, editor));
        public static void CloseVirtualPort(int deviceId, bool editor = false) => iMidiDevice?.CloseVirtualPort(deviceId, editor);
        public static void CloseVirtualPorts(bool editor = false) => iMidiDevice?.CloseVirtualPorts(editor);
        public static int GetVirtualPortCount() => iMidiDevice?.GetVirtualPortCount() ?? 0;
        public static string GetVirtualPortName(int portIndex) => iMidiDevice?.GetVirtualPortName(portIndex) ?? "";
        public static int PopMessage(out MidiMessage midiMessage, bool editor = false)
        {
            if (iMidiInDevice == null) { midiMessage = new MidiMessage(); return 0; }
#if UNITY_EDITOR
            if (threaded && Application.isPlaying)
#else
            if (threaded)
#endif
            {
                midiMessage = new MidiMessage();
                if (editor) return MessagesEditor.Dequeue(ref midiMessage) ? 1 : 0;
                else return Messages.Dequeue(ref midiMessage) ? 1 : 0;
            } else return iMidiInDevice.PopMessage(out midiMessage, editor);
        }

        public static int GetConnectedDeviceCount() => iMidiEditorDevice?.GetConnectedDeviceCount() ?? 0;
    }
}