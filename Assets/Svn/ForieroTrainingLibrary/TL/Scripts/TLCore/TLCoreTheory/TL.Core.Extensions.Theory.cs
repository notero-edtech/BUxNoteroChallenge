/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training.Core.Extensions
{
    public static partial class TLCoreTheoryExtensions
    {
        public static string ToName(this TL.Enums.Scale.ScaleDegreeEnum degreeEnum)
        {
            switch (degreeEnum)
            {
                case TL.Enums.Scale.ScaleDegreeEnum.First: return "Tonic";
                case TL.Enums.Scale.ScaleDegreeEnum.Second: return "Supertonic";
                case TL.Enums.Scale.ScaleDegreeEnum.Third: return "Mediant";
                case TL.Enums.Scale.ScaleDegreeEnum.Fourth: return "Subdominant";
                case TL.Enums.Scale.ScaleDegreeEnum.Fifth: return "Dominant";
                case TL.Enums.Scale.ScaleDegreeEnum.Sixth: return "Submediant";
                case TL.Enums.Scale.ScaleDegreeEnum.Seventh: return "Subtonic";

                default: return degreeEnum.ToString();
            }
        }

        public static void Include(this TL.Enums.Chord.ChordTypeFlags chord, bool value)
        {
            chordInclusion[(int)chord] = value;
        }

        public static bool Included(this TL.Enums.Chord.ChordTypeFlags chord)
        {
            return chordInclusion[(int)chord];
        }

        public static bool[] chordInclusion = new bool[System.Enum.GetValues(typeof(TL.Enums.Chord.ChordTypeFlags)).Length];

        public static string ToName(this TL.Enums.ToneEnum root)
        {
            string result = "";
            switch (root)
            {
                case TL.Enums.ToneEnum.A:
                    result = "A";
                    break;

                //case TL.Enums.ToneEnum.ASharp:
                case TL.Enums.ToneEnum.BFlat:
                    result = "A#/Bb";
                    break;

                case TL.Enums.ToneEnum.B:
                    result = "B";
                    break;
                case TL.Enums.ToneEnum.C:
                    result = "C";
                    break;

                //case TL.Enums.ToneEnum.CSharp:
                case TL.Enums.ToneEnum.DFlat:
                    result = "C#/Db";
                    break;

                case TL.Enums.ToneEnum.E:
                    result = "E";
                    break;

                case TL.Enums.ToneEnum.F:
                    result = "F";
                    break;

                //case TL.Enums.ToneEnum.FSharp:
                case TL.Enums.ToneEnum.GFlat:
                    result = "F#/Gb";
                    break;
                case TL.Enums.ToneEnum.G:
                    result = "G";
                    break;

                //case TL.Enums.ToneEnum.GSharp:
                case TL.Enums.ToneEnum.AFlat:
                    result = "G#/Ab";
                    break;
            }
            return result;
        }

        public static void Include(this TL.Enums.ToneEnum root, bool value)
        {
            rootInclusion[(int)root] = value;
        }

        public static bool Included(this TL.Enums.ToneEnum root)
        {
            return rootInclusion[(int)root];
        }

        public static bool[] rootInclusion = new bool[System.Enum.GetValues(typeof(TL.Enums.ToneEnum)).Length];

        //public static string ToName(this TL.Enums.KeyEnum key)
        //{
        //    string result = "";
        //    switch (key)
        //    {
        //        case TL.Enums.KeyEnum.CFlatMajor:
        //            result = "Cb Major";
        //            break;
        //        case TL.Enums.KeyEnum.GFlatMajorEFlatMinor:
        //            result = "Gb Major / Eb Minor";
        //            break;
        //        case TL.Enums.KeyEnum.DFlatMajorBFlatMinor:
        //            result = "Db Major / Bb Minor";
        //            break;
        //        case TL.Enums.KeyEnum.AFlatMajorFMinor:
        //            result = "Ab Major / F Minor";
        //            break;
        //        case TL.Enums.KeyEnum.EFlatMajorCMinor:
        //            result = "Eb Major / C Minor";
        //            break;
        //        case TL.Enums.KeyEnum.BFlatMajorGMinor:
        //            result = "Bb Major / G Minor";
        //            break;
        //        case TL.Enums.KeyEnum.FMajorDMinor:
        //            result = "F Major / D Minor";
        //            break;
        //        case TL.Enums.KeyEnum.CMajorAMinor:
        //            result = "C Major / A Minor";
        //            break;
        //        case TL.Enums.KeyEnum.GMajorEMinor:
        //            result = "G Major / E Minor";
        //            break;
        //        case TL.Enums.KeyEnum.DMajorBMinor:
        //            result = "D Major / B Minor";
        //            break;
        //        case TL.Enums.KeyEnum.AMajorFSharpMinor:
        //            result = "A Major / F# Minor";
        //            break;
        //        case TL.Enums.KeyEnum.EMajorCSharpMinor:
        //            result = "E Major / C# Minor";
        //            break;
        //        case TL.Enums.KeyEnum.BMajorGSharpMinor:
        //            result = "B Major / G# Minor";
        //            break;
        //        case TL.Enums.KeyEnum.CSharpMajor:
        //            result = "C# Major";
        //            break;
        //    }
        //    return result;
        //}

        //public static void Include(this TL.Enums.KeyEnum key, bool value)
        //{
        //    keyInclusion[(int)key] = value;
        //}

        //public static bool Included(this TL.Enums.KeyEnum key)
        //{
        //    return keyInclusion[(int)key];
        //}

        // public static bool[] keyInclusion = new bool[System.Enum.GetValues(typeof(TL.Enums.KeyEnum)).Length];

        //public static string ToName(this TL.Enums.TimingModifierEnum timingModifier)
        //{
        //    string result = "";
        //    switch (timingModifier)
        //    {
        //        case TL.Enums.TimingModifierEnum.Ties:
        //            result = "Ties";
        //            break;
        //        case TL.Enums.TimingModifierEnum.Swing8th:
        //            result = "Swing 8th";
        //            break;
        //        case TL.Enums.TimingModifierEnum.Swing16th:
        //            result = "Swing 16th";
        //            break;
        //    }
        //    return result;
        //}

        //public static void Include(this TL.Enums.TimingModifierEnum timingModifierEnum, bool value)
        //{
        //    timingModifierInclusion[(int)timingModifierEnum] = value;
        //}

        //public static bool Included(this TL.Enums.TimingModifierEnum timingModifierEnum)
        //{
        //    return timingModifierInclusion[(int)timingModifierEnum];
        //}

        //public static bool[] timingModifierInclusion = new bool[System.Enum.GetValues(typeof(TL.Enums.TimingModifierEnum)).Length];

        // time signature //
        public static string ToName(this TL.Enums.TimeSignatureEnum timeSignatureEnum)
        {
            string result = "";
            switch (timeSignatureEnum)
            {
                case TL.Enums.TimeSignatureEnum.TwoTwo:
                    result = "2/2";
                    break;
                case TL.Enums.TimeSignatureEnum.ThreeTwo:
                    result = "3/2";
                    break;
                case TL.Enums.TimeSignatureEnum.FourTwo:
                    result = "4/2";
                    break;
                case TL.Enums.TimeSignatureEnum.TwoFour:
                    result = "2/4";
                    break;
                case TL.Enums.TimeSignatureEnum.ThreeFour:
                    result = "3/4";
                    break;
                case TL.Enums.TimeSignatureEnum.FourFour:
                    result = "4/4";
                    break;
                case TL.Enums.TimeSignatureEnum.FiveFour:
                    result = "5/4";
                    break;
                case TL.Enums.TimeSignatureEnum.ThreeEight:
                    result = "3/8";
                    break;
                case TL.Enums.TimeSignatureEnum.FourEight:
                    result = "4/8";
                    break;
                case TL.Enums.TimeSignatureEnum.FiveEight:
                    result = "5/8";
                    break;
                case TL.Enums.TimeSignatureEnum.SixEight:
                    result = "6/8";
                    break;
                case TL.Enums.TimeSignatureEnum.SevenEight:
                    result = "7/8";
                    break;
                case TL.Enums.TimeSignatureEnum.NineEight:
                    result = "9/8";
                    break;
                case TL.Enums.TimeSignatureEnum.TwelveEight:
                    result = "12/8";
                    break;
            }
            return result;
        }

        public static void Include(this TL.Enums.TimeSignatureEnum timeSignatureEnum, bool value)
        {
            timeSignatureInclusion[(int)timeSignatureEnum] = value;
        }

        public static bool Include(this TL.Enums.TimeSignatureEnum timeSignatureEnum)
        {
            return timeSignatureInclusion[(int)timeSignatureEnum];
        }

        public static bool[] timeSignatureInclusion = new bool[System.Enum.GetValues(typeof(TL.Enums.TimeSignatureEnum)).Length];
    }
}

