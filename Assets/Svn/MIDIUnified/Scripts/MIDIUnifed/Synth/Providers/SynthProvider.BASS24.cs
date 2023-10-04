using UnityEngine;
using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        class BASS24SynthProvider : SynthProvider, ISynthRecorder
        {
            public override int Start(Settings settings) => Plugin.Start(settings);
            public override int Stop() => Plugin.Stop();
            public override int SendShortMessage(int Command, int Data1, int Data2) => Plugin.SendShortMessage(Command, Data1, Data2);

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR

            public void StartRecording(AudioClip bgClip = null, float volume = 1, float speed = 1f, int semitone = 0)
            {
                BASS24SynthPlugin.StartRecording(bgClip, volume, speed, semitone);
            }

            public void StopRecording()
            {
                BASS24SynthPlugin.StopRecording();
            }

            static class Plugin
            {
                public static int Start(Settings settings)
                {
                    return BASS24SynthPlugin.Start(settings.sampleRate, settings.channels);
                }

                public static int Stop()
                {
                    return BASS24SynthPlugin.Stop();
                }

                public static int SendShortMessage(int Command, int Data1, int Data2)
                {
                    return BASS24SynthPlugin.SendMidiMessage(Command, Data1, Data2);
                }
            }

#else
            public void StartRecording(AudioClip bgClip = null, float volume = 1, float speed = 1f, int semitone = 0)
            {
                Debug.LogError("MU | BASS24Synth : Synthesizer not supported!");
            }

            public void StopRecording()
            {
                Debug.LogError("MU | BASS24Synth : Synthesizer not supported!");
            }
                        
            static class Plugin
            {
                public static int Start(Settings settings)
                {
                    Debug.LogError("MU | BASS24Synth : Synthesizer not supported!");
                    return 0;
                }

                public static int Stop()
                {
                    Debug.LogError("MU | BASS24Synth : Synthesizer not supported!");
                    return 0;
                }

                public static int SendShortMessage(int Command, int Data1, int Data2)
                {
                    Debug.LogError("MU | BASS24Synth : Synthesizer not supported!");
                    return 0;
                }
            }
#endif
        }
    }
}
