#if !UNITY_ANDROID || UNITY_EDITOR
using Notero.MidiPlugin.Windows;
#endif
using UnityEngine;

namespace Notero.MidiPlugin
{
    public interface IMidiPlugin
    {
        void DiscoverAllMidiInputPortByDevice();

        void DiscoverMidiInputPortByDeviceName(string name);

        bool IsReady();

        void SendLedControlMessage(bool isOn, int keyIndex);

        void SendVolumeControlMessage(int valueAsPercent);

        bool isInteractiveAfterMute(string name);

        void AutoManageDevice();

        void SendMidiEvent(byte[] data);

        void CloseAllMidiDevice();
    }


#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// MidiPlugin for Android
    /// </summary>
    class MidiPluginNativeBridge : IMidiPlugin
    {
        private const string UnityPlayerClass = "com.unity3d.player.UnityPlayer";
        private const string NativeClass = "co.notero.midiplugin.MidiPlugin";

        private AndroidJavaObject _midiPlugin;

        public MidiPluginNativeBridge()
        {
            _midiPlugin = new AndroidJavaObject(NativeClass, this.GetJavaPluginContext());
        }

        private AndroidJavaObject GetJavaPluginContext()
        {
            var unityPlayer = new AndroidJavaClass(UnityPlayerClass);
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            return context;
        }

        public void DiscoverAllMidiInputPortByDevice()
        {
            if(_midiPlugin == null) return;

            _midiPlugin.Call("discoverAllMidiInputPort");
        }

        public void DiscoverMidiInputPortByDeviceName(string name)
        {
            if(_midiPlugin == null) return;

            _midiPlugin.Call("discoverMidiInputPort", name);
        }

        public bool IsReady()
        {
            if(_midiPlugin == null) return true;

            return _midiPlugin.Call<bool>("IsReady");
        }

        public void SendLedControlMessage(bool isOn, int keyIndex)
        {
            if(_midiPlugin == null) return;

            _midiPlugin.Call("sendLEDControlMessage", isOn, keyIndex);
        }

        public void SendVolumeControlMessage(int valueAsPercent)
        {
            if(_midiPlugin == null) return;

            _midiPlugin.Call("sendVolumeControlMessage", valueAsPercent);
        }

        public bool isInteractiveAfterMute(string name)
        {
            if(_midiPlugin == null) return true;

            return _midiPlugin.Call<bool>("isInteractiveAfterMute", name);
        }

        public void AutoManageDevice()
        {
            if(_midiPlugin == null) return;

            _midiPlugin.Call("autoManageDevice");
        }

        public void SendMidiEvent(byte[] data)
        {
            if(_midiPlugin == null) return;
            
            _midiPlugin.Call("sendMidiEvent", data);
        }

        public void CloseAllMidiDevice()
        {
            if(_midiPlugin == null) return;

            _midiPlugin.Call("closeAllMidiDevice");
        }
    }
#elif !UNITY_ANDROID || UNITY_EDITOR
    /// <summary>
    /// MidiPlugin for Unsupported
    /// </summary>
    class MidiPluginNativeBridge : IMidiPlugin
    {
        private MidiOutWindowsPlugin m_MidiOutWindowsPlugin;

        public MidiPluginNativeBridge()
        {
            m_MidiOutWindowsPlugin = new();
        }

        public void DiscoverAllMidiInputPortByDevice() { }

        public void DiscoverMidiInputPortByDeviceName(string name) { }

        public bool IsReady()
        {
            return true;
        }

        public void SendLedControlMessage(bool isOn, int keyIndex)
        {
            if(m_MidiOutWindowsPlugin == null) return;

            m_MidiOutWindowsPlugin.SendLEDControlMessage(isOn, keyIndex);
        }

        public void SendVolumeControlMessage(int valueAsPercent)
        {
            if(m_MidiOutWindowsPlugin == null) return;

            m_MidiOutWindowsPlugin.SendVolumeControlMessage(valueAsPercent);
        }

        public bool isInteractiveAfterMute(string name)
        {
            return true;
        }

        public void AutoManageDevice()
        {
            if(m_MidiOutWindowsPlugin == null) return;

            m_MidiOutWindowsPlugin.AutoManageDevice();
        }

        public void SendMidiEvent(byte[] data)
        {
            if(m_MidiOutWindowsPlugin == null) return;

            m_MidiOutWindowsPlugin.SendMidiEvent(data);
        }

        public void CloseAllMidiDevice()
        {
            if(m_MidiOutWindowsPlugin == null) return;

            m_MidiOutWindowsPlugin.CloseAllMidiDevice();
        }
    }
#endif

    public abstract class MidiPlugin
    {
        public static IMidiPlugin Instance = new MidiPluginNativeBridge();
    }
}
