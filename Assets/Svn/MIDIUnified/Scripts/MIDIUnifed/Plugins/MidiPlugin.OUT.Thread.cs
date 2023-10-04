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
    public static partial class MidiOUTPlugin
    {
        public static volatile bool threaded = true; 
        public static volatile int sleep = 1; 
        private static readonly NonBlockingQueue<MidiMessage> Messages = new NonBlockingQueue<MidiMessage>();
                        
        public static void Terminate() { MidiThread.TerminateInternal(); }

        private static class MidiThread
        {
            private static Thread _thread;
            private static volatile bool _terminating = false;

            public static void Initialise()
            {
                if (_thread != null) return;

                if (MIDISettings.instance.debug) Debug.Log("MIDI OUT Thread : Start");

                _thread = new Thread(MidiOutThread);
                _thread.Start();
            }

            private static void MidiOutThread()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJNI.AttachCurrentThread();
#endif
                var m = new MidiMessage();

                do
                {
                    if (threaded)
                    {
                        while (Messages.Dequeue(ref m))
                        {
                            if (m.CommandAndChannel == 0xF0) iMidiOUTDevice.SendData(m.Data, m.DeviceId, m.Editor);
                            else iMidiOUTDevice.SendMessage(m.CommandAndChannel, m.Data1, m.Data2, m.DeviceId, m.Editor);                            
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

                if (MIDISettings.instance.debug) Debug.Log("MIDI OUT Thread : Aborting");
                _terminating = true;

                if (MIDISettings.instance.debug) Debug.Log("MIDI OUT Thread : Joining");
                _thread?.Join();

                if (MIDISettings.instance.debug) Debug.Log("MIDi OUT Thread : Aborted");
            }
        }
    }
}