/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSPageLayoutTickerSystem : NS
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
                                internal static note spNote = null;
                                internal static NSNote nsNote = null;

                                internal static NSDurationBarHorizontal nsDurationBar = null;
                                internal static NSObject nsMidiEventEnd = null;

                                internal static void Parse(note note)
                                {
                                    spNote = note;

                                    nsNote = nsStaveFixed.AddObject<NSNote>( PoolEnum.NS_MOVABLE);
                                    nsNote.options.noteEnum = spNote.ToNoteEnum();
                                    nsNote.options.chordNote = spNote.IsChord();
                                    nsNote.options.keySignatureEnum = nsStaveFixed.parsing.keySignature.keySignatureEnum;
                                    nsNote.options.keyModeEnum = nsStaveFixed.parsing.keySignature.keyModeEnum;
                                    nsNote.options.articulationEnum = spNote.ToArticulationEnum();
                                    nsNote.options.fermataEnum = spNote.ToFermataEnum();

                                    if (spNote.IsChord())
                                    {
                                        nsNote.options.stemEnum = StemEnum.Undefined;
                                        nsNote.options.autoStemEnum = false;
                                    }
                                    else if (spNote.stem != null)
                                    {
                                        nsNote.options.stemEnum = spNote.stem.Value.ToNS();
                                        Assert.IsTrue(nsNote.options.stemEnum != StemEnum.Undefined);
                                    }
                                    else
                                    {
                                        nsNote.options.stemEnum = StemEnum.Undefined;
                                        nsNote.options.autoStemEnum = true;
                                    }

                                    nsNote.options.accidentalEnum = spNote.ToAccidentalEnum();
                                    nsNote.options.accidentalParenthesisEnum = spNote.ToAccidentalParenthesisEnum();

                                    nsDurationBar = nsStaveFixed.AddObject<NSDurationBarHorizontal>( PoolEnum.NS_MOVABLE);
                                    nsDurationBar.passable = true;
                                    nsDurationBar.options.lenght = timePixels * directionSign;

                                    if (NSDisplaySettings.Beams && spNote.beam != null)
                                    {
                                        nsNote.voiceNumber = voiceNumber;
                                        nsNote.options.beamEnum = spNote.beam[0].Value.ToNS();
                                    }

                                    if (spNote.HasDot())
                                    {
                                        nsNote.options.dotsCount = spNote.GetDotCount();
                                    }

                                    var tieType = spNote.GetTieType();

                                    if (tieType == TieTypeEnum.Stop || tieType == TieTypeEnum.Undefined) nsMidiEventEnd = nsPart.AddObject<NSObject>( PoolEnum.NS_MOVABLE); else nsMidiEventEnd = null;

                                    pitch spPitch = spNote.ItemsElementName.ValueOf<pitch>(ItemsChoiceType1.pitch, spNote.Items);

                                    if (spPitch == null) { }
                                    else
                                    {
                                        var stepEnum = spPitch.step.ToNS();
                                        nsNote.options.stepEnum = stepEnum;
                                        nsNote.options.alter = spPitch.GetAlter();

                                        float y = nsNote.GetPositionOnStave((int)stepEnum, int.Parse(spPitch.octave), nsStaveFixed.parsing.clef);

                                        nsNote.SetPositionY(y, true, true);
                                        nsDurationBar.SetPositionY(y, true, true);
                                        nsDurationBar.SetColor(spPitch.ToMidiIndex().ToMidiColor());

                                        if (nsMidiEventEnd) nsMidiEventEnd.SetPositionY(y, true, true);

                                        if (tieType == TieTypeEnum.Start || tieType == TieTypeEnum.Undefined)
                                        {
                                            nsDurationBar.midiData.Add(new MidiMessage()
                                            {
                                                Channel = staveNumber,
                                                Command = CommandEnum.MIDI_NOTE_ON.ToInt(),
                                                Data1 = spPitch.ToMidiIndex(),
                                                Data2 = 100,
                                                //duration = duration,
                                                //time = fbTime
                                            });
                                            nsDurationBar.midiData.duration = timeInMeasure;
                                            nsDurationBar.midiData.noteOn = true;
                                        }

                                        if (nsMidiEventEnd)
                                        {
                                            nsMidiEventEnd.midiData.Add(new MidiMessage()
                                            {
                                                Channel = staveNumber,
                                                Command = CommandEnum.MIDI_NOTE_OFF.ToInt(),
                                                Data1 = spPitch.ToMidiIndex(),
                                                Data2 = 0,
                                                //timeInMeasure = timeInMeasure,
                                                //time = fbTime
                                            });
                                            nsMidiEventEnd.midiData.duration = timeInMeasure;
                                            nsMidiEventEnd.midiData.noteOn = false;
                                        }
                                    }

                                    nsNote.Commit();

                                    if (tieType == TieTypeEnum.Continue || tieType == TieTypeEnum.Stop) nsNote.SetAlpha(0.3f);

                                    nsDurationBar.Commit();
                                    nsDurationBar.SendVisuallyBack();

                                    if (spNote.IsChord())
                                    {
                                        nsNote.PixelShiftX(fbPreviousPixels * directionSign, true);
                                        nsDurationBar.PixelShiftX(fbPreviousPixels * directionSign, true);
                                        nsDurationBar.pixelTime = fbPreviousPixels;
                                        if (nsMidiEventEnd) nsMidiEventEnd.PixelShiftX((fbPreviousPixels + timePixels - shortenDurationBarsForPixels) * directionSign, true);
                                        if (nsMidiEventEnd) nsMidiEventEnd.pixelTime = fbPreviousPixels + timePixels - shortenDurationBarsForPixels;
                                    }
                                    else
                                    {
                                        nsNote.PixelShiftX(fbPixels * directionSign, true);
                                        nsDurationBar.PixelShiftX(fbPixels * directionSign, true);
                                        nsDurationBar.pixelTime = fbPixels;
                                        if (nsMidiEventEnd) nsMidiEventEnd.PixelShiftX((fbPixels + timePixels - shortenDurationBarsForPixels) * directionSign, true);
                                        if (nsMidiEventEnd) nsMidiEventEnd.pixelTime = fbPixels + timePixels - shortenDurationBarsForPixels;
                                    }

                                    if (NSDisplaySettings.Ties)
                                    {
                                        switch (tieType)
                                        {
                                            case TieTypeEnum.Start:
                                                nsNote.tie = nsNote.AddObject<NSTie>(PoolEnum.NS_MOVABLE,
                                                    PivotEnum.MiddleCenter, "TIE");
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
                                                        startNote.tie.vector.tie.options.end =
                                                            startNote.DistanceX(endNote);
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
