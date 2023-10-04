/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using Unity.VisualScripting;
using UnityEngine;
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
                                internal static partial class UnPitched
                                {
                                    internal static void Reset()
                                    {
                                        
                                    }

                                    internal static void Parse(unpitched unpitched)
                                    {
                                        Reset();
                                        var spUnPitched = unpitched;
                                        
                                        var stepEnum = spUnPitched.displaystep.ToNS();
                                        var octave = spUnPitched.displayoctave.ToInt(4);
                                        nsNote.options.stepEnum = stepEnum;
                                        nsNote.options.octave = octave;
                                        nsNote.options.noteHeadEnum = spNote.notehead.ToNS();
                                        
                                        var y = nsNote.GetPositionOnStave(stepEnum.ToInt(), octave, nsStaveFixed.parsing.clef);
                                        
                                        nsNote.SetPositionY(y, true, true);
                                        
                                        // if (tieType is TieTypeEnum.Start or TieTypeEnum.Undefined)
                                        // {
                                        //     nsDurationBar = nsStaveFixed.AddObject<NSDurationBarHorizontal>(PoolEnum.NS_MOVABLE);
                                        //     nsDurationBar.passable = true;
                                        //     nsDurationBar.options.lenght = durationInPixels * directionSign * durationMultiplier;
                                        //     nsDurationBar.SetPositionY(y, true, true);
                                        //     //nsDurationBar.SetColor(spPitch.ToMidiIndex().ToMidiColor());
                                        //
                                        //     nsDurationBar.midiData.Add(new MidiMessage()
                                        //     {
                                        //         channel = staveNumber,
                                        //         command = CommandEnum.MIDI_NOTE_ON.ToInt(),
                                        //         data1 = spPitch.ToMidiIndex(),
                                        //         data2 = NSPlaybackSettings.instance.defaultNoteAttack,
                                        //         time = time,
                                        //         duration = duration * durationMultiplier
                                        //     });
                                        //     nsDurationBar.midiData.duration = duration * durationMultiplier;
                                        //     nsDurationBar.midiData.time = time;
                                        //     nsDurationBar.midiData.noteOn = true;
                                        //
                                        //     midiOnBuffer[spPitch.ToMidiIndex()] = nsDurationBar;
                                        //
                                        //     nsDurationBar.Commit();
                                        //     nsDurationBar.SendVisuallyBack();
                                        //     nsDurationBar.PixelShiftX(pixels * directionSign, true);
                                        //     nsDurationBar.pixelTime = pixels;                                            
                                        // }
                                        //
                                        // if (tieType is TieTypeEnum.Continue)
                                        // {
                                        //     var d = midiOnBuffer[spPitch.ToMidiIndex()] as NSDurationBarHorizontal;
                                        //     d.options.lenght += durationInPixels * directionSign;
                                        //     d.midiData.duration += duration;
                                        //     d.Commit();
                                        // }
                                        //
                                        // if (tieType is TieTypeEnum.Stop or TieTypeEnum.Undefined)
                                        // {
                                        //  //   midiOnBuffer[spPitch.ToMidiIndex()] = null;
                                        //
                                        //     nsMidiEventEnd = nsStaveFixed.AddObject<NSObject>(PoolEnum.NS_MOVABLE);
                                        //     nsMidiEventEnd.SetPositionY(y, true, true);
                                        //     nsMidiEventEnd.midiData.Add(new MidiMessage()
                                        //     {
                                        //         channel = staveNumber,
                                        //         command = CommandEnum.MIDI_NOTE_OFF.ToInt(),
                                        //      //   data1 = spPitch.ToMidiIndex(),
                                        //         data2 = 0,
                                        //         time = time + duration * durationMultiplier
                                        //     });
                                        //     nsMidiEventEnd.midiData.duration = duration * durationMultiplier;
                                        //     nsMidiEventEnd.midiData.time = tieType == TieTypeEnum.Undefined
                                        //         ? time + duration * durationMultiplier
                                        //         : time;
                                        //     nsMidiEventEnd.midiData.noteOn = false;
                                        //
                                        //     foreach (var m in nsMidiEventEnd.midiData.messages)
                                        //     {
                                        //         //Debug.Log("END : " + m.time);
                                        //     }
                                        //
                                        //     nsMidiEventEnd.Commit();
                                        //     nsMidiEventEnd.PixelShiftX((pixels + durationInPixels - shortenDurationBarsForPixels) * directionSign, true);
                                        //     nsMidiEventEnd.pixelTime = pixels + durationInPixels - shortenDurationBarsForPixels;
                                        // }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
