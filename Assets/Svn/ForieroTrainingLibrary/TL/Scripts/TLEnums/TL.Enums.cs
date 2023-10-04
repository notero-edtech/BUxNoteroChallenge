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
            [Flags]
            public enum PlayModeFlags
            {
                Harmonic = 1 << 0,
                Ascending = 1 << 1,
                AscendingThenDescending = 1 << 2,
                AscendingThenHarmonic = 1 << 3,
                HarmonicThenAscending = 1 << 4,
                Descending = 1 << 5,
                DescendingThenAscending = 1 << 6,
                DescendingThenHarmonic = 1 << 7,
                HarmonicThenDescending = 1 << 8,
            }

            public enum PitchRangesPresetEnum
            {
                Custom,
                Soprano,
                MezzoSoprano,
                Alto,
                ContraAlto,
                CounterTenor,
                Tenor,
                Baritone,
                Bass,
                PianoInstrument,
                GuitarInstrument,
                BassInstrument
            }

            public enum PitchRangeAndClefEnum
            {
                Octave_G3_Gb4,
                Octave_D3_Db5,
                Octave_C3_B5,
                UserVocalRange,
                Soprano_G_Clef,
                MezzoSoprano_G_Clef,
                Alto_G_Clef,
                ContraAlto_G_Clef,
                CounterTenor_G_Clef_8vb,
                Tenor_G_Clef_8vb,
                Baritone_F_Clef,
                Bass_F_Clef,
                PianoInstrument,
                GuitarInstrument_G_Clef,
                BassInstrument_F_Clef
            }

            public enum AnswerEnum
            {
                Correct,
                Wrong,
                Unanswered,
                Undefined = int.MaxValue
            }

            public enum ClefEnum
            {
                Treble,
                Bass,
                Soprano,
                MezzoSoprano,
                Alto,
                Tenor,
                Bariton,
                Percussion,
                PercussionUnpitched,
                Undefined = int.MaxValue
            }

            public enum AccidentalEnum
            {
                DoubleFlat = -2,
                Flat = -1,
                Natural = 0,
                Sharp = 1,
                DoubleSharp = 2,
                Undefined = int.MaxValue
            }

            [Flags]
            public enum DurationFlags
            {
                _dot = 1 << 0,
                Breve = 1 << 1,
                Whole = 1 << 2,
                Half = 1 << 3,
                Quarter = 1 << 4,
                Item8th = 1 << 5,
                Item16th = 1 << 6,
                Item32nd = 1 << 7,
                Item64th = 1 << 8,
                Item128th = 1 << 9,
                _tuplet3 = 1 << 10,
                _tuplet5 = 1 << 11,
                _tuplet7 = 1 << 12,
                _tupletMask = ~(_tuplet3 + _tuplet5 + _tuplet7)
            }

            [Flags]
            public enum NoteAndRestFlags
            {
                Whole = DurationFlags.Whole,
                Half = DurationFlags.Half,
                Quarter = DurationFlags.Quarter,
                Item8th = DurationFlags.Item8th,
                Item16th = DurationFlags.Item16th,
                DottedWhole = DurationFlags.Whole + DurationFlags._dot,
                DottedHalf = DurationFlags.Half + DurationFlags._dot,
                DottedQuarter = DurationFlags.Quarter + DurationFlags._dot,
                DottedItem8th = DurationFlags.Item8th + DurationFlags._dot,
                DottedItem16th = DurationFlags.Item16th + DurationFlags._dot
            }

            [Flags]
            public enum TupletNoteAndRestFlags
            {
                HalfTuplet3 = DurationFlags.Whole + DurationFlags._tuplet3,
                HalfTuplet5 = DurationFlags.Whole + DurationFlags._tuplet5,
                HalfTuplet7 = DurationFlags.Whole + DurationFlags._tuplet7,
                QuarterTuplet3 = DurationFlags.Half + DurationFlags._tuplet3,
                QuarterTuplet5 = DurationFlags.Half + DurationFlags._tuplet5,
                QuarterTuplet7 = DurationFlags.Half + DurationFlags._tuplet7,
                Item8thTuplet3 = DurationFlags.Quarter + DurationFlags._tuplet3,
                Item8thTuplet5 = DurationFlags.Quarter + DurationFlags._tuplet5,
                Item8thTuplet6 = DurationFlags.Quarter + DurationFlags._tuplet7,
                Item16thTuplet3 = DurationFlags.Item8th + DurationFlags._tuplet3,
                Item16thTuplet5 = DurationFlags.Item8th + DurationFlags._tuplet5,
                Item16thTuplet7 = DurationFlags.Item8th + DurationFlags._tuplet7
            }

            public enum KeySignatureEnum
            {
                CFlatMaj = -7,
                GFlatMaj_EFlatMin = -6,
                DFlatMaj_BFlatMin = -5,
                AFlatMaj_FMin = -4,
                EFlatMaj_CMin = -3,
                BFlatMaj_GMin = -2,
                FMaj_DMin = -1,
                CMaj_AMin = 0,
                GMaj_EMin = 1,
                DMaj_BMin = 2,
                AMaj_FSharpMin = 3,
                EMaj_CSharpMin = 4,
                BMaj_GSharpMin = 5,
                FSharpMaj_DSharpMin = 6,
                CSharpMaj = 7,
                Undefined = int.MaxValue
            }

            public enum NotationEnum
            {
                TimeProportional,
                FixedSpace
            }

            public enum PlayTonicLeadEnum
            {
                KeyNotes,
                RootChordMelodic,
                RootChordMelodicHarmonic,
                RootChordHarmonic,
                RootChordHarmonicMelodic,
                Cadence_I_IV_V_I,
                Tones123454321,
                AscendingSacle,
                Undefined = int.MaxValue
            }

            public enum RepeatModeEnum
            {
                Random,
                Clockwise,
                CounterClockwise
            }

            public enum PositionInKeyEnum
            {
                AllTones,
                ScaleTones,
                Steps_I_IV_V,
                DiatonicMatch,
                RootOfKey
            }

            public enum PickFromEnum
            {
                Random,
                Keys,
                RootTones
            }

            public enum EvaluationEnum
            {
                Kind,
                Normal,
                Strict
            }

            public enum ToneEnum
            {
                C = 0,
                CSharp = 1,
                DFlat = 1,
                D = 2,
                DSharp = 3,
                EFlat = 3,
                E = 4,
                F = 5,
                FSharp = 6,
                GFlat = 6,
                G = 7,
                GSharp = 8,
                AFlat = 8,
                A = 9,
                ASharp = 10,
                BFlat = 10,
                B = 11,
            }

            public enum RootOfQuestionEnum
            {
                AllTones,
                ScaleTones,
                Steps_I_IV_V,
                DiatonicMatch,
                RootOfKey
            }

            [Flags]
            public enum ChromaticFlags
            {
                _0 = 1 << 0,
                _1 = 1 << 1,
                _2 = 1 << 2,
                _3 = 1 << 3,
                _4 = 1 << 4,
                _5 = 1 << 5,
                _6 = 1 << 6,
                _7 = 1 << 7,
                _8 = 1 << 8,
                _9 = 1 << 9,
                _10 = 1 << 10,
                _11 = 1 << 11,
                _12 = 1 << 12
            }

            [Flags]
            public enum NumberFlags
            {
                _0 = 1 << 0,
                _1 = 1 << 1,
                _2 = 1 << 2,
                _3 = 1 << 3,
                _4 = 1 << 4,
                _5 = 1 << 5,
                _6 = 1 << 6,
                _7 = 1 << 7,
                _8 = 1 << 8,
                _9 = 1 << 9,
                _10 = 1 << 10,
                _11 = 1 << 11,
                _12 = 1 << 12
            }

            [Flags]
            public enum BeatsPerMeasureFlags
            {
                _2 = NumberFlags._2,
                _3 = NumberFlags._3,
                _4 = NumberFlags._4,
                _5 = NumberFlags._5,
                _6 = NumberFlags._6,
                _7 = NumberFlags._7,
                _9 = NumberFlags._9,
                _12 = NumberFlags._12
            }

            public enum NotePerBeatFlags
            {
                _2 = NumberFlags._2,
                _4 = NumberFlags._4,
                _5 = NumberFlags._5,
                _7 = NumberFlags._7,
                _8 = NumberFlags._8,
                _12 = NumberFlags._12
            }

            [Flags]
            public enum DegreeFlags
            {
                I = ChromaticFlags._0,
                IImin = ChromaticFlags._1,
                IImaj = ChromaticFlags._2,
                IIImin = ChromaticFlags._3,
                IIImaj = ChromaticFlags._4,
                IVper = ChromaticFlags._5,
                IVaug = ChromaticFlags._6,
                Vdim = ChromaticFlags._6,
                Vper = ChromaticFlags._7,
                Vaug = ChromaticFlags._8,
                VImin = ChromaticFlags._8,
                VImaj = ChromaticFlags._9,
                VIImin = ChromaticFlags._10,
                VIImaj = ChromaticFlags._11,
                VIII = ChromaticFlags._12,
            }

            [Flags]
            public enum ToneFlags
            {
                C = ChromaticFlags._0,
                CSharpDFlat = ChromaticFlags._1,
                D = ChromaticFlags._2,
                DSharpEFlat = ChromaticFlags._3,
                E = ChromaticFlags._4,
                F = ChromaticFlags._5,
                FSharpGFlat = ChromaticFlags._6,
                G = ChromaticFlags._7,
                GSharpAFlat = ChromaticFlags._8,
                A = ChromaticFlags._9,
                ASharpBFlat = ChromaticFlags._10,
                B = ChromaticFlags._11,
            }

            [Flags]
            public enum KeyFlags
            {
                CFlatMajor = 1 << 0,
                GFlatMajor = 1 << 1,
                EFlatMinor = 1 << 1,
                DFlatMajor = 1 << 2,
                BFlatMinor = 1 << 2,
                AFlatMajor = 1 << 3,
                FMinor = 1 << 3,
                EFlatMajor = 1 << 4,
                CMinor = 1 << 4,
                BFlatMajor = 1 << 5,
                GMinor = 1 << 5,
                FMajor = 1 << 6,
                DMinor = 1 << 6,
                CMajor = 1 << 7,
                AMinor = 1 << 7,
                GMajor = 1 << 8,
                EMinor = 1 << 8,
                DMajor = 1 << 9,
                BMinor = 1 << 9,
                AMajor = 1 << 10,
                FSharpMinor = 1 << 10,
                EMajor = 1 << 11,
                CSharpMinor = 1 << 11,
                BMajor = 1 << 12,
                GSharpMinor = 1 << 12,
                CSharpMajor = 1 << 13
            }

            public enum TimingModifierFlags
            {
                Ties = 1 << 0,
                Swing8th = 1 << 1,
                Swing16th = 1 << 2
            }

            public enum TimeSignatureEnum
            {
                TwoTwo,
                ThreeTwo,
                FourTwo,
                TwoFour,
                ThreeFour,
                FourFour,
                FiveFour,
                ThreeEight,
                FourEight,
                FiveEight,
                SixEight,
                SevenEight,
                NineEight,
                TwelveEight
            }

            public enum SettingsValidationErrorEnum
            {
                None,
                ChordsMissing,
                IntervalsMissing,
                NotesMissing,
                ScalesMissing,
                TonesMissing,
                ProgressionsMissing,
                PlayModesMissing,
                KeysMissing,
                TimeSignaturesMissing
            }

        }
    }
}
