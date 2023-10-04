/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForieroEngine.Music.Detection
{
    public interface IPitchDetector
    {
        float DetectPitch(float[] buffer, int frames);
    }
}
