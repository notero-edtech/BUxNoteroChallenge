/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training;
using UnityEngine;

namespace ForieroEngine.Music.Detection
{
    public class ClapDetection
    {
        Pitch.AutoCorrelator ac;

        float pitch = 0f;
        float lastPitch = 0f;

        public ClapDetection(int sampleRate)
        {
            ac = new Pitch.AutoCorrelator(sampleRate);
        }

        public bool DetectClap(float[] samples)
        {
            pitch = ac.DetectPitch(samples, samples.Length);

            bool detected = lastPitch == 0 && pitch > 0;

            lastPitch = pitch;

            return detected;
        }
    }
}
