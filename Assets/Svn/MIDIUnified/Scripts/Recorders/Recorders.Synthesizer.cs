using UnityEngine;

namespace ForieroEngine.MIDIUnified.Recording
{
    public static partial class Recorders
    {
        public static class Synth
        {
            public static readonly string fileName = "synth.wav";

            public static void Start(AudioClip bgClip = null, float volume = 1f, float speed = 1f, int semitone = 0)
            {
                Synthesizer.Synth.StartRecording(bgClip, volume, speed, semitone);               
            }

            public static void Stop()
            {
                Synthesizer.Synth.StopRecording();
            }
        }
    
    }
}
