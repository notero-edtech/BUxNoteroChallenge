/* Copyright © Marek Ledvina, Foriero s.r.o. */

using UnityEngine;
using ForieroEngine.MIDIUnified;
using System;

[AddComponentMenu("MIDIUnified/Generators/MidiPlayMakerInput")]
public class MidiPlayMakerInput : MonoBehaviour, IMidiSender
{
    public static MidiPlayMakerInput singleton;
    public static Action<MidiPlayMakerInput> OnInitialized;

    public string id = "";
    public string Id => id;
    
    public event ShortMessageEventHandler ShortMessageEvent;

    ShortMessageEventHandler shortMessageEventHandler;

    MidiOutHelper midiOutHelper = new MidiOutHelper();

    void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("GENERATOR : MidiPlayMakerInput already in scene.");
            Destroy(this);
            return;
        }
        shortMessageEventHandler = new ShortMessageEventHandler(ShortMessageHelper);
        midiOutHelper.ShortMessageEvent += shortMessageEventHandler;
        singleton = this;

        if (OnInitialized != null)
        {
            OnInitialized(this);
        }
    }

    void OnDestroy()
    {
        singleton = null;
    }

    public ChannelEnum GetMidiChannel()
    {
        return MIDISettings.instance.playmakerSettings.midiChannel == ChannelEnum.None ? ChannelEnum.C0 : MIDISettings.instance.playmakerSettings.midiChannel;
    }

    public void SetInstrument(ProgramEnum anInstrument, ChannelEnum aChannel)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.SetInstrument(anInstrument, GetMidiChannel());
        }
    }

    public void SetInstrument(int anInstrument)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.SetInstrument(anInstrument, (int)GetMidiChannel());
        }


    }

    public void NoteOn(int aNoteIndex, int aVolume)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.NoteOn(aNoteIndex, aVolume, (int)GetMidiChannel());
        }

    }

    public void NoteOn(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, int aVolume)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.NoteOn(aNote, anAccidental, anOctave, aVolume, GetMidiChannel());
        }
    }

    public void NoteOff(int aNoteIndex)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.NoteOff(aNoteIndex);
        }

    }

    public void NoteOff(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.NoteOff(aNote, anAccidental, anOctave, GetMidiChannel());
        }

    }

    public void Pedal(int aPedal, int aValue)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.Pedal(aPedal, aValue, (int)GetMidiChannel());
        }

    }

    public void Pedal(PedalEnum aPedal, int aValue)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.Pedal(aPedal, aValue, GetMidiChannel());
        }

    }

    public void SendControl(ControllerEnum aControl, int aValue)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.SendControl(aControl, aValue, GetMidiChannel());
        }

    }

    public void SendControl(int aControl, int aValue)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.SendControl(aControl, aValue, (int)GetMidiChannel());
        }

    }

    public void AllSoundOff()
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.AllSoundOff();
        }

    }

    public void ResetAllControllers()
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            midiOutHelper.ResetAllControllers();
        }

    }

    void ShortMessageHelper(int aCommand, int aData1, int aData2, int deviceId)
    {
        SendShortMessage(aCommand, aData1, aData2, deviceId);
    }

    public void SendShortMessage(int aCommand, int aData1, int aData2, int deviceId)
    {
        if (!MIDISettings.instance.playmakerSettings.active) return;

        ShortMessageEvent?.Invoke(
                MIDISettings.instance.playmakerSettings.midiChannel == ChannelEnum.None ? aCommand : (aCommand | (int)MIDISettings.instance.playmakerSettings.midiChannel),
                aData1,
                MIDISettings.instance.playmakerSettings.useCustomVolume ? MidiConversion.GetByteVolume(MIDISettings.instance.playmakerSettings.customVolume, aData2) : aData2,
                deviceId
        );

        if (MIDISettings.instance.playmakerSettings.midiOut)
        {
            MidiOut.SendShortMessage(
                MIDISettings.instance.playmakerSettings.midiChannel == ChannelEnum.None ? aCommand : (aCommand | (int)MIDISettings.instance.playmakerSettings.midiChannel),
                aData1,
                MIDISettings.instance.playmakerSettings.useCustomVolume ? MidiConversion.GetByteVolume(MIDISettings.instance.playmakerSettings.customVolume, aData2) : aData2,
                -1
            );
        }
    }

    class MidiOutHelper
    {

        public event ShortMessageEventHandler ShortMessageEvent;

        public void SetInstrument(ProgramEnum anInstrument, ChannelEnum aChannel = ChannelEnum.C0)
        {
            SendShortMessage((int)aChannel + (int)CommandEnum.MIDI_PROGRAM_CHANGE, (int)anInstrument, 0, -1);
        }

        public void SetInstrument(int anInstrument, int aChannel = 0)
        {
            SendShortMessage(aChannel + (int)CommandEnum.MIDI_PROGRAM_CHANGE, anInstrument, 0, -1);
        }

        public void NoteOn(int aNoteIndex, int aVolume = 80, int aChannel = 0)
        {
            SendShortMessage(aChannel + (int)CommandEnum.MIDI_NOTE_ON, aNoteIndex, aVolume, -1);
        }

        public void NoteOn(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, int aVolume = 80, ChannelEnum aChannel = ChannelEnum.C0)
        {
            int noteIndex = ((int)aNote + (int)anAccidental + ((int)(anOctave == OctaveEnum.None ? OctaveEnum.Octave4 : anOctave) * 12) + 24);
            NoteOn(noteIndex, aVolume, (int)aChannel);
        }

        public void NoteOff(int aNoteIndex, int aChannel = 0)
        {
            SendShortMessage(aChannel + (int)CommandEnum.MIDI_NOTE_OFF, aNoteIndex, 0, -1);
        }

        public void NoteOff(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, ChannelEnum aChannel = ChannelEnum.C0)
        {
            int noteIndex = ((int)aNote + (int)anAccidental + ((int)(anOctave == OctaveEnum.None ? OctaveEnum.Octave4 : anOctave) * 12) + 24);
            NoteOff(noteIndex, (int)aChannel);
        }

        public void Pedal(int aPedal, int aValue, int aChannel = 0)
        {
            SendShortMessage(aChannel + (int)CommandEnum.MIDI_CONTROL_CHANGE, aPedal, aValue, -1);
        }

        public void Pedal(PedalEnum aPedal, int aValue, ChannelEnum aChannel = ChannelEnum.C0)
        {
            SendShortMessage((int)aChannel + (int)CommandEnum.MIDI_CONTROL_CHANGE, (int)aPedal, aValue, -1);
        }

        public void SendControl(ControllerEnum aControl, int aValue, ChannelEnum aChannel = ChannelEnum.C0)
        {
            SendShortMessage((int)aChannel + (int)CommandEnum.MIDI_CONTROL_CHANGE, (int)aControl, aValue, -1);
        }

        public void SendControl(int aControl, int aValue, int aChannel = 0)
        {
            SendShortMessage(aChannel + (int)CommandEnum.MIDI_CONTROL_CHANGE, aControl, aValue, -1);
        }

        public void AllSoundOff()
        {
            for (int i = 0; i < 16; i++)
            {
                SendShortMessage(i + (int)CommandEnum.MIDI_CONTROL_CHANGE, (int)ControllerEnum.AllSoundOff, 0, -1);
            }
        }

        public void ResetAllControllers()
        {
            for (int i = 0; i < 16; i++)
            {
                SendShortMessage(i + (int)CommandEnum.MIDI_CONTROL_CHANGE, (int)ControllerEnum.ResetControllers, 0, -1);
            }
        }

        public void SendShortMessage(int aCommand, int aData1, int aData2, int deviceId)
        {
            ShortMessageEvent?.Invoke(aCommand, aData1, aData2, deviceId);
        }
    }
}
