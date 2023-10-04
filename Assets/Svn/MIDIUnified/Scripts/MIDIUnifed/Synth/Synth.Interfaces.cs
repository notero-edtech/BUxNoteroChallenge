/* Copyright © Marek Ledvina, Foriero s.r.o. */
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Synthesizer
{   
    public interface ISynthProvider
    {
        int Start(Synth.Settings settings);
        int Stop();
        int SendShortMessage(int Command, int Data1, int Data2);
    }

    public interface ISynthRecorder
    {
        void StartRecording(AudioClip bgClip = null, float volume = 1f, float speed = 1f, int semitone = 0);
        void StopRecording();
    }
}
