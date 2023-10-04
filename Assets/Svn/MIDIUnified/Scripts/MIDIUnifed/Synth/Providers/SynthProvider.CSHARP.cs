using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        class CSHARPSynthProvider : SynthProvider, ISynthRecorder
        {
            public override int Start(Settings settings) => Plugin.Start(settings);
            public override int Stop() => Plugin.Stop();
            public override int SendShortMessage(int Command, int Data1, int Data2) => Plugin.SendShortMessage(Command, Data1, Data2);

            public void StartRecording(AudioClip bgClip = null, float volume = 1, float speed = 1f, int semitone = 0)
            {
               if(CSharpSynth.Instance)  CSharpSynth.Instance.StartRecording(bgClip, volume);
            }

            public void StopRecording()
            {
                if (CSharpSynth.Instance) CSharpSynth.Instance.StopRecording();
            }

            static class Plugin
            {
                public static int Start(Settings settings)
                {
                    CSharpSynth.StartSynthesizer(settings);
                    return 1;
                }

                public static int Stop()
                {
                    CSharpSynth.StopSynthesizer();
                    return 1;
                }

                public static int SendShortMessage(int Command, int Data1, int Data2)
                {
                    CSharpSynth.SendShortMessage((byte)Command, (byte)Data1, (byte)Data2);
                    return 1;                    
                }
            }
        }
    }
}
