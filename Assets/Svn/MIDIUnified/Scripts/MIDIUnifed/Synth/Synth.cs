/* Copyright © Marek Ledvina, Foriero s.r.o. */
using System.Threading.Tasks;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        public struct Settings
        {
            public int sampleRate;
            public int channels;
            public int polyphony;
        }

        private static ISynthProvider _provider = null;
        private static ISynthRecorder _recorder = null;
        public static SynthSettings settings;
        
        private static volatile bool _initialized = false;
        public static bool Initialized => _initialized;

        public enum InstrumentAudioOutputEnum { Instrument, Percussion }
        public static InstrumentAudioOutputEnum InstrumentAudioOutput { get; set; } = InstrumentAudioOutputEnum.Instrument;
        
        public enum PercussionAudioOutputEnum { Synth, Audio }
        public static PercussionAudioOutputEnum PercussionAudioOutput { get; set; } = PercussionAudioOutputEnum.Synth;

        public static void Init()
        {
            settings = MIDISynthSettings.GetPlatformSettings();
            _provider = settings.CreateSynthProvider();
            _recorder = _provider is ISynthRecorder ? (ISynthRecorder)_provider : null;
            
            MidiThread.Initialise();
        }

        public static void ClearQueue() => MidiThread.ClearQueue();
        
        public static void Start()
        {
            if (settings == null || _provider == null)
            {
                if (MIDISettings.instance.debug) { Debug.LogError("Settings are NULL. Likely unsupported synth platform or you set not to initialize mid!"); }
                return;
            }

            var sampleRate = settings.sampleRate;
            var channels = settings.channels;

            _initialized = false;

#if UNITY_IOS && !UNITY_EDITOR
            NATIVESynthProvider.soundBank = (settings as SynthSettingsIOS).soundBank;
#endif
            switch (settings.GetSynthEnum())
            {
                case SynthEnum.NONE: break;
                case SynthEnum.NATIVE:                
                case SynthEnum.BASS24:                
                    Task.Run(() => {
                        _initialized = _provider?.Start(new Settings { sampleRate = sampleRate, channels = channels }) == 1;
                        PreInit();
                    });
                    break;
                
                case SynthEnum.CSHARP:
#if MIDIUNIFIED_BETA
                case SynthEnum.TINYSOUNDFONT:
                case SynthEnum.FLUID:
#endif
                    _initialized = _provider?.Start(new Settings { sampleRate = sampleRate, channels = channels }) == 1;
                    PreInit();
                    break;
               
            }
        }

        private static void PreInit()
        {
            if (!Initialized || !settings.preinit) return;
            for (int i = 0; i < settings.channels; i++)
            {
                for (int k = 0; k < 128; k++)
                {
                    _provider?.SendShortMessage((int)CommandEnum.MIDI_NOTE_ON + i, k, 1);
                    _provider?.SendShortMessage((int)CommandEnum.MIDI_NOTE_OFF + i, k, 0);
                }
            }
        }

        public static void Stop()
        {
            MidiThread.ClearQueue();
            _provider?.Stop();
            _initialized = false;
        }

        public static void SendScheduledShortMessage(double time, ITimeProvider timeProvider, int aCommand, int aChannel, int aData1, int aData2) 
            => SendScheduledShortMessage(time, timeProvider, aChannel + aCommand, aData1, aData2);
        public static void SendScheduledShortMessage(double time, ITimeProvider timeProvider, int aChannelCommand, int aData1, int aData2)
        { 
            if (settings == null || !Initialized || !threaded) return;
            ScheduledMessages.Enqueue(
                new ScheduledMidiMessage { 
                    time = time,
                    timeProvider = timeProvider, 
                    midiMessage = new MidiMessage() {CommandAndChannel = (byte)aChannelCommand, Data1 = (byte)aData1, Data2 = (byte)aData2, DataSize = 3 }
                });
        }
        
        // {
        //     if (channel == 9 || InstrumentAudioOutput == InstrumentAudioOutputEnum.Percussion)
        //     {
        //         switch (PercussionAudioOutput)
        //         {
        //             case PercussionAudioOutputEnum.Synth: Synth.SendShortMessage(9 + command, aData1, aData2); break;
        //             case PercussionAudioOutputEnum.Audio: SchedulePercussion((PercussionEnum)aData1, aData2); break;
        //         }
        //     } else { Synth.SendShortMessage(aChannelCommand, aData1, aData2); }
        // }

        public static void SendMidiMessage(MidiMessage m)
        {
            if (settings == null || !Initialized) return;
            if (threaded) Messages.Enqueue(m); 
            else SendShortMessageInternal(m.CommandAndChannel, m.Data1, m.Data2);
        }
        public static void SendShortMessage(int aCommand, int aChannel, int aData1, int aData2) => SendShortMessage(aChannel + aCommand, aData1, aData2);
        public static void SendShortMessage(int aChannelCommand, int aData1, int aData2)
        {
            if (settings == null || !Initialized) return;
            if (threaded) Messages.Enqueue(new MidiMessage { CommandAndChannel = (byte)aChannelCommand, Data1 = (byte)aData1, Data2 = (byte)aData2, DataSize = 3 });
            else SendShortMessageInternal(aChannelCommand, aData1, aData2);
        }

        private static void SendShortMessageInternal(int aChannelCommand, int aData1, int aData2)
        {
            _provider?.SendShortMessage(aChannelCommand, aData1, aData2);
            
#if WWISE
            if (MIDISynthSettings.instance.wwise && Camera.main)
            {
                uint eventId = AkSoundEngine.GetIDFromString("Play_Flute");
                AkMIDIPost p = new AkMIDIPost();
                AkMIDIPostArray pArray = new AkMIDIPostArray(1);
                p.byChan =  (byte)aChannelCommand.ToMidiChannel();
                p.byType = (AkMIDIEventTypes) aChannelCommand.ToMidiCommand();
                p.NoteOnOff.byNote = (byte) aData1;
                p.NoteOnOff.byVelocity = (byte) aData2;
                pArray[0] = p;
                AkSoundEngine.PostMIDIOnEvent(eventId, Camera.main.gameObject, pArray, 1);
            }
#endif
        }

        public static void StartRecording(AudioClip bgClip = null, float volume = 1f, float speed = 1f, int semitone = 0)
        {
            _recorder?.StartRecording(bgClip, volume, speed, semitone);
        }

        public static void StopRecording()
        {
            _recorder.StopRecording();
        }
    }
}