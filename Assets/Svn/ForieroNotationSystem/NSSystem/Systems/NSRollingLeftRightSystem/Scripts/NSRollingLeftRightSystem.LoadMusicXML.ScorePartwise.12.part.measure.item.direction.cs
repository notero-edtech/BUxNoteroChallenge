/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

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
                        internal static partial class Direction
                        {
                            internal static direction spDirection = null;
                            internal static metronome spMetronome = null;
                            internal static sound spSound = null;
                            internal static pedal spPedal = null;
                            internal static octaveshift spOctaveShift = null;
                            internal static formattedtextid spWords = null;

                            static PlacementEnum placementEnum = PlacementEnum.Undefined;

                            internal static void Parse(direction direction)
                            {
                                Reset();

                                spDirection = direction;

                                if (spDirection == null) return;

                                placementEnum = spDirection.GetPlacement();

                                spMetronome = spDirection.GetMetronome();
                                ParseMetronome(spMetronome);

                                spSound = spDirection.sound;
                                ParseSound(spSound);

                                spPedal = spDirection.GetPedal();
                                ParsePedal(spPedal);

                            }

                            internal static void Reset()
                            {
                                spDirection = null;
                                spMetronome = null;
                                spSound = null;
                                spPedal = null;
                                spOctaveShift = null;
                                spWords = null;
                            }

                            static void ParseMetronome(metronome metronome)
                            {
                                if (metronome == null) return;
                                tempo = spMetronome.GetTempoPerQuarterNote(tempo);

                                var placement = placementEnum == PlacementEnum.Undefined ? PlacementEnum.Above : placementEnum;

                                spMetronome.SetNSMetronomeMarkOptions(metronomeMarkOptions);

                                foreach (var part in ns.parsing.parts)
                                {
                                    if (part.Value.parsing.staves.Count == 0) break;

                                    if (!nsPart.parsing.metronomeMark.Same(metronomeMarkOptions))
                                    {
                                        var nsStave = part.Value.parsing.staves.First();

                                        var metronomeMark = nsStave.AddObject<NSMetronomeMark>( PoolEnum.NS_MOVABLE);
                                        metronomeMark.options.CopyValuesFrom(metronomeMarkOptions);
                                        metronomeMark.Commit();

                                        metronomeMark.pixelTime = fbPixels;

                                        metronomeMark.SetPositionX(fbPixels, true, true);
                                        metronomeMark.SetPositionY(nsStave.topEdge + ns.LineSize * 3, true, true);
                                        if (measureIndexer == 0)
                                        {
                                            metronomeMark.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha);
                                            metronomeMark.hidden = true;
                                        }

                                        metronomeMark.passable = true;

                                        nsPart.parsing.metronomeMark.CopyValuesFrom(metronomeMarkOptions);
                                    }
                                }
                            }

                            static void ParseSound(sound sound)
                            {
                                if (sound == null) return;

                                tempo = spSound.GetTempoPerQuarterNote(tempo);

                                var placement = placementEnum == PlacementEnum.Undefined ? PlacementEnum.Above : placementEnum;

                                metronomeMarkOptions.beatsPerMinute = tempo;

                                foreach (var part in ns.parsing.parts)
                                {
                                    if (part.Value.parsing.staves.Count == 0) break;

                                    if (!nsPart.parsing.metronomeMark.Same(metronomeMarkOptions))
                                    {
                                        var nsStave = part.Value.parsing.staves.First();

                                        var metronomeMark = nsStave.AddObject<NSMetronomeMark>( PoolEnum.NS_MOVABLE);
                                        metronomeMark.options.CopyValuesFrom(metronomeMarkOptions);
                                        metronomeMark.Commit();

                                        metronomeMark.pixelTime = fbPixels;

                                        metronomeMark.SetPositionX(fbPixels, true, true);
                                        metronomeMark.SetPositionY(nsStave.topEdge + ns.LineSize * 3, true, true);
                                        if (measureIndexer == 0)
                                        {
                                            metronomeMark.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha);
                                            metronomeMark.hidden = true;
                                        }

                                        metronomeMark.passable = true;

                                        nsPart.parsing.metronomeMark.CopyValuesFrom(metronomeMarkOptions);
                                    }
                                }
                            }

                            static void ParsePedal(pedal pedal)
                            {
                                if (pedal == null) return;

                                var placement = placementEnum == PlacementEnum.Undefined ? PlacementEnum.Below : placementEnum;

                                //int dir = i == 0 ? 1 : -1;
                                //

                                NSPedal nsPedal = null;
                                NSStave nsStave = null;

                                switch (placement)
                                {
                                    case PlacementEnum.Above:
                                        nsStave = nsPart.parsing.staves.First();
                                        nsPedal = nsStave.AddObject<NSPedal>( PoolEnum.NS_MOVABLE);
                                        nsPedal.PixelShiftY(ns.LineSize * keySignaturesDistance, true);
                                        break;
                                    case PlacementEnum.Below:
                                        nsStave = nsPart.parsing.staves.Last();
                                        nsPedal = nsStave.AddObject<NSPedal>( PoolEnum.NS_MOVABLE);
                                        nsPedal.PixelShiftY(-ns.LineSize * keySignaturesDistance, true);
                                        break;
                                }

                                nsPedal.PixelShiftX(Part.fbPixels, true);
                                nsPedal.options.pedalEnum = pedal.type.ToNS();
                                nsPedal.Commit();

                                nsPedal.ApplyMidiMessage(0);
                            }

                            static void ParseOctaveShift(octaveshift octaveshift)
                            {
                                if (octaveshift == null) return;
                            }

                            static void ParseOctaveShift(formattedtextid words)
                            {
                                if (words == null) return;
                            }
                        }
                    }
                }
            }
        }
    }
}
