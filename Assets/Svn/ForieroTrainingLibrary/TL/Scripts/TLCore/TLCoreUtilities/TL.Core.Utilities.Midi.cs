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
            public static partial class Midi
            {
                public static Enums.ToneEnum FromIndex(int index)
                {
                    index = index % 12;
                    return (Enums.ToneEnum)index;
                }

                public static int ToIndex(Enums.ToneEnum tone, int octave)
                {
                    var index  = (int)tone + 12 * octave;
                    return index;
                }

            }
        }
    }
}
