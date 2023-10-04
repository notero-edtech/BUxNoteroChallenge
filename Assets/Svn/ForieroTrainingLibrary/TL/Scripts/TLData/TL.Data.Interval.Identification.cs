/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System;

namespace ForieroEngine.Music.Training
{

    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Interval
            {
                public static partial class Identification
                {
                    public class Data : Interval.Data
                    {
                        public Enums.ToneEnum lowTone;
                        public Enums.ToneEnum highTone;
                        public int interval;

                        public int lowPitch;
                        public int highPitch;
                    }

                    public static Data data = new Data();
                }
            }
        }
    }

}
