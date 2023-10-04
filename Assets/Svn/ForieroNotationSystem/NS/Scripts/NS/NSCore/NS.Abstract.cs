/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem
{
    public abstract partial class NS
    {
        public double pixelLenght = 0;

        public abstract void LoadMidi(byte[] bytes);
        public abstract void LoadMusicXML(byte[] bytes);
        public abstract void LoadMusic(float[] samples, int channels, float totalTime);

        public abstract NSObjectCheckEnum CheckAddObjectConstraints<T>(NSObject parent) where T : NSObject;
        public abstract NSObjectCheckEnum CheckSetObjectConstraints<T>(NSObject parent) where T : NSObject;
    }
}
