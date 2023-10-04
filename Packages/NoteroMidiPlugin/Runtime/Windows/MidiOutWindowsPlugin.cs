using System;
using System.Collections.Generic;
#if !UNITY_ANDROID || UNITY_EDITOR
using Notero.MidiPlugin.Windows.MidiDeviceAdapter;
using NAudio.Midi;
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
namespace Notero.MidiPlugin.Windows
{
    public abstract class MidiDevice
    {
        private MidiOutCapabilities m_MidiOutCapabilities;
        private MidiOut m_MidiOut;

        protected MidiDevice(MidiOutCapabilities midiOutCapabilities, MidiOut midiOut)
        {
            this.m_MidiOutCapabilities = midiOutCapabilities;
            this.m_MidiOut = midiOut;
        }

        public abstract void SendLEDControlMessage(bool isOn, int keyIndex);

        public abstract void SendVolumeControlMessage(int volumeAsPercent);
        
        public abstract void SendMidiEvent(byte[] data);

        public void Close()
        {
            m_MidiOut.Close();
        }
    }

    public class MidiOutWindowsPlugin
    {
        private List<MidiDevice> m_MidiDevices = new();

        private int m_OutputDevicceCount
        {
            get => MidiOut.NumberOfDevices;
        }
        
        public void AutoManageDevice()
        {
            for(int i = 0; i < m_OutputDevicceCount; i++)
            {
                try
                {
                    var midiOutCapabilities = MidiOut.DeviceInfo(i);
                    var midiOut = new MidiOut(i);

                    var midiDeviceAdapter = AdapterRegister.AssertDevice(midiOutCapabilities, midiOut);
                    if(midiDeviceAdapter == null) continue;

                    m_MidiDevices.Add(midiDeviceAdapter);
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogWarning($"[{nameof(MidiOutWindowsPlugin)}] Error message: {e.Message}");
                }
            }
        }

        public void SendLEDControlMessage(bool isOn, int keyIndex)
        {
            foreach(var midiDevice in m_MidiDevices)
            {
                midiDevice.SendLEDControlMessage(isOn, keyIndex);
            }
        }

        public void SendVolumeControlMessage(int volumeAsPercent)
        {
            foreach(var midiDevice in m_MidiDevices)
            {
                midiDevice.SendVolumeControlMessage(volumeAsPercent);
            }
        }

        public void SendMidiEvent(byte[] data)
        {
            foreach(var midiDevice in m_MidiDevices)
            {
                midiDevice.SendMidiEvent(data);
            }
        }

        public void CloseAllMidiDevice()
        {
            foreach(var midiDevice in m_MidiDevices)
            {
                midiDevice.Close();
            }
        }
    }
}
#endif
