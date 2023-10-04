/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Chord
            {
                public static partial class Inversions
                {
                    public class Data : Chord.Data
                    {
                        public Enums.Chord.ChordTypeFlags chord;
                        public Enums.Chord.ChordInversionTypeFlags inversion;
                        public Enums.ToneEnum[] tones;
                        public int[] pitches;
                    }

                    public static Data data = new Data();
                }
            }
        }
    }

}
