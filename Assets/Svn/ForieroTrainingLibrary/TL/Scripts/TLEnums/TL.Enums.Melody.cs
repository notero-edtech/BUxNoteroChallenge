/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Enums
        {
            public static class Melody
            {
                public enum MelodyEnum
                {
                    PitchOnly,
                    Rhythmic
                }

                public enum MelodyLastToneEnum
                {
                    I,
                    I_III_V
                }

                /*public enum MelodyTimeSignatureEnum
                {
                    _3_4 = TimeSignatureFlags._3_4,
                    _4_4 = TimeSignatureFlags._4_4
                }*/

                public enum MelodyPickFrom
                {
                    Random = PickFromEnum.Random,
                    Keys = PickFromEnum.Keys
                }

                [Flags]
                public enum MelodyNoteFlags
                {
                    Whole = DurationFlags.Whole,
                    Half = DurationFlags.Half,
                    Quarter = DurationFlags.Quarter,
                    Item8th = DurationFlags.Item8th,
                    Item16th = DurationFlags.Item16th
                }
            }
        }
    }
}
