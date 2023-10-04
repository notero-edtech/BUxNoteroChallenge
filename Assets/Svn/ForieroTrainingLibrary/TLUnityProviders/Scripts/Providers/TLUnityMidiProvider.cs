/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using UnityEngine;
using ForieroEngine.MIDIUnified;


namespace ForieroEngine.Music.Training.Classes.Providers
{
    public class TLUnityMidiProvider : MidiProvider
    {
        public static MidiProvider provider;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Init()
        {
            System.Diagnostics.Stopwatch stopWatch = Debug.isDebugBuild ? System.Diagnostics.Stopwatch.StartNew() : null;
            provider = new TLUnityMidiProvider() as MidiProvider;
            TL.Providers.midi = provider;
            if (Debug.isDebugBuild) Debug.Log("METHOD STOPWATCH (TLUnityMidiProvider - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

        public override void NoteDispatch(int interval1Low, float toneDuration, float toneGap, int attack, int instrumentChannel, Action onStart = null, Action onStop = null)
        {
            MidiOut.NoteDispatch(interval1Low, toneDuration, toneGap, attack, instrumentChannel, -1, onStart, onStop);
        }

        public override void SchedulePercussion(TL.Enums.MIDI.PercussionEnum percussionEnum, int attack, float scheduleTime = 0)
        {
            MidiOut.SchedulePercussion((PercussionEnum)percussionEnum, attack, scheduleTime);
        }

        public override void Reset()
        {
            Debug.Log("MidiProvider - RESET");
            MidiOut.AllSoundOff();
            MIDIPercussion.CancelScheduledPercussion();
        }

        MidiEvents midiEvents = new MidiEvents();
        ShortMessageEventHandler shortMessageEventHandler;

        public TLUnityMidiProvider()
        {
            if (MidiInput.singleton)
            {
                OnMidiInputInitialized(MidiInput.singleton);
            }
            else
            {
                MidiInput.OnInitialized += OnMidiInputInitialized;
            }

            if (MidiKeyboardInput.singleton)
            {
                OnMidiKeyboardInputInitialized(MidiKeyboardInput.singleton);
            }
            else
            {
                MidiKeyboardInput.OnInitialized += OnMidiKeyboardInputInitialized;
            }

            if (MidiPlayMakerInput.singleton)
            {
                OnMidiPlaymakerInputInitialized(MidiPlayMakerInput.singleton);
            }
            else
            {
                MidiPlayMakerInput.OnInitialized += OnMidiPlaymakerInputInitialized;
            }

            midiEvents.NoteOnEvent += NoteOn;
            midiEvents.NoteOffEvent += NoteOff;
        }

        private void OnMidiPlaymakerInputInitialized(MidiPlayMakerInput midiPlaymakerInput)
        {
            midiEvents.AddSender(midiPlaymakerInput);
        }

        private void OnMidiKeyboardInputInitialized(MidiKeyboardInput midiKeyboardInput)
        {
            midiEvents.AddSender(midiKeyboardInput);
        }

        public void OnMidiInputInitialized(MidiInput midiInput)
        {
            midiEvents.AddSender(midiInput);
        }

        void NoteOn(int aNote, int aVolume, int aChannel)
        {
            if (TL.Inputs.OnMidiOn != null)
            {
                TL.Inputs.OnMidiOn(aNote);
            }
        }

        void NoteOff(int aNote, int aVolume, int aChannel)
        {
            if (TL.Inputs.OnMidiOff != null)
            {
                TL.Inputs.OnMidiOff(aNote);
            }

        }

        void PedalOn(PedalEnum aPedal, int aValue, int aChannel)
        {

        }

        void PedalOff(PedalEnum aPedal, int aValue, int aChannel)
        {

        }
    }
}
