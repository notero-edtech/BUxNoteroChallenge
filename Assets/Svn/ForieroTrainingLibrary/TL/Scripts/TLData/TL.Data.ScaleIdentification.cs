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
                    public class Data : Scale.Data
                    {
                        public Enums.Scale.ScaleFlags scale;
                        public Enums.ToneEnum root;
                        public Enums.ToneEnum[] tones;
                        public int[] pitches;

                    }

                    public static Data data = new Data();
                }
            }
        }
    }
}


