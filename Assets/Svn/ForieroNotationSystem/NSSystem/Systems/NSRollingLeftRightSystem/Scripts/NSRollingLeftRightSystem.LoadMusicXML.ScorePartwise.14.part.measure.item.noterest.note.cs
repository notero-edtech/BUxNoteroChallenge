/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingLeftRightSystem : NS
    {
        static partial class ScorePartwise
        {
            internal static partial class Part
            {
                internal static partial class Measure
                {
                    internal static partial class Item
                    {
                        internal static partial class NoteRest
                        {
                            internal static partial class Note
                            {
                                private static note spNote = null;
                                private static NSNote nsNote = null;

                                private static NSDurationBarHorizontal nsDurationBar = null;
                                private static NSObject nsMidiEventEnd = null;
                                private static float durationMultiplier = 1f;
                                private static TieTypeEnum tieType = TieTypeEnum.Undefined;
                                private static float time = 0;
                                private static float pixels = 0;

                                internal static void Reset()
                                {
                                    nsDurationBar = null;
                                    nsMidiEventEnd = null;
                                    tieType = TieTypeEnum.Undefined;
                                    time = 0;
                                    pixels = 0;
                                }
                                
                                internal static void Parse(note note)
                                {
                                    Reset();
                                    spNote = note;

                                    nsNote = nsStaveFixed.AddObject<NSNote>(PoolEnum.NS_MOVABLE);
                                    nsNote.options.noteEnum = spNote.ToNoteEnum();
                                    nsNote.options.chordNote = spNote.IsChord();
                                    nsNote.options.keySignatureEnum = nsStaveFixed.parsing.keySignature.keySignatureEnum;
                                    nsNote.options.keyModeEnum = nsStaveFixed.parsing.keySignature.keyModeEnum;
                                    nsNote.options.articulationEnum = spNote.ToArticulationEnum();
                                    nsNote.options.fermataEnum = spNote.ToFermataEnum();
                                    if(NSDisplaySettings.instance.fingering) nsNote.options.fingering = spNote.ToFingeringOptions();

                                    if(!spNote.IsChord()) durationMultiplier = spNote.ToArticulationEnum().ToDurationMultiplier();
                                    tieType = spNote.GetTieType();
                                    
                                    if (spNote.IsChord())
                                    {
                                        nsNote.options.stemEnum = StemEnum.Undefined;
                                        nsNote.options.autoStemEnum = false;
                                    } else if (spNote.stem != null) {
                                        nsNote.options.stemEnum = spNote.stem.Value.ToNS();
                                        nsNote.options.autoStemEnum = false;
                                        Assert.IsTrue(nsNote.options.stemEnum != StemEnum.Undefined);
                                    } else {
                                        nsNote.options.stemEnum = StemEnum.Undefined;
                                        nsNote.options.autoStemEnum = true;
                                    }

                                    nsNote.options.accidentalEnum = spNote.ToAccidentalEnum();
                                    nsNote.options.accidentalParenthesisEnum = spNote.ToAccidentalParenthesisEnum();
                                   
                                    if (NSDisplaySettings.Beams && spNote.beam != null)
                                    {
                                        nsNote.voiceNumber = voiceNumber;
                                        nsNote.options.beamEnum = spNote.beam[0].Value.ToNS();
                                    }

                                    if (spNote.HasDot()) { nsNote.options.dotsCount = spNote.GetDotCount(); }

                                    pixels = spNote.IsChord() ? fbPreviousPixels : fbPixels;
                                    time = spNote.IsChord() ? fbPreviousTime : fbTime;

                                    if (spNote.IsUnPitched()) UnPitched.Parse(spNote.GetUnPitched());
                                    else if(spNote.IsPitch()) Pitch.Parse(spNote.GetPitch());

                                    nsNote.Commit();
                                    nsNote.PixelShiftX(pixels * directionSign, true);
                                    if (tieType is TieTypeEnum.Continue or TieTypeEnum.Stop) nsNote.SetAlpha(0.3f);

                                    if (NSDisplaySettings.Ties)
                                    {
                                        switch (tieType)
                                        {
                                            case TieTypeEnum.Start:
                                                nsNote.tie = nsNote.AddObject<NSTie>(PoolEnum.NS_MOVABLE, PivotEnum.MiddleCenter, "TIE");
                                                nsNote.tie.options.orientationEnum = spNote.GetTieOrientation();
                                                nsNote.tie.Commit();
                                                voices[voiceNumber].tiedNotes.Add(nsNote);
                                                break;
                                            case TieTypeEnum.Stop:
                                            {
                                                var endNote = nsNote;
                                                for (var i = voices[voiceNumber].tiedNotes.Count - 1; i >= 0; i--)
                                                {
                                                    var startNote = voices[voiceNumber].tiedNotes[i];
                                                    if (startNote.IsSamePitch(nsNote))
                                                    {
                                                        voices[voiceNumber].tiedNotes.RemoveAt(i);
                                                        endNote.tie = startNote.tie;
                                                        startNote.tie.vector.tie.options.end = startNote.DistanceX(endNote);
                                                    }
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    nsStaveFixed.ApplyLedgerLines(nsNote);
                                    nsStaveFixed.ApplyLedgerLines(nsDurationBar);

                                    nsNote.Update();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
