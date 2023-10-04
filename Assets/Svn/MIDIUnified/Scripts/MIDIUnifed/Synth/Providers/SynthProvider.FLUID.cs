using UnityEngine;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        class FLUIDSynthProvider : SynthProvider, ISynthRecorder
        {
            public override int Start(Settings settings) => Plugin.Start(settings);
            public override int Stop() => Plugin.Stop();
            public override int SendShortMessage(int Command, int Data1, int Data2) => Plugin.SendShortMessage(Command, Data1, Data2);

#if (UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_STANDALONE || UNITY_EDITOR) && MIDIUNIFIED_BETA
            public void StartRecording(AudioClip bgClip = null, float volume = 1, float speed = 1f, int semitone = 0)
            {
                if (FluidSynth.singleton) FluidSynth.singleton.StartRecording(bgClip, volume);
            }

            public void StopRecording()
            {
                if (FluidSynth.singleton) FluidSynth.singleton.StopRecording();
            }

            static class Plugin
            {
                public static int Start(Settings settings) {
                    FluidSynth.StartSynthesizer(settings);
                    return 1;
                }

                public static int Stop()
                {
                    FluidSynth.StopSynthesizer();
                    return 1;
                }

                public static int SendShortMessage(int Command, int Data1, int Data2) {
                    FluidSynth.SendShortMessage((byte)Command, (byte)Data1, (byte)Data2);
                    return 1;
                }
            }
#else           
            public void StartRecording(AudioClip bgClip = null, float volume = 1, float speed = 1f, int semitone = 0)
            {
                Debug.LogError ("FluidSynth Synthesizer not supported!");			
            }

            public void StopRecording()
            {
                Debug.LogError ("FluidSynth Synthesizer not supported!");			
            }

            static class Plugin
            {
                public static int Start(Settings settings)
                {
                    Debug.LogError("FluidSynth Synthesizer not supported!");
                    return 0;
                }

                public static int Stop()
                {
                    Debug.LogError("FluidSynth Synthesizer not supported!");
                    return 0;
                }

                public static int SendShortMessage(int Command, int Data1, int Data2)
                {
                    Debug.LogError("FluidSynth Synthesizer not supported!");
                    return 0;
                }
            }
#endif
        }
    }
}
