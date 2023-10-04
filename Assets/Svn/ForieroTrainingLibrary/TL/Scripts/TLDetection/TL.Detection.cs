/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Detection;
using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Detection
        {
            static bool initialized = false;
#pragma warning disable 414
            static int sampleRate = 44100;
#pragma warning restore 414

            static PitchTracker pitchTracker = null;
            static ClapDetection clapDetection = null;
            static VolumeLevelDetection volumeLevelDetection = null;

            static Enums.Microphone.DetectionFlags _detectionFlags =
                Enums.Microphone.DetectionFlags.Clap |
                Enums.Microphone.DetectionFlags.Pitch |
                Enums.Microphone.DetectionFlags.Tuner;

            public static Enums.Microphone.DetectionFlags detectionFlags
            {
                get
                {
                    return _detectionFlags;
                }
                set
                {
                    _detectionFlags = value;
                }
            }

            public static void Initialize(int sampleRate)
            {
                Detection.sampleRate = sampleRate;

                volumeLevelDetection = new VolumeLevelDetection();

                pitchTracker = new PitchTracker();
                pitchTracker.SampleRate = sampleRate;
                pitchTracker.PitchDetected += PitchTracker_PitchDetected;

                clapDetection = new ClapDetection(sampleRate);

                initialized = true;
            }

            public static void Update(float[] samples)
            {
                if (!initialized) return;

                Inputs.volumeLevel = volumeLevelDetection.DetectVolumeLevel(samples);
                Inputs.OnVolumeLevel?.Invoke(Inputs.volumeLevel);

                if (detectionFlags.Has(Enums.Microphone.DetectionFlags.Clap)) if (clapDetection.DetectClap(samples)) Inputs.OnClap?.Invoke();

                if (detectionFlags.Has(Enums.Microphone.DetectionFlags.Pitch) || detectionFlags.Has(Enums.Microphone.DetectionFlags.Tuner))
                {
                    pitchTracker.ProcessBuffer(samples, samples.Length);
                }
            }

            public static void Reset()
            {
                detectionFlags = 0;
            }

            static void PitchTracker_PitchDetected(PitchTracker sender, PitchTracker.PitchRecord pitchRecord)
            {
                if (pitchRecord.MidiNote == 0) return;
                if (detectionFlags.Has(Enums.Microphone.DetectionFlags.Pitch)) Inputs.OnPitch?.Invoke(pitchRecord.MidiNote, pitchRecord.MidiCents);
                if (detectionFlags.Has(Enums.Microphone.DetectionFlags.Tuner)) Inputs.OnTuner?.Invoke(pitchRecord.Pitch);
            }
        }
    }
}
