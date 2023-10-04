/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training.Core.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        static partial class Utilities
        {
            public static partial class Scales
            {
                public static List<Enums.Interval.IntervalFlags> GetScaleIntervals(Enums.Scale.ScaleFlags scale)
                {
                    var result = new List<Enums.Interval.IntervalFlags>();
                    var temp = (int)scale;
                    for (int i=0; i<=12; i++)
                    {
                        var flag = (1 << i);
                        if ((temp & flag) !=0)
                        {
                            var interval = (Enums.Interval.IntervalFlags)flag;
                            result.Add(interval);
                        }
                    }
                    return result;

                    //switch (scale)
                    //{
                        //case Enums.Scale.ScaleFlags.Major:
                        //    return GetIntervalsFromSteps("W-W-H-W-W-W-H");
                        //case Enums.Scale.ScaleFlags.PentatonicMajor:
                        //    return GetIntervalsFromSteps("W-W-A2-W-A2");
                        //case Enums.Scale.ScaleFlags.NaturalMinor:
                        //    return GetIntervalsFromSteps("W-H-W-W-H-W-W");
                        //case Enums.Scale.ScaleFlags.HarmonicMinor:
                        //    return GetIntervalsFromSteps("W-H-W-W-H-A2-H");
                        //case Enums.Scale.ScaleFlags.MelodicMinor:
                        //    return GetIntervalsFromSteps("W-H-W-W-W-W-H"); // ascending, descending is W-H-W-W-H-W-W
                        //case Enums.Scale.ScaleFlags.PentatonicMinor:
                        //    return GetIntervalsFromSteps("A2-W-W-A2-W");
                        //case Enums.Scale.ScaleFlags.Algerian:
                        //    return GetIntervalsFromSteps("W-H-A2-H-H-A2-H-W-H-W");
                        //case Enums.Scale.ScaleFlags.Arabian:
                        //    return GetIntervalsFromSteps("W-H-A2-H-H-A2-H-W-H-W");
                        //case Enums.Scale.ScaleFlags.Balinese:
                        //    return GetIntervalsFromSteps("H-W-A3-H-A3"); // The Balinese scale consisA2 of 5 different notes just like the pentatonic with the fourth and seventh scale steps omitted.
                        //case Enums.Scale.ScaleFlags.Byzantine:
                        //    return GetIntervalsFromSteps("H-A2-H-W-H-A2-H");
                        //case Enums.Scale.ScaleFlags.Chinese:
                        //    return GetIntervalsFromSteps("A3-W-H-A3-H"); // The Chinese scale consisA2 of 8 different notes. TODO - There is a harmony added to each scale step.
                        //case Enums.Scale.ScaleFlags.Egyptian:
                        //    return GetIntervalsFromSteps("W-A2-W-A2-W");
                        //case Enums.Scale.ScaleFlags.Ethiopian:
                        //    return GetIntervalsFromSteps("W-H-W-W-H-W-W"); // TODO: changes when descending
                        //case Enums.Scale.ScaleFlags.Hungarian:
                        //    return GetIntervalsFromSteps("A2-H-W-H-W-H-W");
                        //case Enums.Scale.ScaleFlags.Israeli:
                        //    return GetIntervalsFromSteps("H-A2-H-W-H-W-W");
                        //case Enums.Scale.ScaleFlags.Japanese:
                        //    return GetIntervalsFromSteps("H-A3-W-H-A3");
                        //case Enums.Scale.ScaleFlags.Javanese:
                        //    return GetIntervalsFromSteps("H-W-W-W-W-H-W");
                        //case Enums.Scale.ScaleFlags.Mongolian:
                        //    return GetIntervalsFromSteps("W-W-A2-W-A2");
                        //case Enums.Scale.ScaleFlags.Neapolitan:
                        //    return GetIntervalsFromSteps("H-W-W-W-W-W-H");
                        //case Enums.Scale.ScaleFlags.Persian:
                        //    return GetIntervalsFromSteps("H-A2-H-H-W-A2-H");
                        //case Enums.Scale.ScaleFlags.Spanish:
                        //return GetIntervalsFromSteps("H-A2-H-W-H-W-W");

                      //  default:
                        //    return null;
                    //}
                }

                private static List<Enums.Interval.IntervalFlags> GetIntervalsFromSteps(string stepList)
                {
                    string[] steps = stepList.Split('-');
                    int curOfs = 0;
                    var result = new List<Enums.Interval.IntervalFlags>();
                    for (int i = 0; i < steps.Length; i++)
                    {
                        int add;
                        var step = steps[i];

                        switch (step)
                        {
                            case "H":
                                add = 1;
                                break; // half step
                            case "W":
                                add = 2;
                                break; // whole step
                            case "A2":
                                add = 3;
                                break; // augmented 2
                            case "A3":
                                add = 4;
                                break; // augmented 3
                            case "A4":
                                add = 5;
                                break; // augmented 4
                            default:
                                Debug.LogError("Unknown scale step :" + step);
                                return null;
                        }

                        curOfs = (add % 12);
                        result.Add((Enums.Interval.IntervalFlags)(1 << curOfs));
                    }

                    return result;
                }

                public static Enums.ToneEnum[] GetScaleTones(Enums.ToneEnum root, Enums.Scale.ScaleFlags scale)
                {
                    var intervals = GetScaleIntervals(scale);
                    var result = new Enums.ToneEnum[intervals.Count];

                    var rootVal = (int)root;

                    int toneIndex = 0;
                    foreach (var interval in intervals)
                    {
                        var intervalVal = interval.ToBitNumber();
                        var curVal = (rootVal + intervalVal) % 12;

                        result[toneIndex] = ((Enums.ToneEnum)curVal);
                        toneIndex++;
                        }

                    return result;
                }


                public static Enums.ToneEnum[] GetChordTonesFromDegree(Enums.Scale.ScaleDegreeEnum degree, Enums.ToneEnum[] tones)
                {
                    var result = new Enums.ToneEnum[3];
                    switch (degree)
                    {
                        case Enums.Scale.ScaleDegreeEnum.First:
                            {
                                result[0] = tones[0];
                                result[1] = tones[2];
                                result[2] = tones[4];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.Second:
                            {
                                result[0] = tones[1];
                                result[1] = tones[3];
                                result[2] = tones[5];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.FlattenedSecond:
                            {
                                result[0] = Theory.GetPreviousTone(tones[1]);
                                result[1] = tones[3];
                                result[2] = tones[5];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.Third:
                            {
                                result[0] = tones[2];
                                result[1] = tones[4];
                                result[2] = tones[6];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.FlattenedThird:
                            {
                                result[0] = Theory.GetPreviousTone(tones[2]);
                                result[1] = tones[4];
                                result[2] = tones[6];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.Fourth:
                            {
                                result[0] = tones[3];
                                result[1] = tones[5];
                                result[2] = tones[0];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.Fifth:
                            {
                                result[0] = tones[4];
                                result[1] = tones[6];
                                result[2] = tones[1];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.FlattenedFifth:
                            {
                                result[0] = Theory.GetPreviousTone(tones[4]);
                                result[1] = tones[6];
                                result[2] = tones[1];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.Sixth:
                            {
                                result[0] = tones[5];
                                result[1] = tones[0];
                                result[2] = tones[2];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.FlattenedSixth:
                            {
                                result[0] = Theory.GetPreviousTone(tones[5]);
                                result[1] = tones[0];
                                result[2] = tones[2];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.Seventh:
                            {
                                result[0] = tones[6];
                                result[1] = tones[1];
                                result[2] = tones[3];
                                break;
                            }

                        case Enums.Scale.ScaleDegreeEnum.FlattenedSeventh:
                            {
                                result[0] = Theory.GetPreviousTone(tones[6]);
                                result[1] = tones[1];
                                result[2] = tones[3];
                                break;
                            }

                        default: return null;
                    }

                    return result;
                }

            }
        }
    }
}
