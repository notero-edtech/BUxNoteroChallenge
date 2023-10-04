/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training;
using UnityEngine;

namespace ForieroEngine.Music.Detection
{
    public class TunerDetection
    {
        Pitch.Tuner tuner;

        float lastPitch = 0f;
        int sampleRate = 44100;
        float minFrequency = 60;
        float maxFrequency = 1300;

        public TunerDetection(int sampleRate, float minFrequency, float maxFrequency)
        {
            this.sampleRate = sampleRate;
            this.minFrequency = minFrequency;
            this.maxFrequency = maxFrequency;
            tuner = new Pitch.Tuner();
        }

        public bool DetectPitch(float[] samples, out float pitch)
        {
            pitch = tuner.FindFundamentalFrequency(samples, sampleRate, minFrequency, maxFrequency);

            bool detected = lastPitch == 0 && pitch > 0;

            lastPitch = pitch;

            return detected;
        }
    }
}
