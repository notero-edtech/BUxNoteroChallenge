/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

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
                        internal static partial class Direction
                        {
                            internal static direction spDirection = null;
                            internal static metronome spMetronome = null;
                            internal static sound spSound = null;
                            internal static pedal spPedal = null;
                            internal static octaveshift spOctaveShift = null;
                            internal static formattedtextid spWords = null;

                            internal static NSMetronomeMark.Options options = new NSMetronomeMark.Options();

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

                                Debug.Log("METRONOME TEMPO : " + tempo);

                                var placement = placementEnum == PlacementEnum.Undefined ? PlacementEnum.Above : placementEnum;


                            }

                            static void ParseSound(sound sound)
                            {
                                if (sound == null) return;

                                tempo = spSound.GetTempoPerQuarterNote(tempo);

                                Debug.Log("SOUND TEMPO : " + tempo);

                                var placement = placementEnum == PlacementEnum.Undefined ? PlacementEnum.Above : placementEnum;

                            }

                            static void ParsePedal(pedal pedal)
                            {
                                if (pedal == null) return;

                                var placement = placementEnum == PlacementEnum.Undefined ? PlacementEnum.Below : placementEnum;

                                //int dir = i == 0 ? 1 : -1;
                                //

                                //NSPedal nsPedal = null;
                                //NSStave nsStave = null;

                                //switch (placement)
                                //{
                                //    case PlacementEnum.Above:
                                //        nsStave = Part.staves.First();
                                //        nsPedal = nsStave.AddObject<NSPedal>( PoolEnum.NS_MOVABLE);
                                //        nsPedal.PixelShiftY(ns.lineSize * keySignaturesDistance, true);
                                //        break;
                                //    case PlacementEnum.Below:
                                //        nsStave = Part.staves.Last();
                                //        nsPedal = nsStave.AddObject<NSPedal>( PoolEnum.NS_MOVABLE);
                                //        nsPedal.PixelShiftY(-ns.lineSize * keySignaturesDistance, true);
                                //        break;
                                //}

                                //nsPedal.PixelShiftY(Part.fbPixels, true);
                                //nsPedal.options.pedalEnum = pedal.type.ToNS();
                                //nsPedal.Commit();

                                //nsPedal.ApplyMidiMessage(0);
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
