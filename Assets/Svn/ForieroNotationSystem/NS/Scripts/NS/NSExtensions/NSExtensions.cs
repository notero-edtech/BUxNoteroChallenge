/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using UnityEngine;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensions
    {
        public static void Log(this System.Object o) => Debug.Log(o.ToString());
        public static int ToInt(this System.Enum enumValue) => System.Convert.ToInt32(enumValue);
        
        public static float GetDotsCoefficient(this int dots)
        {
            var iterator = 0f;
            var coefficient = 0f;
            while (dots > 0)
            {
                iterator++;
                coefficient += 0.5f / iterator;
                dots--;
            }
            return 1f + coefficient;
        }

        public static RestEnum ToRestEnum(this NoteEnum e) => (RestEnum)e;
        public static NoteEnum ToNoteEnum(this RestEnum e) => (NoteEnum)e;
        
        public static FlagEnum ToFlagEnum(this NoteEnum e)
        {
            return e switch
            {
                NoteEnum.Item8th => FlagEnum.Item8th,
                NoteEnum.Item16th => FlagEnum.Item16th,
                NoteEnum.Item32nd => FlagEnum.Item32nd,
                NoteEnum.Item64th => FlagEnum.Item64th,
                NoteEnum.Item128th => FlagEnum.Item128th,
                _ => FlagEnum.Undefined
            };
        }

        public static HorizontalDirectionEnum ToHorizontalDirectionEnum(this NSPlayback.NSRollingPlayback.RollingMode rollingMode)
        {
            return rollingMode switch
            {
                NSPlayback.NSRollingPlayback.RollingMode.Left => HorizontalDirectionEnum.Left,
                NSPlayback.NSRollingPlayback.RollingMode.Right => HorizontalDirectionEnum.Right,
                _ => HorizontalDirectionEnum.Undefined
            };
        }

        public static VerticalDirectionEnum ToVerticalDirectionEnum(this NSPlayback.NSRollingPlayback.RollingMode rollingMode)
        {
            return rollingMode switch
            {
                NSPlayback.NSRollingPlayback.RollingMode.Top => VerticalDirectionEnum.Up,
                NSPlayback.NSRollingPlayback.RollingMode.Bottom => VerticalDirectionEnum.Down,
                _ => VerticalDirectionEnum.Undefined
            };
        }
        
        public static HorizontalDirectionEnum ToHorizontalDirectionEnum(this NSPlayback.NSTickerPlayback.RollingMode rollingMode)
        {
            return rollingMode switch
            {
                NSPlayback.NSTickerPlayback.RollingMode.Left => HorizontalDirectionEnum.Left,
                NSPlayback.NSTickerPlayback.RollingMode.Right => HorizontalDirectionEnum.Right,
                _ => HorizontalDirectionEnum.Undefined
            };
        }
       
        public static Vector2 ToPivotAndAnchors(this PivotEnum e)
        {
            return e switch
            {
                PivotEnum.TopLeft => new Vector2(0, 1f),
                PivotEnum.TopCenter => new Vector2(0.5f, 1f),
                PivotEnum.TopRight => new Vector2(1f, 1f),
                PivotEnum.MiddleLeft => new Vector2(0, 0.5f),
                PivotEnum.MiddleCenter => new Vector2(0.5f, 0.5f),
                PivotEnum.MiddleRight => new Vector2(1f, 0.5f),
                PivotEnum.BottomLeft => new Vector2(0, 0f),
                PivotEnum.BottomCenter => new Vector2(0.5f, 0f),
                PivotEnum.BottomRight => new Vector2(1f, 0f),
                _ => Vector2.zero
            };
        }

        public static int ToBaseStaveIndex(this NSClef.Options clef)
        {
            int result = 0;
            switch (clef.clefEnum)
            {
                case ClefEnum.Treble:
                    if (clef.staveLine == 0) result = 34;
                    else result = 36 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue);
                    break;
                case ClefEnum.Bass:
                    if (clef.staveLine == 0) result = 22;
                    else result = 28 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue);
                    break;
                case ClefEnum.Soprano:
                    if (clef.staveLine == 0) result = 32;
                    else result = 32 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue);
                    break;
                case ClefEnum.MezzoSoprano:
                    if (clef.staveLine == 0) result = 30;
                    else result = 32 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue);
                    break;
                case ClefEnum.Alto:
                    if (clef.staveLine == 0) result = 28;
                    else result = 32 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue);
                    break;
                case ClefEnum.Tenor:
                    if (clef.staveLine == 0) result = 26;
                    else result = 32 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue);
                    break;
                case ClefEnum.Baritone:
                    if (clef.staveLine == 0) result = 24;
                    else result = 32 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue);
                    break;
                case ClefEnum.Percussion: result = 34; break;
                case ClefEnum.PercussionUnpitched: result = 28; break;
                case ClefEnum.C: result = 32 - 2 * Mathf.Clamp(clef.staveLine - 1, 0, int.MaxValue); break;
                case ClefEnum.TAB: result = 28; break;
                case ClefEnum.Undefined: result = 28; break;
            }

            result += clef.octaveChange * 7;

            return result;
        }

        public static StepEnum ToStepEnum(this KeySignatureEnum keySignatureEnum, KeyModeEnum keyModeEnum = KeyModeEnum.Undefined)
        {
            StepEnum r = StepEnum.Undefined;
            if (keySignatureEnum == KeySignatureEnum.Undefined) keySignatureEnum = KeySignatureEnum.CMaj_AMin;
            if (keyModeEnum == KeyModeEnum.Undefined) keyModeEnum = KeyModeEnum.Major;
            switch (keySignatureEnum)
            {
                case KeySignatureEnum.CFlatMaj: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.A; else r = StepEnum.C; break;
                case KeySignatureEnum.GFlatMaj_EFlatMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.E; else r = StepEnum.G; break;
                case KeySignatureEnum.DFlatMaj_BFlatMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.B; else r = StepEnum.D; break;
                case KeySignatureEnum.AFlatMaj_FMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.F; else r = StepEnum.A; break;
                case KeySignatureEnum.EFlatMaj_CMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.C; else r = StepEnum.E; break;
                case KeySignatureEnum.BFlatMaj_GMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.G; else r = StepEnum.B; break;
                case KeySignatureEnum.FMaj_DMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.D; else r = StepEnum.F; break;
                case KeySignatureEnum.CMaj_AMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.A; else r = StepEnum.C; break;
                case KeySignatureEnum.GMaj_EMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.E; else r = StepEnum.G; break;
                case KeySignatureEnum.DMaj_BMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.B; else r = StepEnum.D; break;
                case KeySignatureEnum.AMaj_FSharpMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.F; else r = StepEnum.A; break;
                case KeySignatureEnum.EMaj_CSharpMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.C; else r = StepEnum.E; break;
                case KeySignatureEnum.BMaj_GSharpMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.G; else r = StepEnum.B; break;
                case KeySignatureEnum.FSharpMaj_DSharpMin: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.D; else r = StepEnum.F; break;
                case KeySignatureEnum.CSharpMaj: if (keyModeEnum == KeyModeEnum.Minor) r = StepEnum.A; else r = StepEnum.C; break;
            }
            return r;
        }

        public static int ToKeySignatureSharpsIndex(this ClefEnum e)
        {
            return e switch
            {
                ClefEnum.Treble => 8,
                ClefEnum.Bass => 6,
                ClefEnum.Soprano => 0,
                ClefEnum.MezzoSoprano => 0,
                ClefEnum.Alto => 0,
                ClefEnum.Tenor => 0,
                ClefEnum.Baritone => 0,
                ClefEnum.Percussion => 0,
                ClefEnum.PercussionUnpitched => 0,
                ClefEnum.Undefined => 0,
                _ => 0
            };
        }

        public static int ToKeySignatureFlatsIndex(this ClefEnum e)
        {
            return e switch
            {
                ClefEnum.Treble => 4,
                ClefEnum.Bass => 2,
                ClefEnum.Soprano => 0,
                ClefEnum.MezzoSoprano => 0,
                ClefEnum.Alto => 0,
                ClefEnum.Tenor => 0,
                ClefEnum.Baritone => 0,
                ClefEnum.Percussion => 0,
                ClefEnum.PercussionUnpitched => 0,
                ClefEnum.Undefined => 0,
                _ => 0
            };
        }

        public static VerticalDirectionEnum ToStemVerticalDirectionEnum(this note aNote)
        {
            var result = VerticalDirectionEnum.Undefined;
            if (aNote.stem != null)
            {
                switch (aNote.stem.Value)
                {
                    case stemvalue.up: result = VerticalDirectionEnum.Up; break;
                    case stemvalue.down: result = VerticalDirectionEnum.Down; break;
                }
            }
            return result;
        }

        public static NoteEnum ToNoteEnum(this notetypevalue notetypevalue)
        {
            var result = NoteEnum.Undefined;
            switch (notetypevalue)
            {
                case notetypevalue.@long: result = NoteEnum.Long; break;
                case notetypevalue.breve: result = NoteEnum.Breve; break;
                case notetypevalue.whole: result = NoteEnum.Whole; break;
                case notetypevalue.half: result = NoteEnum.Half; break;
                case notetypevalue.quarter: result = NoteEnum.Quarter; break;
                case notetypevalue.eighth: result = NoteEnum.Item8th; break;
                case notetypevalue.Item16th: result = NoteEnum.Item16th; break;
                case notetypevalue.Item32nd: result = NoteEnum.Item32nd; break;
                case notetypevalue.Item64th: result = NoteEnum.Item64th; break;
                case notetypevalue.Item128th: result = NoteEnum.Item128th; break;
                case notetypevalue.Item256th: result = NoteEnum.Item256th; break;
                case notetypevalue.Item512th: result = NoteEnum.Item512th; break;
                case notetypevalue.Item1024th: result = NoteEnum.Item128th; break;
            }
            return result;
        }

        public static NoteEnum ToNoteEnum(this note aNote)
        {
            var result = NoteEnum.Undefined;
            if (aNote.type != null) { result = aNote.type.Value.ToNoteEnum(); }
            else { Debug.LogError("Note does not contain 'type'"); }
            return result;
        }

        public static RestEnum ToRestEnum(this note aNote)
        {
            var result = RestEnum.Undefined;
            if (aNote.ItemsElementName.Contains(ItemsChoiceType1.rest))
            {
                if (aNote.type != null)
                {
                    result = aNote.type.Value switch
                    {
                        notetypevalue.breve => RestEnum.Breve,
                        notetypevalue.whole => RestEnum.Whole,
                        notetypevalue.half => RestEnum.Half,
                        notetypevalue.quarter => RestEnum.Quarter,
                        notetypevalue.eighth => RestEnum.Item8th,
                        notetypevalue.Item16th => RestEnum.Item16th,
                        notetypevalue.Item32nd => RestEnum.Item32nd,
                        notetypevalue.Item64th => RestEnum.Item64th,
                        notetypevalue.Item128th => RestEnum.Item128th,
                        notetypevalue.Item256th => RestEnum.Item256th,
                        notetypevalue.Item512th => RestEnum.Item512th,
                        notetypevalue.Item1024th => RestEnum.Item128th,
                        _ => result
                    };
                }
                else
                {
                    Debug.LogError("Rest does not contain 'type'");
                }
            }
            return result;
        }

        public static OctaveEnum ToOctaveEnum(this note aNote)
        {
            var result = OctaveEnum.Undefined;
            if (aNote.ItemsElementName.Contains(ItemsChoiceType1.pitch))
            {
                var pitch = (aNote.Items[aNote.ItemsElementName.IndexOf(ItemsChoiceType1.pitch)] as pitch);
                var octave = pitch.octave.Exists() ? int.Parse(pitch.octave) + 1 : 0;
                result = octave switch
                {
                    1 => OctaveEnum.First,
                    2 => OctaveEnum.Second,
                    3 => OctaveEnum.Third,
                    4 => OctaveEnum.Fourth,
                    5 => OctaveEnum.Fifth,
                    6 => OctaveEnum.Sixth,
                    7 => OctaveEnum.Seventh,
                    8 => OctaveEnum.Eight,
                    _ => result
                };
            }
            return result;
        }

        public static StepEnum ToStepEnum(this note aNote)
        {
            var result = StepEnum.Undefined;
            if (aNote.ItemsElementName.Contains(ItemsChoiceType1.pitch))
            {
                result = (aNote.Items[aNote.ItemsElementName.IndexOf(ItemsChoiceType1.pitch)] as pitch)?.step switch
                {
                    step.A => StepEnum.A,
                    step.B => StepEnum.B,
                    step.C => StepEnum.C,
                    step.D => StepEnum.D,
                    step.E => StepEnum.E,
                    step.F => StepEnum.F,
                    step.G => StepEnum.G,
                    _ => result
                };
            }
            return result;
        }

        public static AccidentalEnum ToAccidentalEnum(this note aNote)
        {
            return aNote?.accidental?.Value switch
            {
                accidentalvalue.sharp => AccidentalEnum.Sharp,
                accidentalvalue.doublesharp => AccidentalEnum.DoubleSharp,
                accidentalvalue.natural => AccidentalEnum.Natural,
                accidentalvalue.flat => AccidentalEnum.Flat,
                accidentalvalue.flatflat => AccidentalEnum.DoubleFlat,
                _ => AccidentalEnum.Undefined
            };
        }

        public static YesNoEnum ToAccidentalParenthesisEnum(this note aNote)
        {
            if (aNote?.accidental == null) return YesNoEnum.Undefined;
            return aNote.accidental.HasParenthesis() ? YesNoEnum.Yes : YesNoEnum.No;
        }

        public static float ToDurationMultiplier(this ArticulationEnum a) =>
            a switch
            {
                ArticulationEnum.Staccatissimo => NSPlaybackSettings.StaccatissimoDurationMultiplier,
                ArticulationEnum.Staccato => NSPlaybackSettings.StaccatoDurationMultiplier,
                _ => 1f
            };

        public static ArticulationEnum ToArticulationEnum(this note aNote)
        {
            var r = ArticulationEnum.Undefined;
            if (aNote.notations is not { Length: > 0 }) return r;
            var articulations = aNote.notations[0].Items.ObjectsOfType<articulations>();
            if (articulations == null) return r;
            foreach (var articulation in articulations)
            {
                foreach (var a in articulation.ItemsElementName)
                {
                    r = a.ToNS();
                    return r;
                }
            }
            return r;
        }

        public static NSFingering.Options? ToFingeringOptions(this note aNote)
        {
            if (aNote.notations is not { Length: > 0 }) return null;
            var t = aNote.notations[0].Items.ObjectOfType<technical>();
            var f = t?.Items.ObjectOfType<fingering>();
            if (f == null) return null;
            return new NSFingering.Options()
            {
                Number =  int.Parse(f.Value),
                PositionX = f.defaultxSpecified ? (float)f.defaultx : 0,
                PositionY = f.defaultySpecified ? (float)f.defaulty : 0,
                Placement = f.placementSpecified ? (PlacementEnum)f.placement : PlacementEnum.Undefined
            };
        }

        public static FermataEnum ToFermataEnum(this note aNote)
        {
            var r = FermataEnum.Undefined;
            if (aNote.notations is not { Length: > 0 }) return r;
            var fermatas = aNote.notations[0].Items.ObjectsOfType<fermata>();
            if (fermatas == null) return r;

            foreach (var f in fermatas)
            {
                if (f.typeSpecified)
                {
                    switch (f.Value)
                    {
                        case fermatashape.normal: r = FermataEnum.Normal; break;
                        case fermatashape.angled: r = FermataEnum.Short; break;
                        case fermatashape.square: r = FermataEnum.Long; break;
                        case fermatashape.doubleangled: r = FermataEnum.VeryShort; break;
                        case fermatashape.doublesquare: r = FermataEnum.VeryLong; break;
                        case fermatashape.doubledot: //r = FermataEnum.Normal;
                            break;
                        case fermatashape.halfcurve: //r = FermataEnum.Normal;
                            break;
                        case fermatashape.curlew: //r = FermataEnum.Normal;
                            break;
                        case fermatashape.Item: //r = FermataEnum.Normal;
                            break;
                    }
                }
            }
            return r;
        }
    }
}
