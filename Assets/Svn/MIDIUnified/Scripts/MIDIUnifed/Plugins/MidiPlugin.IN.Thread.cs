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
        public static volatile bool threaded = true;
        public static volatile int sleep = 1;
        public static volatile bool through = false;
        public static volatile bool synth = false;
        
        private static readonly NonBlockingQueue<MidiMessage> Messages = new NonBlockingQueue<MidiMessage>();
        private static readonly NonBlockingQueue<MidiMessage> MessagesEditor = new NonBlockingQueue<MidiMessage>();

        public static void Terminate() { MidiThread.TerminateInternal(); }

        private static class MidiThread
        {
            private static volatile bool _terminating = false;
            private static Thread _thread;

            public static void Initialise()
            {
                if (_thread != null) return;

                if (MIDISettings.instance.debug) Debug.Log("MIDI IN Thread : Start");

                _thread = new Thread(MidiInThread);
                _thread.Start();
            }

            private static void MidiInThread()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJNI.AttachCurrentThread();
#endif
                do
                {
                    if (threaded)
                    {
                        while (iMidiInDevice.PopMessage(out var m) == 1)
                        {
                            Messages.Enqueue(m);

                            if (m.Command.IsToneON()) DSP.ToneOn(m.Data1);
                            if (m.Command.IsToneOFF()) DSP.ToneOff(m.Data1);

                            if (m.Command.IsControlChange())
                            {
                                switch (m.Data1.ToControllerEnum())
                                {
                                    case ControllerEnum.Sostenuto:
                                        DSP.ControlChange[(int)PedalEnum.Sostenuto].On = m.Data2 > 0;
                                        if (DSP.ControlChange[(int)PedalEnum.Sostenuto].On) DSP.ControlChange[(int)PedalEnum.Sostenuto].PressedTime = (float)AudioSettings.dspTime;
                                        else  DSP.ControlChange[(int)PedalEnum.Sostenuto].ReleasedTime = (float)AudioSettings.dspTime;
                                        break;
                                    case ControllerEnum.DamperPedal: 
                                        DSP.ControlChange[(int)PedalEnum.DamperPedal].On = m.Data2 > 0;
                                        if (DSP.ControlChange[(int)PedalEnum.DamperPedal].On) DSP.ControlChange[(int)PedalEnum.DamperPedal].PressedTime = (float)AudioSettings.dspTime;
                                        else  DSP.ControlChange[(int)PedalEnum.DamperPedal].ReleasedTime = (float)AudioSettings.dspTime;
                                        break;
                                    case ControllerEnum.SoftPedal: 
                                        DSP.ControlChange[(int)PedalEnum.SoftPedal].On = m.Data2 > 0;
                                        if (DSP.ControlChange[(int)PedalEnum.SoftPedal].On) DSP.ControlChange[(int)PedalEnum.SoftPedal].PressedTime = (float)AudioSettings.dspTime;
                                        else  DSP.ControlChange[(int)PedalEnum.SoftPedal].ReleasedTime = (float)AudioSettings.dspTime;
                                        break;
                                    case ControllerEnum.Portamento: 
                                        DSP.ControlChange[(int)PedalEnum.Portamento].On = m.Data2 > 0;
                                        if (DSP.ControlChange[(int)PedalEnum.Portamento].On) DSP.ControlChange[(int)PedalEnum.Portamento].PressedTime = (float)AudioSettings.dspTime;
                                        else  DSP.ControlChange[(int)PedalEnum.Portamento].ReleasedTime = (float)AudioSettings.dspTime;
                                        break;
                                }
                            }
                            
                            if (through)
                            {
                                MidiOUTPlugin.SendMidiMessage(new MidiMessage(m) { Editor = false, Through = true });
                                m.Editor = true; m.Through = true;
                            }

                            if (synth)
                            {
                                Synthesizer.Synth.SendMidiMessage(m);
                                m.Editor = false; m.Synth = true;
                            }
                        }

                        while (iMidiInDevice.PopMessage(out var m, true) == 1)
                        {
                            MessagesEditor.Enqueue(m);
                            
                            if (through)
                            {
                                MidiOUTPlugin.SendMidiMessage(new MidiMessage(m) { Editor = true, Through = true });
                                m.Editor = true; m.Through = true;
                            }

                            if (synth)
                            {
                                Synthesizer.Synth.SendMidiMessage(m);
                                m.Editor = true; m.Synth = true;
                            }
                        }
                    }

                    Thread.Sleep(sleep);
                } while (!_terminating);
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJNI.DetachCurrentThread();
#endif
            }

            public static void TerminateInternal()
            {
                if (_terminating) return;

                if (MIDISettings.instance.debug) Debug.Log("MIDI IN Thread : Aborting");
                _terminating = true;

                if (MIDISettings.instance.debug) Debug.Log("MIDI IN Thread : Joining");
                _thread?.Join();

                if (MIDISettings.instance.debug) Debug.Log("MIDi IN Thread : Aborted");
            }
        }
    }
}