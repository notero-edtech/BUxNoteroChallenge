/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
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

                                internal static NSDurationBarVertical nsDurationBar = null;
                                internal static NSObject nsMidiEventEnd = null;
                                internal static float durationMultiplier = 1f;

                                internal static void Reset()
                                {
                                    nsDurationBar = null;
                                    nsMidiEventEnd = null;
                                }
                                
                                internal static void Parse(note note)
                                {
                                    Reset();
                                    spNote = note;

                                    nsNote = nsStaveFixed.AddObject<NSNote>(PoolEnum.NS_MOVABLE);
                                    nsNote.options.noteEnum = spNote.ToNoteEnum();
                                    nsNote.options.chordNote = false;
                                    nsNote.options.autoStemEnum = false;
                                    nsNote.options.keySignatureEnum = nsStaveFixed.parsing.keySignature.keySignatureEnum;
                                    nsNote.options.keyModeEnum = nsStaveFixed.parsing.keySignature.keyModeEnum;
                                    
                                    switch (NSPlayback.NSRollingPlayback.rollingMode)
                                    {
                                        case NSPlayback.NSRollingPlayback.RollingMode.Top: nsNote.options.stemEnum = StemEnum.Down; break;
                                        case NSPlayback.NSRollingPlayback.RollingMode.Bottom: nsNote.options.stemEnum = StemEnum.Up; break;
                                    }

                                    nsNote.options.accidentalEnum = spNote.ToAccidentalEnum();
                                    nsNote.options.accidentalParenthesisEnum = spNote.ToAccidentalParenthesisEnum();
                                    
                                    nsNote.voiceNumber = voiceNumber;

                                    if (spNote.HasDot()) { nsNote.options.dotsCount = spNote.GetDotCount(); }

                                    if(!spNote.IsChord()) durationMultiplier = spNote.ToArticulationEnum().ToDurationMultiplier();
                                    var tieType = spNote.GetTieType();
                                   
                                    var scale = 1f;

                                    var time = spNote.IsChord() ? fbPreviousTime : fbTime;
                                    var pixels = spNote.IsChord() ? fbPreviousPixels : fbPixels;
                                    
                                    if(spNote.IsPitch())
                                    {
                                        var spPitch = spNote.GetPitch();
                                        var stepEnum = spPitch.step.ToNS();
                                        nsNote.options.stepEnum = stepEnum;
                                        nsNote.options.alter = spPitch.GetAlter();

                                        var midiIndex = spPitch.ToMidiIndex();
                                        var width = ns.GetKeyWidth(midiIndex);

                                        scale = width / nsNote.size.x;

                                        var x = ns.GetKeyPosition(midiIndex);
                                        nsNote.SetPositionX(x, true, true);
                                       
                                        if (tieType is TieTypeEnum.Start or TieTypeEnum.Undefined)
                                        {
                                            nsDurationBar = nsStaveFixed.AddObject<NSDurationBarVertical>(PoolEnum.NS_MOVABLE);
                                            nsDurationBar.passable = true;
                                            nsDurationBar.options.lenght = durationInPixels * directionSign * durationMultiplier;
                                            nsDurationBar.options.thickness = width;
                                            nsDurationBar.SetPositionX(x, true, true);
                                            nsDurationBar.SetColor(midiIndex.ToMidiColor());
                                            
                                            nsDurationBar.midiData.Add(new MidiMessage()
                                            {
                                                Channel = staveNumber,
                                                Command = CommandEnum.MIDI_NOTE_ON.ToInt(),
                                                Data1 = spPitch.ToMidiIndex(),
                                                Data2 = NSPlaybackSettings.instance.defaultNoteAttack,
                                                Time = time,
                                                Duration = duration * durationMultiplier,
                                            });
                                            nsDurationBar.midiData.duration = duration * durationMultiplier;
                                            nsDurationBar.midiData.time = time;
                                            nsDurationBar.midiData.noteOn = true;
                                            
                                            nsDurationBar.Commit();
                                            nsDurationBar.SendVisuallyBack();
                                            nsDurationBar.PixelShiftY(pixels * directionSign, true);
                                            nsDurationBar.pixelTime = pixels;
                                            
                                            midiOnBuffer[spPitch.ToMidiIndex()] = nsDurationBar;
                                        }

                                        if (tieType is TieTypeEnum.Continue)
                                        {
                                            var d = midiOnBuffer[spPitch.ToMidiIndex()] as NSDurationBarVertical;
                                            d.options.lenght += durationInPixels * directionSign;
                                            d.midiData.duration += duration;
                                            d.Commit();
                                        }

                                        if (tieType is TieTypeEnum.Stop or TieTypeEnum.Undefined)
                                        {
                                            midiOnBuffer[spPitch.ToMidiIndex()] = null;
                                            
                                            nsMidiEventEnd = nsStaveFixed.AddObject<NSObject>( PoolEnum.NS_MOVABLE);
                                            nsMidiEventEnd.SetPositionX(x, true, true);
                                            nsMidiEventEnd.midiData.Add(new MidiMessage()
                                            {
                                                Channel = staveNumber,
                                                Command = CommandEnum.MIDI_NOTE_OFF.ToInt(),
                                                Data1 = spPitch.ToMidiIndex(),
                                                Data2 = 0,
                                                Time = time + duration * durationMultiplier
                                            });
                                            nsMidiEventEnd.midiData.duration = duration * durationMultiplier;
                                            nsMidiEventEnd.midiData.time = tieType == TieTypeEnum.Undefined ? time + duration * durationMultiplier : time;
                                            nsMidiEventEnd.midiData.noteOn = false;
                                            
                                            foreach (var m in nsMidiEventEnd.midiData.messages)
                                            {
                                                //Debug.Log("END : " + m.time);
                                            }
                                            
                                            nsMidiEventEnd.Commit();
                                            nsMidiEventEnd.PixelShiftY((pixels + durationInPixels - shortenDurationBarsForPixels) * directionSign, true);
                                            nsMidiEventEnd.pixelTime = pixels + durationInPixels - shortenDurationBarsForPixels;
                                        }
                                    }

                                    nsNote.Commit();

                                    if (tieType is TieTypeEnum.Continue or TieTypeEnum.Stop) nsNote.SetAlpha(0.3f);

                                    if (nsNote.accidental)
                                    {
                                        nsNote.accidental.DestroyChildren();
                                        nsNote.accidental.Destroy();
                                    }

                                    nsNote.SetScale(scale, true);
                                    nsNote.PixelShiftY(pixels * directionSign, true);
                                    
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
