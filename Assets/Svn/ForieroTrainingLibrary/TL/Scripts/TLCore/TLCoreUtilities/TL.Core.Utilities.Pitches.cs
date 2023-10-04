/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        static partial class Utilities
        {
            public static partial class Pitches
            {
                public static Enums.ToneEnum ToTone(int pitch)
                {
                    return Utilities.Midi.FromIndex(pitch);
                }

                public static int FromTone(Enums.ToneEnum tone, int octave =  4)
                {
                    return Utilities.Midi.ToIndex(tone, octave);
                }

            }
        }
    }
}
