/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System;
using System.Linq;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.MIDIUnified.Synthesizer;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public static partial class MidiOut
    {
        #region PRIVATE
        private static int lastMidiChannelCommand = 0;
        private static int lastMidiChannelData1 = 0;
        private static int lastMidiChannelData2 = 0;
        private static int lastMidiChannelInfinityCounter = 0;

        private struct NoteCache { public int index; public bool on; }
        private class ChannelCache { public readonly NoteCache[] notes = new NoteCache[128]; }

        private static readonly ChannelCache[] ChannelCaches = new ChannelCache[16] 
        {
            new (), new (), new (), new (), new (), new (), new (), new (),
            new (), new (), new (), new (), new (), new (), new (), new (),
        };
        #endregion
        
        //0 = everything//
        //1 = nothing//
        //2,4,8//
        public static int channelMask = -1;
        public static int synthChannelMask = -1;
        public static bool fireMidiOutEvents = true;

        public static bool ApplyChannelVolumes { get; set; } =  false;
        public static readonly float[] Volumes = new float[16] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
        
        public static bool MuteMessages { get; set; } = false;
        public static bool IgnoreProgramMessages { get; set; } = false;

        private static readonly ProgramEnum[] ProgramEnums = new ProgramEnum[16] {
            ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano,
            ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano, ProgramEnum.AcousticGrandPiano
        };

        public static event ShortMessageEventHandler ShortMessageEvent;
        
        public static int SetInstrument(ProgramEnum anInstrument, ChannelEnum aChannel = ChannelEnum.C0)
            => aChannel == ChannelEnum.None ? 0 : SetInstrument((int)anInstrument, (int)aChannel);
        
        public static int SetInstrument(int anInstrument, int aChannel = 0, int deviceId = -1)
            => SendShortMessage(aChannel + (int)CommandEnum.MIDI_PROGRAM_CHANGE, anInstrument, 0, deviceId);
        
        public static void InitPercussion(bool all = false)
        {
            if (all)
            {
                foreach (var p in Enum.GetValues(typeof(PercussionEnum))) { SchedulePercussion((PercussionEnum)p, 1, 0.1f, false); }
            }
            else
            {
                SchedulePercussion(MIDIPercussionSettings.GetPercussionEnum(BeatType.Heavy), 1, 0.1f, false);
                SchedulePercussion(MIDIPercussionSettings.GetPercussionEnum(BeatType.Light), 1, 0.1f, false);
                SchedulePercussion(MIDIPercussionSettings.GetPercussionEnum(BeatType.Subdivision), 1, 0.1f, false);
            }
        }

        public static double SchedulePercussion(PercussionEnum aPercussion, int anAttack = 80, double scheduleTime = 0, bool absoluteDspTime = false)
            => MIDIPercussion.SchedulePercussion(aPercussion, anAttack, scheduleTime, absoluteDspTime);

        public static int Percussion(PercussionEnum aPercussion, int anAttack = 80)
            => NoteOn((int)aPercussion, anAttack, 9);

        public static void NoteDispatch(int aNoteIndex, float aDuration = 0f, float aDelay = 0f, int anAttack = 80, int aChannel = 0, int deviceId = -1, Action started = null, Action finished = null)
            => MidiDispatcher.DispatchNote(aNoteIndex, anAttack, aChannel, deviceId, aDuration, aDelay, started, finished);
        
        public static void NoteDispatch(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, float aDuration = 0f, float aDelay = 0f, int anAttack = 80, ChannelEnum aChannel = ChannelEnum.C0, int deviceId = -1)
        {
            if (aChannel == ChannelEnum.None) return;
            var noteIndex = MidiConversion.MidiIndex(aNote, anAccidental, anOctave);
            NoteDispatch(noteIndex, aDuration, aDelay, anAttack, (int)aChannel);
        }

        public static int NoteOn(int aNoteIndex, int anAttack = 80, int aChannel = 0, int deviceId = -1, bool editor = false)
            => SendShortMessage(aChannel, (int)CommandEnum.MIDI_NOTE_ON, aNoteIndex, anAttack, deviceId, editor);
        
        public static int NoteOn(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, int anAttack = 80, ChannelEnum aChannel = ChannelEnum.C0, int deviceId = -1, bool editor = false)
        {
            if (aChannel == ChannelEnum.None) return 0;
            var noteIndex = MidiConversion.MidiIndex(aNote, anAccidental, anOctave);
            return NoteOn(noteIndex, anAttack, (int)aChannel, deviceId, editor);
        }
        
        public static int NoteOff(int aNoteIndex, int aChannel = 0, int deviceId = -1, bool editor = false)
            => SendShortMessage((int)CommandEnum.MIDI_NOTE_OFF, aChannel, aNoteIndex, 0, deviceId, editor);
        
        public static int NoteOff(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, ChannelEnum aChannel = ChannelEnum.C0, int deviceId = -1, bool editor = false)
        {
            if (aChannel == ChannelEnum.None) return 0;
            var noteIndex = MidiConversion.MidiIndex(aNote, anAccidental, anOctave);
            return NoteOff(noteIndex, (int)aChannel, deviceId, editor);
        }

        public static int Pedal(int aPedal, int aValue, int aChannel = 0, int deviceId = -1)
            => SendShortMessage((int)CommandEnum.MIDI_CONTROL_CHANGE, aChannel, aPedal, aValue, deviceId);
        
        public static int Pedal(PedalEnum aPedal, int aValue, ChannelEnum aChannel = ChannelEnum.C0, int deviceId = -1)
        {
            if (aChannel == ChannelEnum.None) return 0;
            return Pedal((int)aPedal, aValue, (int)aChannel, deviceId);
        }

        public static int SendControl(ControllerEnum aControl, int aValue, ChannelEnum aChannel = ChannelEnum.C0, int deviceId = -1)
        {
            if (aChannel == ChannelEnum.None && aControl != ControllerEnum.None) return 0;
            return SendControl((int)aControl, aValue, (int)aChannel, deviceId);
        }

        public static int SendControl(int aControl, int aValue, int aChannel = 0, int deviceId = -1)
            => SendShortMessage((int)CommandEnum.MIDI_CONTROL_CHANGE, aChannel, aControl, aValue, deviceId);
        
        public static void ChannelSoundsOff(int aChannel, int deviceId = -1)
        {
            var index = 0;
            foreach (NoteCache n in ChannelCaches[aChannel].notes)
            {
                if (n.on) NoteOff(index, aChannel);
                index++;
            }
            SendShortMessage((int)CommandEnum.MIDI_CONTROL_CHANGE, aChannel, (int)ControllerEnum.AllSoundOff, 0, deviceId);
        }

        public static void ChannelPedalsOff(int aChannel, int deviceId = -1)
        {
            Pedal((int)PedalEnum.DamperPedal, 0, aChannel, deviceId);
            Pedal((int)PedalEnum.Sostenuto, 0, aChannel, deviceId);
            Pedal((int)PedalEnum.SoftPedal, 0, aChannel, deviceId);
        }

        public static void AllPedalsOff(int deviceId = -1)
        {
            for (var i = 0; i < 16; i++)
            {
                Pedal((int)PedalEnum.DamperPedal, 0, i, deviceId);
                Pedal((int)PedalEnum.Sostenuto, 0, i, deviceId);
                Pedal((int)PedalEnum.SoftPedal, 0, i, deviceId);
            }
        }

        public static void AllSoundOff(int deviceId = -1)
        {
            if (MIDISynthSettings.instance.active)
            {
                var channel = 0;
                foreach (var ch in ChannelCaches)
                {
                    var index = 0;
                    foreach (var n in ch.notes)
                    {
                        if (n.on) NoteOff(index, channel, deviceId);
                        index++;
                    }
                    channel++;
                }
            }
            else
            {
                for (var i = 0; i < 16; i++) { SendShortMessage((int)CommandEnum.MIDI_CONTROL_CHANGE, i, (int)ControllerEnum.AllSoundOff, 0, deviceId); }
            }
        }

        public static void ResetAllControllers(int deviceId = -1)
        {
            for (var i = 0; i < 16; i++) { SendShortMessage((int)CommandEnum.MIDI_CONTROL_CHANGE, 0, (int)ControllerEnum.ResetControllers, 0, deviceId); }
        }

        public static int SendShortMessage(int aCommand, int aChannel, int aData1, int aData2, int deviceId, bool editor = false)
            => SendShortMessage(aChannel + aCommand, aData1, aData2, deviceId, editor);

        public static void IsInfiniteLoop(int aChannelCommand, int aData1, int aData2)
        {
            if (!MIDISettings.instance.watchInfiniteLoop) return;
            
            if (lastMidiChannelCommand == aChannelCommand && lastMidiChannelData1 == aData1 && lastMidiChannelData2 == aData2) lastMidiChannelInfinityCounter++;
            else lastMidiChannelInfinityCounter = 0;

            if (lastMidiChannelInfinityCounter <= MIDISettings.instance.infiniteLoopThreshold) return;
            
            MidiINPlugin.DisconnectDevices();
            MidiOUTPlugin.DisconnectDevices();
            lastMidiChannelInfinityCounter = 0;
            Debug.LogError(
                "Detected infinite midi loop. Disconnecting all MidiIn and MidiOut devices \n" +
                lastMidiChannelCommand.ToString("X2") + " " +
                lastMidiChannelData1.ToString("X2") + " " +
                lastMidiChannelData2.ToString("X2") + " "
            );
        }
        
        public static int SendShortMessage(int aChannelCommand, int aData1, int aData2, int deviceId, bool editor = false)
        {
            if (!MIDISettings.instance.outputSettings.active) return 0;

            IsInfiniteLoop(aChannelCommand, aData1, aData2);

            lastMidiChannelCommand = aChannelCommand;
            lastMidiChannelData1 = aData1;
            lastMidiChannelData2 = aData2;

            var channel = (aChannelCommand & 0x0F);
            var command = (aChannelCommand & 0xF0);
          
            if (command == (int)CommandEnum.MIDI_PROGRAM_CHANGE)
            {
                ProgramEnums[channel] = (ProgramEnum)aData1;
                if (IgnoreProgramMessages) return 0;
            }

            if (command.IsToneON())
            {
                ChannelCaches[channel].notes[aData1].on = true;
                if (ApplyChannelVolumes) { aData2 = (int)(Volumes[channel] * aData2); }
            } else if (command.IsToneOFF()) { ChannelCaches[channel].notes[aData1].on = false; }

            if (fireMidiOutEvents) ShortMessageEvent?.Invoke(aChannelCommand, aData1, aData2, deviceId);

            if (MuteMessages)  return 0;

            if (MIDISettings.instance.outputSettings.synth && ((1 << channel) & synthChannelMask) != 0) Synth.SendShortMessage(aChannelCommand, aData1, aData2);
            
            return ((1 << channel) & channelMask) != 0 ? MidiOUTPlugin.SendShortMessage((byte)aChannelCommand, (byte)aData1, (byte)aData2, deviceId, editor) : 0;
        }

        public static int SendData(byte[] aData, int deviceId = -1) => MIDISettings.instance.outputSettings.active ? MidiOUTPlugin.SendData(aData, deviceId) : 0;
    }
}