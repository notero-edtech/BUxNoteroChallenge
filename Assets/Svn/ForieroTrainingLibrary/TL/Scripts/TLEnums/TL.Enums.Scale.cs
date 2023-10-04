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
            public static class Scale
            {
                [Flags]
                public enum ScaleFlags
                {
                    IonianModeI = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImaj + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImaj + DegreeFlags.VIImaj + DegreeFlags.VIII,
                    DorianModeII = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImin + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImaj + DegreeFlags.VIImin + DegreeFlags.VIII,
                    PhrygianModeIII = DegreeFlags.I + DegreeFlags.IImin + DegreeFlags.IIImin + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImin + DegreeFlags.VIImin + DegreeFlags.VIII,
                    LydianModeIV = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImaj + DegreeFlags.IVaug + DegreeFlags.Vper + DegreeFlags.VImaj + DegreeFlags.VIImaj + DegreeFlags.VIII,
                    MixolydianModeV = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImaj + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImaj + DegreeFlags.VIImin + DegreeFlags.VIII,
                    AeolianModeVI = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImin + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImin + DegreeFlags.VIImin + DegreeFlags.VIII,
                    LocrianModeVII = DegreeFlags.I + DegreeFlags.IImin + DegreeFlags.IIImin + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImin + DegreeFlags.VIImin + DegreeFlags.VIII,
                    Major = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImaj + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImaj + DegreeFlags.VIImaj + DegreeFlags.VIII,
                    HarnomicMajor = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImaj + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImin + DegreeFlags.VIImaj + DegreeFlags.VIII,
                    HarmonicMinor = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImin + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImin + DegreeFlags.VIImaj + DegreeFlags.VIII,
                    // melodic minor flatten VI & VII when going down //
                    MelodicMinor = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImin + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImaj + DegreeFlags.VIImaj + DegreeFlags.VIII,
                    NaturalMinor = DegreeFlags.I + DegreeFlags.IImaj + DegreeFlags.IIImin + DegreeFlags.IVper + DegreeFlags.Vper + DegreeFlags.VImin + DegreeFlags.VIImin + DegreeFlags.VIII,

                    //PentatonicMajor = 1 << 4,
                    //PentatonicMinor = 1 << 5,
                    //Algerian = 1 << 6,
                    //Arabian = 1 << 7,
                    //Balinese = 1 << 8,
                    //Byzantine = 1 << 9,
                    //Chinese = 1 << 10,
                    //Egyptian = 1 << 11,
                    //Ethiopian = 1 << 12,
                    //Hungarian = 1 << 13,
                    //Israeli = 1 << 14,
                    //Japanese = 1 << 15,
                    //Javanese = 1 << 16,
                    //Mongolian = 1 << 17,
                    //Neapolitan = 1 << 18,
                    //Persian = 1 << 19,
                    //Spanish = 1 << 20

                    // TODO: Add more
                }

                // needs to be removed //
                public enum ScaleDegreeEnum
                {
                    First,
                    FlattenedSecond,
                    Second,
                    FlattenedThird,
                    Third,
                    Fourth,
                    FlattenedFifth,
                    Fifth,
                    FlattenedSixth,
                    Sixth,
                    FlattenedSeventh,
                    Seventh
                }
            }
        }
    }
}
