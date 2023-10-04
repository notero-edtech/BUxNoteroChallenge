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
            public static class Interval
            {
                [Flags]
                public enum IntervalFlags
                {
                    Unison = DegreeFlags.I,
                    Minor2nd = DegreeFlags.IImin,
                    Major2nd = DegreeFlags.IImaj,
                    Minor3rd = DegreeFlags.IIImin,
                    Major3rd = DegreeFlags.IIImaj,
                    Perfect4th = DegreeFlags.IVper,
                    Diminished5th = DegreeFlags.Vdim,
                    Perfect5th = DegreeFlags.Vper,
                    Augmented5th = DegreeFlags.Vaug,
                    Minor6th = DegreeFlags.VImin,
                    Major6th = DegreeFlags.VImaj,
                    Minor7th = DegreeFlags.VIImin,
                    Major7th = DegreeFlags.VIImaj,
                    Octave = DegreeFlags.VIII
                }

                public enum CommonToneEnum
                {
                    FirstTone,
                    LastTone,
                    FirstOrLastTone,
                    NearByFirstTone,
                    NearByLastTone,
                    NearByFirstOrLastTone,
                    Undefined = int.MaxValue
                }

                public enum NearByMaxDistanceEnum
                {
                    Minor2nd = 1,
                    Major2nd = 2,
                    Minor3rd = 3,
                    Major3rd = 4,
                    Perfect4th = 5,
                    Diminished5th = 6,
                    Perfect5th = 7,
                    Minor6th = 8,
                    Major6th = 9,
                    Minor7th = 10,
                    Major7th = 11
                }
            }
        }
    }
}
