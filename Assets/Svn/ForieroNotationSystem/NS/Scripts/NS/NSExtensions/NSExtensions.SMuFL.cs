/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.SMuFL.Extensions;
using ForieroEngine.Music.SMuFL.Ranges;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static class NSSMuFLExtensions
    {
        public static string ToSMuFL(this TimeSignatureEnum e)
        {
            string s = "";
            switch (e)
            {
                case TimeSignatureEnum.Common: s = TimeSignatures.TimeSigCommon.ToCharString(); break;
                case TimeSignatureEnum.Cut: s = TimeSignatures.TimeSigCutCommon.ToCharString(); break;
            }
            return s;
        }

        public static string ToSMuFL(this FlagEnum e, StemEnum s)
        {
            string result = "";
            int enumShift = (s == StemEnum.Down ? 1 : 0);
            switch (e)
            {
                case FlagEnum.Item8th: result = ((Flags)((int)Flags.Flag8thUp + enumShift)).ToCharString(); break;
                case FlagEnum.Item16th: result = ((Flags)((int)Flags.Flag16thUp + enumShift)).ToCharString(); break;
                case FlagEnum.Item32nd: result = ((Flags)((int)Flags.Flag32ndUp + enumShift)).ToCharString(); break;
                case FlagEnum.Item64th: result = ((Flags)((int)Flags.Flag64thUp + enumShift)).ToCharString(); break;
            }
            return result;
        }

        public static string ToSMuFL(this BarLineEnum e)
        {
            string s = "";
            switch (e)
            {
                case BarLineEnum.Regular: s = Barlines.BarlineSingle.ToCharString(); break;
                case BarLineEnum.Dotted: s = Barlines.BarlineDotted.ToCharString(); break;
                case BarLineEnum.Dashed: s = Barlines.BarlineDashed.ToCharString(); break;
                case BarLineEnum.Heavy: s = Barlines.BarlineHeavy.ToCharString(); break;
                case BarLineEnum.HeavyHeavy: s = Barlines.BarlineHeavyHeavy.ToCharString(); break;
                case BarLineEnum.HeavyLight: s = Barlines.BarlineReverseFinal.ToCharString(); break;
                case BarLineEnum.LightHeavy: s = Barlines.BarlineFinal.ToCharString(); break;
                case BarLineEnum.LightLight: s = Barlines.BarlineDouble.ToCharString(); break;
                case BarLineEnum.Short: s = Barlines.BarlineShort.ToCharString(); break;
                case BarLineEnum.Tick: s = Barlines.BarlineTick.ToCharString(); break;
            }
            return s;
        }

        static int ToSMuFLABCSeqeunceIndex(this StepEnum stepEnum)
        {
            int i = 0;
            switch (stepEnum)
            {
                case StepEnum.Undefined:
                case StepEnum.A: i = 0; break;
                case StepEnum.B: i = 3; break;
                case StepEnum.C: i = 6; break;
                case StepEnum.D: i = 9; break;
                case StepEnum.E: i = 12; break;
                case StepEnum.F: i = 15; break;
                case StepEnum.G: i = 18; break;
            }
            return i;
        }

        static int ToSMuDoReMiSeqeunceIndex(this StepEnum stepEnum)
        {
            int i = 0;
            switch (stepEnum)
            {
                case StepEnum.Undefined:
                case StepEnum.A: i = 0; break;
                case StepEnum.B: i = 1; break;
                case StepEnum.C: i = 2; break;
                case StepEnum.D: i = 3; break;
                case StepEnum.E: i = 4; break;
                case StepEnum.F: i = 5; break;
                case StepEnum.G: i = 6; break;
            }
            return i;
        }

        static int ToSMuFLDoReMiIndex(this StepEnum stepEnum, KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined, KeyModeEnum keyModeEnum = KeyModeEnum.Undefined)
        {
            var offset = keySignatureEnum.ToStepEnum(keyModeEnum).ToSMuDoReMiSeqeunceIndex();
            return 7.Repeat(stepEnum.ToSMuDoReMiSeqeunceIndex() - offset);
        }

        public static int ToSMuFLUnicode(this ToneNamesEnum noteNamesEnum, NoteEnum noteEnum)
        {
            int i = 0;
            switch (noteNamesEnum)
            {
                case ToneNamesEnum.Undefined: throw new NotImplementedException(); break;
                case ToneNamesEnum.ToneNames:
                    {
                        if (noteEnum <= NoteEnum.Whole) i = (int)NoteNameNoteheads.NoteAWhole;
                        else if (noteEnum >= NoteEnum.Quarter) i = (int)NoteNameNoteheads.NoteABlack;
                        else i = (int)NoteNameNoteheads.NoteAHalf;
                    }
                    break;
                case ToneNamesEnum.SolfegeFixed:
                case ToneNamesEnum.SolfegeMovable:
                    {
                        if (noteEnum <= NoteEnum.Whole) i = (int)NoteNameNoteheads.NoteDoWhole;
                        else if (noteEnum >= NoteEnum.Quarter) i = (int)NoteNameNoteheads.NoteDoBlack;
                        else i = (int)NoteNameNoteheads.NoteDoHalf;
                    }
                    break;
                case ToneNamesEnum.SolfegeFixedSymbolic:
                case ToneNamesEnum.SolfegeMovableSymbolic:
                    // smufl does not define ordered symbolic doremi symbols // 
                    i = 0;
                    break;
            }
            return i;
        }

        static string ToSMuFLSymbolicDoReMi(this NoteEnum noteEnum, int doremiIndex)
        {
            ShapeNoteNoteheads shape = ShapeNoteNoteheads.NoteShapeTriangleUpBlack;
            switch (doremiIndex)
            {
                case 0: shape = ShapeNoteNoteheads.NoteShapeTriangleUpBlack; break;
                case 1: shape = ShapeNoteNoteheads.NoteShapeMoonBlack; break;
                case 2: shape = ShapeNoteNoteheads.NoteShapeDiamondBlack; break;
                case 3: shape = ShapeNoteNoteheads.NoteShapeTriangleLeftBlack; break;
                case 4: shape = ShapeNoteNoteheads.NoteShapeRoundBlack; break;
                case 5: shape = ShapeNoteNoteheads.NoteShapeSquareBlack; break;
                case 6: shape = ShapeNoteNoteheads.NoteShapeTriangleRoundBlack; break;
            }
            int symbolIndex = (int)shape;
            if (noteEnum <= NoteEnum.Half) { symbolIndex--; }
            return symbolIndex.ToCharString();
        }

        public static string ToSMuFL(this NoteHeadEnum noteHeadEnum)
        {
            string s = "";
            switch (noteHeadEnum)
            {
                case NoteHeadEnum.Do: s = NoteNameNoteheads.NoteDoWhole.ToCharString(); break;
                case NoteHeadEnum.Re: s = NoteNameNoteheads.NoteReWhole.ToCharString(); break;
                case NoteHeadEnum.Mi: s = NoteNameNoteheads.NoteMiWhole.ToCharString(); break;
                case NoteHeadEnum.Fa: s = NoteNameNoteheads.NoteFaWhole.ToCharString(); break;
                case NoteHeadEnum.FaUp: break;
                case NoteHeadEnum.So: s = NoteNameNoteheads.NoteSoWhole.ToCharString(); break;
                case NoteHeadEnum.La: s = NoteNameNoteheads.NoteLaWhole.ToCharString(); break;
                case NoteHeadEnum.Ti: s = NoteNameNoteheads.NoteTiWhole.ToCharString(); break;
                case NoteHeadEnum.Slash: s = Noteheads.NoteheadCircleSlash.ToCharString(); break;
                case NoteHeadEnum.Triangle: s = Noteheads.NoteheadTriangleUpBlack.ToCharString(); break;
                case NoteHeadEnum.Diamond:  s = Noteheads.NoteheadDiamondBlack.ToCharString(); break;
                case NoteHeadEnum.Square: s = Noteheads.NoteheadSquareBlack.ToCharString(); break;
                case NoteHeadEnum.Cross: break;
                case NoteHeadEnum.X: s = Noteheads.NoteheadXBlack.ToCharString(); break;
                case NoteHeadEnum.CircleX:  s = Noteheads.NoteheadCircleX.ToCharString(); break;
                case NoteHeadEnum.InvertedTriangle:  s = Noteheads.NoteheadTriangleDownBlack.ToCharString(); break;
                case NoteHeadEnum.ArrowDown:  s = Noteheads.NoteheadLargeArrowDownBlack.ToCharString(); break;
                case NoteHeadEnum.ArrowUp: s = Noteheads.NoteheadLargeArrowUpBlack.ToCharString(); break;
                case NoteHeadEnum.Circled:  s = Noteheads.NoteheadCircledBlack.ToCharString(); break;
                case NoteHeadEnum.CircleDot: break;
                case NoteHeadEnum.LeftTriangle:  s = Noteheads.NoteheadTriangleLeftBlack.ToCharString(); break;
                case NoteHeadEnum.Rectangle: break;
                case NoteHeadEnum.Slashed:  s = Noteheads.NoteheadSlashedBlack1.ToCharString(); break;
                case NoteHeadEnum.BackSlashed: s = Noteheads.NoteheadSlashedBlack2.ToCharString(); break;
                case NoteHeadEnum.Normal: break;
                case NoteHeadEnum.Cluster: break;
                case NoteHeadEnum.Other: break;
                case NoteHeadEnum.None: break;
                case NoteHeadEnum.Undefined: break;
                default: s = ""; break;
            }
            return s;
        }
        
        public static string ToSMuFL(this NoteEnum noteEnum, ToneNamesEnum noteNamesEnum, StepEnum stepEnum = StepEnum.Undefined, KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined, KeyModeEnum keyModeEnum = KeyModeEnum.Undefined, int alter = 0)
        {
            string s = "";
            switch (noteNamesEnum)
            {
                case ToneNamesEnum.Undefined:
                    s = noteEnum.ToSMuFL();
                    break;
                case ToneNamesEnum.ToneNames:
                    if (stepEnum == StepEnum.Undefined)
                    {
                        Debug.LogError("For ToneNamesNoteHead you need to define StepEnum otherwise Normal noteheads are returned back.");
                        s = noteEnum.ToSMuFL(ToneNamesEnum.Undefined);
                    }
                    else
                    {
                        var stepIndex = stepEnum.ToSMuFLABCSeqeunceIndex();
                        var unicode = noteNamesEnum.ToSMuFLUnicode(noteEnum);
                        s = (unicode + stepIndex + alter).ToCharString();
                    }
                    break;
                case ToneNamesEnum.SolfegeFixed:
                    if (stepEnum == StepEnum.Undefined)
                    {
                        Debug.LogError("For FixedSolfegeNoteHead you need to define StepEnum otherwise Normal noteheads are returned back.");
                        s = noteEnum.ToSMuFL(ToneNamesEnum.Undefined);
                    }
                    else
                    {
                        var stepIndex = stepEnum.ToSMuFLDoReMiIndex();
                        var unicode = noteNamesEnum.ToSMuFLUnicode(noteEnum);
                        s = (unicode + stepIndex).ToCharString();
                    }
                    break;
                case ToneNamesEnum.SolfegeMovable:
                    if (stepEnum == StepEnum.Undefined || keySignatureEnum == KeySignatureEnum.Undefined)
                    {
                        Debug.LogError("For MovableSolfegeNoteHead you need to define StepEnum and KeysSignature otherwise Normal noteheads are returned back.");
                        s = noteEnum.ToSMuFL(ToneNamesEnum.Undefined);
                    }
                    else
                    {
                        var stepIndex = stepEnum.ToSMuFLDoReMiIndex(keySignatureEnum, keyModeEnum);
                        var unicode = noteNamesEnum.ToSMuFLUnicode(noteEnum);
                        s = (unicode + stepIndex).ToCharString();
                    }
                    break;
                case ToneNamesEnum.SolfegeFixedSymbolic:
                    if (stepEnum == StepEnum.Undefined)
                    {
                        Debug.LogError("For SymbolicFixedSolfegeNoteHead you need to define StepEnum otherwise Normal noteheads are returned back.");
                        s = noteEnum.ToSMuFL(ToneNamesEnum.Undefined);
                    }
                    else
                    {
                        var stepIndex = stepEnum.ToSMuFLDoReMiIndex();
                        s = noteEnum.ToSMuFLSymbolicDoReMi(stepIndex);
                    }
                    break;
                case ToneNamesEnum.SolfegeMovableSymbolic:
                    if (stepEnum == StepEnum.Undefined || keySignatureEnum == KeySignatureEnum.Undefined)
                    {
                        Debug.LogError("For SymbolicMovableSolfegeNoteHead you need to define StepEnum and KeySignature otherwise Normal noteheads are returned back.");
                        s = noteEnum.ToSMuFL(ToneNamesEnum.Undefined);
                    }
                    else
                    {
                        var stepIndex = stepEnum.ToSMuFLDoReMiIndex(keySignatureEnum, keyModeEnum);
                        s = noteEnum.ToSMuFLSymbolicDoReMi(stepIndex);
                    }
                    break;
            }
            return s;
        }

        public static string ToSMuFL(this RestEnum e)
        {
            string s = "";
            switch (e)
            {
                case RestEnum.Breve: s = Rests.RestDoubleWhole.ToCharString(); break;
                case RestEnum.Whole: s = Rests.RestWhole.ToCharString(); break;
                case RestEnum.Half: s = Rests.RestHalf.ToCharString(); break;
                case RestEnum.Quarter: s = Rests.RestQuarter.ToCharString(); break;
                case RestEnum.Item8th: s = Rests.Rest8th.ToCharString(); break;
                case RestEnum.Item16th: s = Rests.Rest16th.ToCharString(); break;
                case RestEnum.Item32nd: s = Rests.Rest32nd.ToCharString(); break;
                case RestEnum.Item64th: s = Rests.Rest64th.ToCharString(); break;
                case RestEnum.Item128th: s = Rests.Rest128th.ToCharString(); break;
                case RestEnum.Item256th: s = Rests.Rest256th.ToCharString(); break;
                case RestEnum.Item512th: s = Rests.Rest512th.ToCharString(); break;
                case RestEnum.Item1024th: s = Rests.Rest1024th.ToCharString(); break;
                case RestEnum.Undefined: UnityEngine.Debug.LogError("RestEnum.ToSMuFL not defined! " + e.ToString()); break;
            }
            return s;
        }

        public static string ToSMuFL(this NoteEnum e)
        {
            string s = "";
            switch (e)
            {
                case NoteEnum.Long: s = Noteheads.NoteheadDoubleWhole.ToCharString(); break;
                case NoteEnum.Breve: s = Noteheads.NoteheadDoubleWhole.ToCharString(); break;
                case NoteEnum.Whole: s = Noteheads.NoteheadWhole.ToCharString(); break;
                case NoteEnum.Half: s = Noteheads.NoteheadHalf.ToCharString(); break;
                default: s = Noteheads.NoteheadBlack.ToCharString(); break;
            }
            return s;
        }

        public static string ToSMuFL(this NoteEnum e, StemEnum stemEnum, int dots)
        {
            string s = "";

            int stemDown = stemEnum == StemEnum.Down ? 1 : 0;

            switch (e)
            {
                case NoteEnum.Long: s = MetronomeMarks.MetNoteDoubleWhole.ToCharString(); break;
                case NoteEnum.Breve: s = MetronomeMarks.MetNoteDoubleWhole.ToCharString(); break;
                case NoteEnum.Whole: s = MetronomeMarks.MetNoteWhole.ToCharString(); break;
                case NoteEnum.Half: s = (MetronomeMarks.MetNoteHalfUp + stemDown).ToCharString(); break;
                case NoteEnum.Quarter: s = (MetronomeMarks.MetNoteQuarterUp + stemDown).ToCharString(); break;
                case NoteEnum.Item8th: s = (MetronomeMarks.MetNote8thUp + stemDown).ToCharString(); break;
                case NoteEnum.Item16th: s = (MetronomeMarks.MetNote16thUp + stemDown).ToCharString(); break;
                case NoteEnum.Item32nd: s = (MetronomeMarks.MetNote32ndUp + stemDown).ToCharString(); break;
                case NoteEnum.Item64th: s = (MetronomeMarks.MetNote64thUp + stemDown).ToCharString(); break;
                case NoteEnum.Item128th: s = (MetronomeMarks.MetNote128thUp + stemDown).ToCharString(); break;
                case NoteEnum.Item256th: s = (MetronomeMarks.MetNote256thUp + stemDown).ToCharString(); break;
                case NoteEnum.Item512th: s = (MetronomeMarks.MetNote512thUp + stemDown).ToCharString(); break;
                case NoteEnum.Item1024th: s = (MetronomeMarks.MetNote1024thUp + stemDown).ToCharString(); break;
                default: s = Noteheads.NoteheadBlack.ToCharString(); break;
            }

            for (int i = 0; i < dots; i++)
            {
                s += MetronomeMarks.MetAugmentationDot.ToCharString();
            }

            return s;
        }

        public static int ToSMuFLUnicode(this NoteEnum e)
        {
            var r = 0;
            switch (e)
            {
                case NoteEnum.Long: r = (int)Noteheads.NoteheadDoubleWhole; break;
                case NoteEnum.Breve: r = (int)Noteheads.NoteheadDoubleWhole; break;
                case NoteEnum.Whole: r = (int)Noteheads.NoteheadWhole; break;
                case NoteEnum.Half: r = (int)Noteheads.NoteheadHalf; break;
                default: r = (int)Noteheads.NoteheadBlack; break;
            }
            return r;
        }

        public static string ToSMuFL(this AccidentalEnum e)
        {
            string s = "";
            switch (e)
            {
                case AccidentalEnum.Undefined: s = ""; break;
                case AccidentalEnum.DoubleFlat: s = StandardAccidentals12Edo.AccidentalDoubleFlat.ToCharString(); break;
                case AccidentalEnum.Flat: s = StandardAccidentals12Edo.AccidentalFlat.ToCharString(); break;
                case AccidentalEnum.Natural: s = StandardAccidentals12Edo.AccidentalNatural.ToCharString(); break;
                case AccidentalEnum.Sharp: s = StandardAccidentals12Edo.AccidentalSharp.ToCharString(); break;
                case AccidentalEnum.DoubleSharp: s = StandardAccidentals12Edo.AccidentalDoubleSharp.ToCharString(); break;
            }
            return s;
        }

        public static string ToSMuFL(this StaveEnum e)
        {
            string s = "";
            switch (e)
            {
                case StaveEnum.One: s = Staves.Staff1LineWide.ToCharString(); break;
                case StaveEnum.Two: s = Staves.Staff2LinesWide.ToCharString(); break;
                case StaveEnum.Three: s = Staves.Staff3LinesWide.ToCharString(); break;
                case StaveEnum.Four: s = Staves.Staff4LinesWide.ToCharString(); break;
                case StaveEnum.Five: s = Staves.Staff5LinesWide.ToCharString(); break;
                case StaveEnum.Six: s = Staves.Staff6LinesWide.ToCharString(); break;
            }
            return s;
        }

        public static string ToSMuFL(this ClefEnum e, int octavechange = 0)
        {
            string s = "";
            switch (e)
            {
                case ClefEnum.Undefined: s = ""; break;
                case ClefEnum.Treble:
                    switch (octavechange)
                    {
                        case -1: s = Clefs.GClef8vb.ToCharString(); break;
                        case 1: s = Clefs.GClef8va.ToCharString(); break;
                        default: s = Clefs.GClef.ToCharString(); break;
                    }
                    break;
                case ClefEnum.Bass:
                    switch (octavechange)
                    {
                        case -1: s = Clefs.FClef8vb.ToCharString(); break;
                        case 1: s = Clefs.FClef8va.ToCharString(); break;
                        default: s = Clefs.FClef.ToCharString(); break;
                    }
                    break;
                case ClefEnum.Alto: s = Clefs.CClef.ToCharString(); break;
                case ClefEnum.Baritone: s = Clefs.CClef.ToCharString(); break;
                case ClefEnum.Soprano: s = Clefs.CClef.ToCharString(); break;
                case ClefEnum.MezzoSoprano: s = Clefs.CClef.ToCharString(); break;
                case ClefEnum.Tenor: s = Clefs.CClef.ToCharString(); break;
                case ClefEnum.C: s = Clefs.CClef.ToCharString(); break;
                case ClefEnum.TAB: s = Clefs._6stringTabClef.ToCharString(); break;
                case ClefEnum.Percussion: s = Clefs.UnpitchedPercussionClef1.ToCharString(); break;
                case ClefEnum.PercussionUnpitched: s = Clefs.UnpitchedPercussionClef1.ToCharString(); break;
            }
            return s;
        }

        public static string ToSMuFL(this PedalEnum p)
        {
            string s = "";
            switch (p)
            {
                case PedalEnum.Pedal: s = KeyboardTechniques.KeyboardPedalPed.ToCharString(); break;
                case PedalEnum.PedalLetter: s = KeyboardTechniques.KeyboardPedalP.ToCharString(); break;
                case PedalEnum.Stop: s = KeyboardTechniques.KeyboardPedalUp.ToCharString(); break;
                case PedalEnum.Sostenuto: s = KeyboardTechniques.KeyboardPedalSost.ToCharString(); break;
                case PedalEnum.SostenutoLetter: s = KeyboardTechniques.KeyboardPedalS.ToCharString(); break;
            }
            return s;
        }

        public static string ToSMuFL(this FermataEnum e, OrientationEnum o = OrientationEnum.Undefined)
        {
            string s = "";
            switch (e)
            {
                case FermataEnum.VeryShort:
                    s = (HoldsAndPauses.FermataVeryShortAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case FermataEnum.Short:
                    s = (HoldsAndPauses.FermataShortAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case FermataEnum.Normal:
                    s = (HoldsAndPauses.FermataAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case FermataEnum.Long:
                    s = (HoldsAndPauses.FermataLongAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case FermataEnum.VeryLong:
                    s = (HoldsAndPauses.FermataVeryLongAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case FermataEnum.Undefined:
                    break;
            }
            return s;
        }

        public static string ToSMuFL(this ArticulationEnum e, OrientationEnum o = OrientationEnum.Undefined)
        {
            string s = "";
            switch (e)
            {
                case ArticulationEnum.Accent:
                    s = (Articulation.ArticAccentAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.BreathMark:
                    s = (HoldsAndPauses.BreathMarkComma).ToCharString();
                    break;
                case ArticulationEnum.Caesura:
                    s = (HoldsAndPauses.Caesura).ToCharString();
                    break;
                case ArticulationEnum.DetachedLegato:
                    s = (Articulation.ArticTenutoStaccatoAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Doit:
                    //s = (Articulation.ArticAccentAbove + (p == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.FallOff:
                    //s = (Articulation.ArticAccentAbove + (p == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Plop:
                    //s = (Articulation.ArticAccentAbove + (p == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Scoop:
                    //s = (Articulation.ArticAccentAbove + (p == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Spiccato:
                    s = (Articulation.ArticStaccatissimoAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Staccatissimo:
                    s = (Articulation.ArticStaccatissimoWedgeAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Staccato:
                    s = (Articulation.ArticStaccatoAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Stress:
                    s = (Articulation.ArticStressAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.StrongAccent:
                    s = (Articulation.ArticMarcatoAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Tenuto:
                    s = (Articulation.ArticTenutoAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Unstress:
                    s = (Articulation.ArticUnstressAbove + (o == OrientationEnum.Inverted ? 1 : 0)).ToCharString();
                    break;
                case ArticulationEnum.Undefined:
                    break;
            }
            return s;
        }
    }
}
