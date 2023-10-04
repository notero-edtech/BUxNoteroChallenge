/* Copyright © Marek Ledvina, Foriero s.r.o. */
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Synthesizer
{
    public static partial class Synth
    {
        public enum SynthEnum
        {
            NONE = 0,
            NATIVE = 1,
            BASS24 = 2,
            CSHARP = 3,
#if MIDIUNIFIED_BETA
            FLUID = 4,
            TINYSOUNDFONT = 5
#endif
        }

        [System.Serializable]
        public abstract class SynthSettings
        {
            public enum OutputSampleRateDividerEnum
            {
                One = 1,
                Two = 2,
                Four = 4
            }

            [Tooltip("AudioSettings.outputSampleRate / outputSampleRateDivider")]
            public OutputSampleRateDividerEnum outputSampleRateDivider = OutputSampleRateDividerEnum.One;

            [Range(1, 256)]
            [Tooltip("Number of maximum simultaneously playing tones.")]
            public int polyphony = 64;
            public int sampleRate => 
#if WWISE
                44100 / (int) outputSampleRateDivider;
#else
                AudioSettings.outputSampleRate / (int)outputSampleRateDivider;
#endif

            [Range(1, 16)]
            public int channels = 16;
            public bool preinit = false;

            [Range(0, 4)]
            public float volume = 1f;

            public abstract Synth.SynthEnum GetSynthEnum();

            public ISynthProvider CreateSynthProvider()
            {
                switch (GetSynthEnum())
                {
                    case SynthEnum.NONE: return null;
                    case SynthEnum.NATIVE: return new NATIVESynthProvider();
                    case SynthEnum.BASS24: return new BASS24SynthProvider();
                    case SynthEnum.CSHARP: return new CSHARPSynthProvider();
#if MIDIUNIFIED_BETA
                    case SynthEnum.TINYSOUNDFONT: return new TINYSOUNDFONDSynthProvider();
                    case SynthEnum.FLUID: return new FLUIDSynthProvider();
#endif
                    default: return null;
                }
            }
        }

        [System.Serializable]
        public class SynthSettingsWSA : SynthSettings
        {
            public enum SynthEnum
            {
                NONE = Synth.SynthEnum.NONE,
                BASS24 = Synth.SynthEnum.BASS24,
                CSHARP = Synth.SynthEnum.CSHARP,
#if MIDIUNIFIED_BETA
                TINYSOUNDFONT = Synth.SynthEnum.TINYSOUNDFONT,
#endif                
            }

            public SynthEnum synthesizer = SynthEnum.CSHARP;

            public override Synth.SynthEnum GetSynthEnum()
            {
                return (Synth.SynthEnum)synthesizer;
            }
        }

        [System.Serializable]
        public class SynthSettingsWEBGL : SynthSettings
        {
            public enum SynthEnum
            {
                NONE = Synth.SynthEnum.NONE,
                CSHARP = Synth.SynthEnum.CSHARP,
#if MIDIUNIFIED_BETA
                TINYSOUNDFONT = Synth.SynthEnum.TINYSOUNDFONT,
#endif
            }

            public SynthEnum synthesizer = SynthEnum.CSHARP;

            public override Synth.SynthEnum GetSynthEnum()
            {
                return (Synth.SynthEnum)synthesizer;
            }
        }

        [System.Serializable]
        public class SynthSettingsOSX : SynthSettings
        {
            public enum SynthEnum
            {
                NONE = Synth.SynthEnum.NONE,
                BASS24 = Synth.SynthEnum.BASS24,
                NATIVE = Synth.SynthEnum.NATIVE,
                CSHARP = Synth.SynthEnum.CSHARP,
#if MIDIUNIFIED_BETA
                FLUID = Synth.SynthEnum.FLUID,
                TINYSOUNDFONT = Synth.SynthEnum.TINYSOUNDFONT,
#endif
            }

            public SynthEnum synthesizer = SynthEnum.CSHARP;

            public override Synth.SynthEnum GetSynthEnum()
            {
                return (Synth.SynthEnum)synthesizer;
            }
        }

        [System.Serializable]
        public class SynthSettingsLINUX : SynthSettings
        {
            public enum SynthEnum
            {
                NONE = Synth.SynthEnum.NONE,
                BASS24 = Synth.SynthEnum.BASS24,
                NATIVE = Synth.SynthEnum.NATIVE,
                CSHARP = Synth.SynthEnum.CSHARP,
#if MIDIUNIFIED_BETA
                FLUID = Synth.SynthEnum.FLUID,
                TINYSOUNDFONT = Synth.SynthEnum.TINYSOUNDFONT,
#endif
            }

            public SynthEnum synthesizer = SynthEnum.CSHARP;

            public override Synth.SynthEnum GetSynthEnum()
            {
                return (Synth.SynthEnum)synthesizer;
            }
        }

        [System.Serializable]
        public class SynthSettingsWIN : SynthSettings
        {
            public enum SynthEnum
            {
                NONE = Synth.SynthEnum.NONE,
                BASS24 = Synth.SynthEnum.BASS24,
                CSHARP = Synth.SynthEnum.CSHARP,
#if MIDIUNIFIED_BETA
                FLUID = Synth.SynthEnum.FLUID,
                TINYSOUNDFONT = Synth.SynthEnum.TINYSOUNDFONT,
#endif
            }

            public SynthEnum synthesizer = SynthEnum.CSHARP;

            public override Synth.SynthEnum GetSynthEnum()
            {
                return (Synth.SynthEnum)synthesizer;
            }
        }

        [System.Serializable]
        public class SynthSettingsIOS : SynthSettings
        {
            public enum SynthEnum
            {
                NONE = Synth.SynthEnum.NONE,
                BASS24 = Synth.SynthEnum.BASS24,
                NATIVE = Synth.SynthEnum.NATIVE,
                CSHARP = Synth.SynthEnum.CSHARP,
#if MIDIUNIFIED_BETA
                FLUID = Synth.SynthEnum.FLUID,
                TINYSOUNDFONT = Synth.SynthEnum.TINYSOUNDFONT,
#endif
            }

            public SynthEnum synthesizer = SynthEnum.CSHARP;

            public override Synth.SynthEnum GetSynthEnum()
            {
                return (Synth.SynthEnum)synthesizer;
            }

            public enum SoundBankEnum
            {
                sf2 = 0,
                dls = 1,
                aupreset = 2
            }

            public SoundBankEnum soundBank = SoundBankEnum.sf2;
        }

        [System.Serializable]
        public class SynthSettingsANDROID : SynthSettings
        {
            public enum SynthEnum
            {
                NONE = Synth.SynthEnum.NONE,
                BASS24 = Synth.SynthEnum.BASS24,
                CSHARP = Synth.SynthEnum.CSHARP,
#if MIDIUNIFIED_BETA
                FLUID = Synth.SynthEnum.FLUID,
                TINYSOUNDFONT = Synth.SynthEnum.TINYSOUNDFONT,
#endif
            }

            public SynthEnum synthesizer = SynthEnum.CSHARP;

            public override Synth.SynthEnum GetSynthEnum()
            {
                return (Synth.SynthEnum)synthesizer;
            }
        }
    }
}
