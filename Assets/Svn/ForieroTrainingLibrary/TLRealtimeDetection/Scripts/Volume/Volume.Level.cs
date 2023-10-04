/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Diagnostics;
using UnityEngine;

namespace ForieroEngine.Music.Detection.Volume
{
    // originally based on awesomebox, modified by Mark Heath
    public class VolumeLevel
    {
        public float DetectVolume(float[] samples, int samplesCount)
        {
            float levelMax = 0;
            for (int i = 0; i < samplesCount; i++)
            {
                float wavePeak = samples[i] * samples[i];
                if (levelMax < wavePeak)
                {
                    levelMax = wavePeak;
                }
            }
            // levelMax equals to the highest normalized value power 2, a small number because < 1
            // use it like:
            return Mathf.Sqrt(levelMax);
        }
    }
}
