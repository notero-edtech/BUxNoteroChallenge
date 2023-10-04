/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.Detection
{
    public class PitchDetection
    {
        PitchTracker pitchTracker;
        Action<int> OnDetectedMidiIndex;

        public PitchDetection(int sampleRate, Action<int> onDetectedMidiIndex)
        {
            pitchTracker = new PitchTracker();
            pitchTracker.SampleRate = sampleRate;
            pitchTracker.PitchDetected += PitchTracker_PitchDetected;
            OnDetectedMidiIndex = onDetectedMidiIndex;
        }

        public void DetectPitch(float[] samples)
        {
            pitchTracker.ProcessBuffer(samples, samples.Length);
        }

        void PitchTracker_PitchDetected(Detection.PitchTracker sender, Detection.PitchTracker.PitchRecord pitchRecord)
        {
            if (pitchRecord.MidiNote > 0 && OnDetectedMidiIndex != null)
            {
                OnDetectedMidiIndex(pitchRecord.MidiNote);
            }
        }
    }

    public class PitchEvaluator
    {
        Dictionary<int, int> cummulatedPitches = new Dictionary<int, int>();

        public int treshold = 10;

        public List<int> detectedPitches = new List<int>();

        public void Reset()
        {
            cummulatedPitches = new Dictionary<int, int>();
            detectedPitches = new List<int>();
        }

        public void Update(int midi)
        {
            if (cummulatedPitches.ContainsKey(midi))
            {
                cummulatedPitches[midi] = cummulatedPitches[midi] + 1;
            }
            else
            {
                cummulatedPitches.Add(midi, 1);
            }

            foreach (KeyValuePair<int, int> kv in cummulatedPitches)
            {
                if (kv.Value >= treshold)
                {
                    if (!detectedPitches.Contains(kv.Key))
                    {
                        detectedPitches.Add(kv.Key);
                    }
                }
            }
        }
    }
}
