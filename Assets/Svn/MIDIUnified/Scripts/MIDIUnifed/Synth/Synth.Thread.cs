/* Copyright © Marek Ledvina, Foriero s.r.o. */

using System;
using System.Threading;
using ForieroEngine.Collections.NonBlocking;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        public static volatile bool threaded = true;
        public static volatile int sleep = 2;
        private static readonly NonBlockingQueue<MidiMessage> Messages = new NonBlockingQueue<MidiMessage>();
        private static readonly NonBlockingQueue<ScheduledMidiMessage> ScheduledMessages = new NonBlockingQueue<ScheduledMidiMessage>();
        
        public static void Terminate() { MidiThread.Terminate(); }

        private static class MidiThread
        {
            private static volatile bool _terminating = false;
            private static Thread _thread;

            public static void Initialise()
            {
                if (_thread != null) return;

                if (MIDISettings.instance.debug) Debug.Log("MIDI Synth Thread : Start");

                _thread = new Thread(SynthThread);
                _thread.Start();
            }

            public static void ClearQueue()
            {
                var m = new MidiMessage();
                while (Messages.Dequeue(ref m)) {} 
                var sm = new ScheduledMidiMessage();
                while (ScheduledMessages.Dequeue(ref sm)) {}
            }
            
            private static void SynthThread()
            {
                var m = new MidiMessage();
                var sm = new ScheduledMidiMessage();

                do
                {
                    if (threaded)
                    {
                        while (ScheduledMessages.Dequeue(ref sm) && sm.timeProvider != null && sm.time > sm.timeProvider.GetTime())
                        {
                            Messages.Enqueue(sm.midiMessage);
                        }
                        
                        while (Messages.Dequeue(ref m))
                        {
                            if (m.CommandAndChannel != 0xF0) SendShortMessageInternal(m.CommandAndChannel, m.Data1, m.Data2);
                        }
                    }
                    Thread.Sleep(sleep);
                } while (!_terminating);
            }


            public static void Terminate()
            {
                if (_terminating) return;

                if (MIDISettings.instance.debug) Debug.Log("MIDI Synth Thread : Aborting");
                _terminating = true;

                if (MIDISettings.instance.debug) Debug.Log("MIDI Synth Thread : Joining");
                _thread?.Join();

                if (MIDISettings.instance.debug) Debug.Log("MIDi Synth Thread : Aborted");
            }
        }
    }
}
