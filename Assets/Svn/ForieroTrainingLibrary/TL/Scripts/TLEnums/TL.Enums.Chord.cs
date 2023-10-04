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
            public static class Chord
            {
                [Flags]
                public enum ChordTypeFlags
                {
                    Major = DegreeFlags.IIImaj + DegreeFlags.Vper,
                    Minor = DegreeFlags.IIImin + DegreeFlags.Vper,
                    Diminished = DegreeFlags.IIImin + DegreeFlags.Vdim,
                    Augmented = DegreeFlags.IIImaj + DegreeFlags.Vaug,
                    Sus2 = DegreeFlags.IImaj + DegreeFlags.Vper,
                    Sus4 = DegreeFlags.IVper + DegreeFlags.Vper,
                    Major7 = DegreeFlags.IIImaj + DegreeFlags.Vper + DegreeFlags.VIImaj,
                    Major7b = DegreeFlags.IIImaj + DegreeFlags.Vper + DegreeFlags.VIImin,
                    Minor7 = DegreeFlags.IIImin + DegreeFlags.Vper + DegreeFlags.VIImaj,
                    Minor7b = DegreeFlags.IIImin + DegreeFlags.Vper + DegreeFlags.VIImin
                }

                [Flags]
                public enum ChordInversionTypeFlags
                {
                    Root = 1 << 0,
                    First = 1 << 1,
                    Second = 1 << 2,
                    Third = 1 << 3
                }

                public enum ChordVoicingEnum
                {
                    RootPoistion,
                    ClosedVoicing,
                    OpenVoicing
                }

                [Flags]
                public enum ChordFlags
                {
                    Imaj = DegreeFlags.I + ChordTypeFlags.Major,
                    Imin = DegreeFlags.I + ChordTypeFlags.Minor,
                    Imaj7 = DegreeFlags.I + ChordTypeFlags.Major7,
                    Imin7 = DegreeFlags.I + ChordTypeFlags.Minor7,

                    IImin = DegreeFlags.IImaj + ChordTypeFlags.Minor,
                    IImin7 = DegreeFlags.IImaj + ChordTypeFlags.Minor7,

                    IIImin = DegreeFlags.IIImaj + ChordTypeFlags.Minor,

                    IVmaj = DegreeFlags.IVper + ChordTypeFlags.Major,
                    IVmin = DegreeFlags.IVper + ChordTypeFlags.Minor,

                    Vmaj = DegreeFlags.Vper + ChordTypeFlags.Major,
                    Vmin = DegreeFlags.Vper + ChordTypeFlags.Minor,
                    //VIIdim7
                }

                [Flags]
                public enum ChordProgressionFlags
                {
                    Vmaj_Imaj = ChordFlags.Vmaj + ChordFlags.Imaj,
                    Vmaj_Imin = ChordFlags.Vmaj + ChordFlags.Imin
                }
            }
        }
    }
}
