using UnityEngine;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        abstract class SynthProvider : ISynthProvider
        {
            public abstract int Start(Settings settings);
            public abstract int Stop();
            public abstract int SendShortMessage(int Command, int Data1, int Data2);
        }
    }
}
