/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Chord
            {
                public static partial class Progressions
                {
                    public class Data : Chord.Data
                    {
                        public Enums.Scale.ScaleFlags scale;
                        public Enums.ToneEnum root;
                        public Enums.Scale.ScaleDegreeEnum[] degrees;
                        public int[] pitches;
                    }

                    public static Data data = new Data();
                }
            }
        }
    }
}
