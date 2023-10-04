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
            public static partial class Theory
            {
                public static Enums.ToneEnum GetPreviousTone(Enums.ToneEnum tone)
                {
                    return (tone == Enums.ToneEnum.C) ? Enums.ToneEnum.B : (Enums.ToneEnum)((int)tone - 1);
                }

                public static Enums.ToneEnum GetNextTone(Enums.ToneEnum tone)
                {
                    return (tone == Enums.ToneEnum.B) ? Enums.ToneEnum.C : (Enums.ToneEnum)((int)tone + 1);
                }

                public static int GetDistanceBetweenTones(Enums.ToneEnum low, Enums.ToneEnum high)
                {
                    int lowVal = (int)low;
                    int highVal = (int)high;

                    if (highVal < lowVal)
                    {
                        highVal += 12;
                    }
                    return highVal - lowVal;
                }
            }
        }
    }
}
