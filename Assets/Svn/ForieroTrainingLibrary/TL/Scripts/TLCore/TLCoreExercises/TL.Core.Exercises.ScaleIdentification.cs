/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Scale
            {
                public static partial class Identification
                {
                    static partial class CoreIdentification
                    {

                        public static void Generate()
                        {
                            data.root = Utilities.RandomEnum<Enums.ToneEnum>();
                            data.scale = Utilities.RandomEnumFromFlags<Enums.Scale.ScaleFlags>(settings.scaleFlags);

                            data.tones = Utilities.Scales.GetScaleTones(data.root, data.scale);
                            data.pitches = TL.Exercises.settings.pitchRange.GetPitchRangeFromTones(data.tones);
                        }
                    }
                }
            }
        }
    }
}



