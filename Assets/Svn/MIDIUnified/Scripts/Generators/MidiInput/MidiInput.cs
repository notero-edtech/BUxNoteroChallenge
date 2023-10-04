/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;

[AddComponentMenu("MIDIUnified/Generators/MidiInput")]
public class MidiInput : MonoBehaviour, IMidiSender
{
    public static MidiInput singleton;
    public static Action<MidiInput> OnInitialized;

    public string id = "";
    public string Id => id;
    public event ShortMessageEventHandler ShortMessageEvent;
    public delegate void MidiBytesEventHandler(byte[] bytes, int deviceId);
    public static event MidiBytesEventHandler MidiBytesEvent;

    private int _volume = 0;
    private int _command = 0;
    
    private void Awake()
    {
        if (singleton != null) {
            Debug.LogError("GENERATOR : MidiInput already in scene");
            Destroy(this);
            return;
        }

        singleton = this;
        OnInitialized?.Invoke(this);
    }

    private void OnDestroy() { singleton = null; }
       
    private void Update() {    
        if(MIDISettings.instance.inputSettings.update == MIDISettings.MidiInputSettings.UpdateEnum.Update) ProcessMidiInMessages();
    }

    private void LateUpdate() {
        if (MIDISettings.instance.inputSettings.update == MIDISettings.MidiInputSettings.UpdateEnum.LateUpdate) ProcessMidiInMessages();
    }

    private void FixedUpdate() {
        if (MIDISettings.instance.inputSettings.update == MIDISettings.MidiInputSettings.UpdateEnum.FixedUpdate) ProcessMidiInMessages();
    }

    private void ProcessMidiMessage(MidiMessage midiMessage)
    {
        if (!MIDISettings.instance.inputSettings.active) return;
                                        
        if (MIDISettings.instance.inputSettings.logAll) midiMessage.Log("MIDI IN : ");

        MidiOut.IsInfiniteLoop(midiMessage.CommandAndChannel, midiMessage.Data1, midiMessage.Data2);
                
        switch (midiMessage.CommandAndChannel)
        {
            // NOTE_ON, NOTE_OFF //
            case <= 0b1001_1111 and >= 0b1000_0000 when midiMessage.DataSize == 3:
                if (MIDISettings.instance.inputSettings.logShortMessages) midiMessage.Log("MIDI IN SHORT : ");

                if (midiMessage.CommandAndChannel.ToMidiCommand() == 0x90 && midiMessage.Data2 == 0) { midiMessage.CommandAndChannel = (byte)(midiMessage.CommandAndChannel.ToMidiChannel() + 0x80); }

                if (MIDISettings.instance.inputSettings.useCustomVolume) { _volume = MidiConversion.GetMidiVolume(MIDISettings.instance.inputSettings.customVolume); }
                else { _volume = (int)Mathf.Clamp(MIDISettings.instance.inputSettings.multiplyVolume * midiMessage.Data2, 0, 127); }

                if (MIDISettings.instance.inputSettings.midiChannel == ChannelEnum.None) { _command = midiMessage.CommandAndChannel; }
                else { _command = (int)MIDISettings.instance.inputSettings.midiChannel + midiMessage.CommandAndChannel.ToMidiCommand(); }

                if (MIDISettings.instance.inputSettings.midiOut) { MidiOut.SendShortMessage(_command, midiMessage.Data1, _volume, -1); }

                ShortMessageEvent?.Invoke(_command, midiMessage.Data1, _volume, midiMessage.DeviceId);
                break;
            // CC MESSAGES //
            case <= 0b1110_1111 and >= 0b1010_0000 when midiMessage.DataSize == 3:
                if (MIDISettings.instance.inputSettings.logShortMessages) midiMessage.Log("MIDI IN SHORT : ");

                if (MIDISettings.instance.inputSettings.midiChannel == ChannelEnum.None) { _command = midiMessage.CommandAndChannel; }
                else { _command = (int)MIDISettings.instance.inputSettings.midiChannel + midiMessage.CommandAndChannel.ToMidiCommand(); }

                if (MIDISettings.instance.inputSettings.midiOut) { MidiOut.SendShortMessage(_command, midiMessage.Data1, midiMessage.Data2, -1); }

                ShortMessageEvent?.Invoke(_command, midiMessage.Data1, midiMessage.Data2, midiMessage.DeviceId);
                break;
            case <= 0b1111_1111 and >= 0b1111_0000:
                if (MIDISettings.instance.inputSettings.logSystemMessages) midiMessage.Log("MIDI IN SYSTEM : ");
                break;
        }
                
        MidiBytesEvent?.Invoke(midiMessage.Data, midiMessage.DeviceId);
    }

    private void ProcessMidiInMessages()
    {
        if (!MIDI.initialized) return;
        while (MidiINPlugin.PopMessage(out var m) == 1) { if (!MIDISettings.instance.inputSettings.cleanBuffer) { ProcessMidiMessage(m); } }
        MIDISettings.instance.inputSettings.cleanBuffer = false;
    }
}
