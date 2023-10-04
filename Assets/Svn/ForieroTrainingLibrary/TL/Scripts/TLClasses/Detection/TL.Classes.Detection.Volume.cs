/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training;
using UnityEngine;

namespace ForieroEngine.Music.Detection
{
    public class VolumeLevelDetection
    {
        Volume.VolumeLevel vl = new Volume.VolumeLevel();

        public float DetectVolumeLevel(float[] samples)
        {
            return vl.DetectVolume(samples, samples.Length);
        }
    }
}
