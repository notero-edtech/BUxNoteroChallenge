/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.MusicXML.Xsd;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public partial class NS
    {
        public static int NoteIndex(StepEnum aStep, OctaveEnum anOctave) => (int)anOctave * 7 + (int)aStep;
        public static int MidiIndex(StepEnum aStep, OctaveEnum anOctave) => (int)anOctave * 12 + (int)aStep;
    }

    public static partial class NSExtensionsMusicXML
    {
        public static TimeSignatureEnum ToNS(this timesymbol symbol)
        {
            TimeSignatureEnum r = TimeSignatureEnum.Undefined;
            switch (symbol)
            {
                case timesymbol.common: r = TimeSignatureEnum.Common; break;
                case timesymbol.cut: r = TimeSignatureEnum.Cut; break;
                case timesymbol.dottednote: r = TimeSignatureEnum.DottedNote; break;
                case timesymbol.normal: r = TimeSignatureEnum.Normal; break;
                case timesymbol.note: r = TimeSignatureEnum.Note; break;
                case timesymbol.singlenumber: r = TimeSignatureEnum.SingleNumber; break;
            }
            return r;
        }

        public static ClefEnum ToNS(this clefsign clef, int staveLine)
        {
            switch (clef)
            {
                case clefsign.C:
                    switch (staveLine)
                    {
                        case 1: return ClefEnum.Soprano;
                        case 2: return ClefEnum.MezzoSoprano;
                        case 3: return ClefEnum.Alto;
                        case 4: return ClefEnum.Tenor;
                        case 5: return ClefEnum.Baritone;
                        case 0:
                        default: return ClefEnum.C; 
                    }
                case clefsign.F: return ClefEnum.Bass; 
                case clefsign.G: return ClefEnum.Treble; 
                case clefsign.jianpu: return ClefEnum.Jianpu; 
                case clefsign.percussion: return ClefEnum.Percussion; 
                case clefsign.TAB: return ClefEnum.TAB; 
                case clefsign.none:
                default: return ClefEnum.Undefined;
            }
        }

        public static PedalEnum ToNS(this pedaltype pedal)
        {
            var r = PedalEnum.Undefined;
            switch (pedal)
            {
                case pedaltype.start: r = PedalEnum.Pedal; break;
                case pedaltype.stop: r = PedalEnum.Stop; break;
                case pedaltype.sostenuto: r = PedalEnum.Sostenuto; break;
                case pedaltype.change:
                case pedaltype.@continue:
                    Debug.LogError("Unsupported pedal type : " + pedal.ToString());
                    break;
            }
            return r;
        }

        public static KeySignatureEnum ToNSKeySignatureEnum(this key key)
        {
            return (KeySignatureEnum)int.Parse(key.ItemsElementName.ValueOf<string>(ItemsChoiceType10.fifths, key.Items));
        }

        public static KeyModeEnum ToNSKeyModeEnum(this key key)
        {
            var r = KeyModeEnum.Major;
            if (key.ItemsElementName.Contains(ItemsChoiceType10.mode))
            {
                var value = key.ItemsElementName.ValueOf<string>(ItemsChoiceType10.mode, key.Items).ToLower();
                switch (value)
                {
                    case "major": r = KeyModeEnum.Major; break;
                    case "minor": r = KeyModeEnum.Minor; break;
                    case "dorian": r = KeyModeEnum.Dorian; break;
                    case "phrygian": r = KeyModeEnum.Phrygian; break;
                    case "lydian": r = KeyModeEnum.Lydian; break;
                    case "mixolydian": r = KeyModeEnum.Mixolydian; break;
                    case "aeolian": r = KeyModeEnum.Aeolian; break;
                    case "ionian": r = KeyModeEnum.Ionian; break;
                    case "locrian": r = KeyModeEnum.Locrian; break;
                    default: r = KeyModeEnum.Major; break;
                }
            }
            return r;
        }

        public static StemEnum ToNS(this stemvalue s) =>
            s switch
            {
                stemvalue.down => StemEnum.Down,
                stemvalue.up => StemEnum.Up,
                stemvalue.@double => StemEnum.Double,
                stemvalue.none => StemEnum.Undefined,
                _ => StemEnum.Undefined
            };

        public static BeamEnum ToNS(this beamvalue b) =>
            b switch
            {
                beamvalue.@continue => BeamEnum.Continue,
                beamvalue.backwardhook => BeamEnum.Undefined,
                beamvalue.begin => BeamEnum.Start,
                beamvalue.end => BeamEnum.End,
                beamvalue.forwardhook => BeamEnum.Undefined,
                _ => BeamEnum.Undefined
            };

        public static StepEnum ToNS(this step s) =>
            s switch {
                step.A => StepEnum.A,
                step.B => StepEnum.B,
                step.C => StepEnum.C,
                step.D => StepEnum.D,
                step.E => StepEnum.E,
                step.F => StepEnum.F,
                step.G => StepEnum.G,
                _ => StepEnum.Undefined
            };

        public static NoteHeadEnum ToNS(this noteheadvalue n) =>
            n switch
            {
                noteheadvalue.slash => NoteHeadEnum.Slash,
                noteheadvalue.triangle => NoteHeadEnum.Triangle,
                noteheadvalue.diamond => NoteHeadEnum.Diamond,
                noteheadvalue.square => NoteHeadEnum.Square,
                noteheadvalue.cross => NoteHeadEnum.Cross,
                noteheadvalue.x => NoteHeadEnum.X,
                noteheadvalue.circlex => NoteHeadEnum.CircleX,
                noteheadvalue.invertedtriangle => NoteHeadEnum.InvertedTriangle,
                noteheadvalue.arrowdown => NoteHeadEnum.ArrowDown,
                noteheadvalue.arrowup => NoteHeadEnum.ArrowUp,
                noteheadvalue.circled => NoteHeadEnum.Circled,
                noteheadvalue.slashed => NoteHeadEnum.Slashed,
                noteheadvalue.backslashed => NoteHeadEnum.BackSlashed,
                noteheadvalue.normal => NoteHeadEnum.Normal,
                noteheadvalue.cluster => NoteHeadEnum.Cluster,
                noteheadvalue.circledot => NoteHeadEnum.CircleDot,
                noteheadvalue.lefttriangle => NoteHeadEnum.LeftTriangle,
                noteheadvalue.rectangle => NoteHeadEnum.Rectangle,
                noteheadvalue.none => NoteHeadEnum.None,
                noteheadvalue.@do => NoteHeadEnum.Do,
                noteheadvalue.re => NoteHeadEnum.Re,
                noteheadvalue.mi => NoteHeadEnum.Mi,
                noteheadvalue.fa => NoteHeadEnum.Fa,
                noteheadvalue.faup => NoteHeadEnum.FaUp,
                noteheadvalue.so => NoteHeadEnum.So,
                noteheadvalue.la => NoteHeadEnum.La,
                noteheadvalue.ti => NoteHeadEnum.Ti,
                noteheadvalue.other => NoteHeadEnum.Other,
                _ => NoteHeadEnum.Undefined
            };

        public static int ToInt(this string s, int defaultValue = 0) => int.TryParse(s, out var r) ? r : defaultValue;
        public static float ToFloat(this string s, float defaultValue = 0f) => float.TryParse(s, out var r) ? r : defaultValue;
        public static double ToDouble(this string s, double defaultValue = 0.0) => double.TryParse(s, out var r) ? r : defaultValue;

        public static int RestIndex(this ClefEnum aClef) =>
            aClef switch
            {
                ClefEnum.Treble => 66,
                _ => 66
            };

        public static NoteHeadEnum ToNS(this notehead nh, NoteHeadEnum defaultValue = NoteHeadEnum.Undefined) => nh == null ? NoteHeadEnum.Normal : nh.Value.ToNS();
        
        public static ArticulationEnum ToNS(this ItemsChoiceType4 e) =>
            e switch
            {
                ItemsChoiceType4.accent => ArticulationEnum.Accent,
                ItemsChoiceType4.breathmark => ArticulationEnum.BreathMark,
                ItemsChoiceType4.caesura => ArticulationEnum.Caesura,
                ItemsChoiceType4.detachedlegato => ArticulationEnum.DetachedLegato,
                ItemsChoiceType4.doit => ArticulationEnum.Doit,
                ItemsChoiceType4.falloff => ArticulationEnum.FallOff,
                ItemsChoiceType4.otherarticulation => ArticulationEnum.Undefined,
                ItemsChoiceType4.plop => ArticulationEnum.Plop,
                ItemsChoiceType4.scoop => ArticulationEnum.Scoop,
                ItemsChoiceType4.softaccent => ArticulationEnum.Undefined, 
                ItemsChoiceType4.spiccato => ArticulationEnum.Spiccato,
                ItemsChoiceType4.staccatissimo => ArticulationEnum.Staccatissimo,
                ItemsChoiceType4.staccato => ArticulationEnum.Staccato,
                ItemsChoiceType4.stress => ArticulationEnum.Stress,
                ItemsChoiceType4.strongaccent => ArticulationEnum.StrongAccent,
                ItemsChoiceType4.tenuto => ArticulationEnum.Tenuto,
                ItemsChoiceType4.unstress => ArticulationEnum.Unstress,
                _ => ArticulationEnum.Undefined
            };
          
        public static OrientationEnum ToOrientationEnum(this PlacementEnum e) =>
            e switch
            {
                PlacementEnum.Above => OrientationEnum.Normal,
                PlacementEnum.Below => OrientationEnum.Inverted,
                PlacementEnum.Undefined => OrientationEnum.Undefined,
                _ => OrientationEnum.Undefined
            };
    }
}
