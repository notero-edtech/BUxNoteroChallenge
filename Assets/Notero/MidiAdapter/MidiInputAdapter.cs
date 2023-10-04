using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using Notero.Unity.UI.VirtualPiano;
using Notero.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notero.MidiAdapter.Reconnection;
using UnityEngine;

namespace Notero.MidiAdapter
{
    public interface IMidiKeyCallable
    {
        public delegate void NoteEventHandler(int midiId, int octaveIndex, int noteNumber);

        public event NoteEventHandler NoteOnEvent;
        public event NoteEventHandler NoteOffEvent;
    }

    public delegate void ShortMessageEventHandler(int aCommand, int aData1, int aData2, int deviceId);

    public interface IMidiEvents
    {
        event ShortMessageEventHandler ShortMessageEvent;
    }

    public class MidiInputAdapter : MonoSingleton<MidiInputAdapter>, IMidiEvents, IMidiKeyCallable
    {
        private MidiReconnectionConfig m_MidiReconnectionConfig;

        public bool IsDeviceConnected => !m_MidiReconnection.Enable;

        public bool IsReconnecting => m_MidiReconnection.Enable;

        public MidiReconnection MidiReconnection => m_MidiReconnection;

        protected MidiReconnection m_MidiReconnection;

        /// <summary>
        /// On key pressed
        /// </summary>
        public event IMidiKeyCallable.NoteEventHandler NoteOnEvent;

        /// <summary>
        /// On key released
        /// </summary>
        public event IMidiKeyCallable.NoteEventHandler NoteOffEvent;

        /// <summary>
        /// On midi keyboard send message signal out
        /// </summary>
        public event ShortMessageEventHandler ShortMessageEvent;

        private HashSet<int> m_KeyIndexLedOnList = new HashSet<int>();
        private HashSet<int> m_MidiIdPressedList = new HashSet<int>();

        protected readonly MidiEvents m_MidiEvents = new MidiEvents();

        /// <summary>
        /// Global midi id for C3 is 36 but unified midi id for C3 is 48, 
        /// so we have to convert unified midi id to global one by minus 12
        /// </summary>
        private const string m_MidiDevicesStoreKey = "MIDI_IN_NAMES";

        private const int m_MidiIdOffset = 12;
        private const int m_DefaultMidiChannel = 0;
        private const int m_DefaultUnmuteVoloumePercent = 50;

        private bool m_IsMute;

        public override void Awake()
        {
            base.Awake();
            m_MidiReconnectionConfig = Resources.Load<MidiReconnectionConfig>("MidiReconnectionConfig");
            m_MidiReconnection = new MidiReconnection(m_MidiReconnectionConfig);
            StartCoroutine(StartConnectMidi());

            MidiINPlugin.OnDeviceConnected += OnDeviceConnected;
            MidiINPlugin.OnDeviceDisconnected += OnDeviceDisconnected;
            MidiOut.ShortMessageEvent += OnShortMessageEvent;
            m_MidiEvents.NoteOnEvent += OnNoteOnEvent;
            m_MidiEvents.NoteOffEvent += OnNoteOffEvent;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DisconnectMidiDevice(false);

            MidiINPlugin.OnDeviceConnected -= OnDeviceConnected;
            MidiINPlugin.OnDeviceDisconnected -= OnDeviceDisconnected;
            MidiOut.ShortMessageEvent -= OnShortMessageEvent;
            m_MidiEvents.NoteOnEvent -= OnNoteOnEvent;
            m_MidiEvents.NoteOffEvent -= OnNoteOffEvent;
        }

        public virtual void Update()
        {
            var isReady = IsMidiPluginReady();
            if(!isReady) return;

            m_MidiReconnection?.UpdateMidiReconnection(Time.deltaTime);
            ProceedKeyboardInput();
        }

        public void SetComputerKeyboardMidiInputEnable(bool isEnable)
        {
            MIDISettings.instance.keyboardSettings.customVolume = isEnable ? 1 : 0.004f;
        }

        private void ProceedKeyboardInput()
        {
            if(!MIDISettings.instance.keyboardSettings.active) return;

            if(Input.GetKeyDown(KeyCode.Z) && MIDISettings.instance.keyboardSettings.updateKeyboardOctave)
            {
                if(MIDISettings.instance.keyboardSettings.muteTonesWhenChangingOctave && MidiKeyboardInput.singleton) MidiKeyboardInput.singleton.MuteTones();

                MIDISettings.instance.keyboardSettings.keyboardOctave--;
                ForceAllRelease();
            }

            if(Input.GetKeyDown(KeyCode.X) && MIDISettings.instance.keyboardSettings.updateKeyboardOctave)
            {
                if(MIDISettings.instance.keyboardSettings.muteTonesWhenChangingOctave && MidiKeyboardInput.singleton) MidiKeyboardInput.singleton.MuteTones();

                MIDISettings.instance.keyboardSettings.keyboardOctave++;
                ForceAllRelease();
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.DeleteKey(m_MidiDevicesStoreKey);
        }

        public void ConnectAllMidiDevices()
        {
            // We cannot connect midi IN and OUT in the same time and same device
            foreach(string deviceName in MidiINPlugin.deviceNames)
            {
                if(!IsConnected(deviceName, MidiPluginType.IN))
                {
                    MidiOUTPlugin.DisconnectDeviceByName(deviceName);
                    MidiINPlugin.ConnectDeviceByName(deviceName);
                }
            }
        }

        private IEnumerator StartConnectMidi()
        {
            yield return new WaitUntil(IsMidiPluginReady);

            ConnectAllMidiDevices();
        }

        public void OnIsMuteChange(bool isMute)
        {
            m_IsMute = isMute;
            SetPianoVolume(isMute ? 0 : m_DefaultUnmuteVoloumePercent);
        }

        protected bool IsMidiPluginReady()
        {
            return MidiINPlugin.initialized && MidiOUTPlugin.initialized;
        }

        protected virtual void OnDeviceConnected(MidiDevice device)
        {
            if(device.deviceId != -1)
            {
                m_MidiReconnection.Enable = false;
                ConnectToInputPort();
                SetPianoVolume(m_IsMute ? 0 : m_DefaultUnmuteVoloumePercent);
                _ = ResetAllLed(SetAllLedOn);
            }

            Debug.Log($"{nameof(OnDeviceConnected)} name: {device.name}, index: {device.deviceId}");
        }

        protected virtual void OnDeviceDisconnected(MidiDevice device)
        {
            Debug.Log($"{nameof(OnDeviceDisconnected)} name: {device.name}, index: {device.deviceId}");
        }

        private void DisconnectMidiDevice(bool isEditor)
        {
            MidiINPlugin.DisconnectDevices(isEditor);
            MidiINPlugin.StoreConnections();
            MidiPlugin.MidiPlugin.Instance.CloseAllMidiDevice();
            Debug.Log($"[{nameof(MidiInputAdapter)}] {nameof(DisconnectMidiDevice)}");
        }

        private bool IsConnected(string deviceName, MidiPluginType type)
        {
            List<MidiDevice> connectedDevices = type == MidiPluginType.IN
                ? MidiINPlugin.connectedDevices
                : MidiOUTPlugin.connectedDevices;
            return connectedDevices.Any(inDevice => inDevice.name == deviceName);
        }

        protected virtual void OnNoteOnEvent(int aMidiId, int aValue, int aChannel)
        {
            if(aChannel == m_DefaultMidiChannel) OnNoteEvent(aMidiId, NoteOnEvent);
            m_MidiIdPressedList.Add(aMidiId);
        }

        protected virtual void OnNoteOffEvent(int aMidiId, int aValue, int aChannel)
        {
            if(aChannel == m_DefaultMidiChannel) OnNoteEvent(aMidiId, NoteOffEvent);
            if(m_MidiIdPressedList.Contains(aMidiId)) m_MidiIdPressedList.Remove(aMidiId);
        }

        private void OnNoteEvent(int aMidiId, IMidiKeyCallable.NoteEventHandler noteEvent)
        {
            aMidiId -= m_MidiIdOffset;
            int octaveIndex = VirtualPianoHelper.GetOctaveIndex(aMidiId);
            int noteNumber = VirtualPianoHelper.GetKeyIndex(aMidiId);

            noteEvent?.Invoke(aMidiId, octaveIndex, noteNumber);
        }

        public void ConnectToInputPort()
        {
            MidiPlugin.MidiPlugin.Instance.AutoManageDevice();
        }

        public void OnMute()
        {
            Debug.Log("Mute");
            MidiPlugin.MidiPlugin.Instance.SendVolumeControlMessage(0);
        }

        public void OnUnmute()
        {
            Debug.Log("Unmute");
            MidiPlugin.MidiPlugin.Instance.SendVolumeControlMessage(100);
        }

        public void OnLedOn()
        {
            Debug.Log("LED On");
            MidiPlugin.MidiPlugin.Instance.SendLedControlMessage(true, 0x40);
        }

        public void OnLedOff()
        {
            Debug.Log("LED Off");
            MidiPlugin.MidiPlugin.Instance.SendLedControlMessage(false, 0x40);
        }

        public void SendMidiEvent(byte[] data)
        {
            MidiPlugin.MidiPlugin.Instance.SendMidiEvent(data);
        }

        public void SetLedOn(int keyIndex, bool cache = false)
        {
            keyIndex += m_MidiIdOffset;

            MidiPlugin.MidiPlugin.Instance.SendLedControlMessage(true, keyIndex);

            if(cache) m_KeyIndexLedOnList.Add(keyIndex);
        }

        public void SetLedOff(int keyIndex)
        {
            keyIndex += m_MidiIdOffset;

            MidiPlugin.MidiPlugin.Instance.SendLedControlMessage(false, keyIndex);
            if(m_KeyIndexLedOnList.Contains(keyIndex)) m_KeyIndexLedOnList.Remove(keyIndex);
        }

        public void SetAllLedOff()
        {
            foreach(var keyIndex in m_KeyIndexLedOnList)
            {
                MidiPlugin.MidiPlugin.Instance.SendLedControlMessage(false, keyIndex);
            }

            m_KeyIndexLedOnList.Clear();
        }

        public void SetAllLedOn()
        {
            foreach(var keyIndex in m_KeyIndexLedOnList)
            {
                MidiPlugin.MidiPlugin.Instance.SendLedControlMessage(true, keyIndex);
            }
        }

        public async Task ResetAllLed(Action onFinish)
        {
            for(int i = 0; i <= 127; i++)
            {
                await Task.Delay(50);
                MidiPlugin.MidiPlugin.Instance.SendLedControlMessage(false, i);
            }

            onFinish?.Invoke();
        }

        public bool isInteractiveAfterMute(string deviceName)
        {
            return MidiPlugin.MidiPlugin.Instance.isInteractiveAfterMute(deviceName);
        }

        private void SetPianoVolume(int valuePercent)
        {
            MidiPlugin.MidiPlugin.Instance.SendVolumeControlMessage(valuePercent);
        }

        protected virtual void OnShortMessageEvent(int aCommand, int aData1, int aData2, int deviceId)
        {
            m_MidiEvents.AddShortMessage(aCommand, aData1, aData2);
        }

        private void ForceAllRelease()
        {
            foreach(var midiId in m_MidiIdPressedList.ToList()) MidiOut.NoteOff(midiId);

            m_MidiIdPressedList.Clear();
        }

        private enum MidiPluginType
        {
            IN,
            OUT
        }
    }
}