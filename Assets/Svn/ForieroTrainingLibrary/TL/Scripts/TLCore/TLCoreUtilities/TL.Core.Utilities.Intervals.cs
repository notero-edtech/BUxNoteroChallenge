/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training.Classes;
using UnityEngine;
using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        static partial class Utilities
        {
            public static partial class Intervals
            {
                public static int GetIntervalBetweenPitches(int low, int high)
                {
                    int diff = high - low;
                    return diff;
                }

                public static Enums.Interval.IntervalFlags GetIntervalBetweenTones(Enums.ToneEnum low, Enums.ToneEnum high)
                {
                    int lowVal = (int)low;
                    int highVal = (int)high;

                    if (highVal < lowVal)
                    {
                        highVal += 12; // jump to next octave
                    }

                    int diff = highVal - lowVal;
                    return (Enums.Interval.IntervalFlags)(1 << diff);
                }

                public static List<int> MergeIntervalFlags(List<Enums.Interval.IntervalFlags> intervals)
                {
                    var result = new List<int>();
                    for (int i=0; i<intervals.Count; i++)
                    {
                        var temp = (int)intervals[i];
                        for (int j=0; j<=12; j++)
                        {
                            var flag = 1 << j;
                            if ((temp & flag) != 0)
                            {
                                result.Add(j + i * 12);
                            }
                        }
                    }
                    return result;
                }

                public static Enums.ToneEnum GetNextTone(Enums.ToneEnum low, Enums.Interval.IntervalFlags interval)
                {
                    int lowVal = (int)low;
                    int diff = interval.ToBitNumber();
                    int highVal = (lowVal + diff) % 12;
                    return (Enums.ToneEnum)highVal;
                }

                public static Enums.ToneEnum GetNextTone(Enums.ToneEnum low, int intervalInSemitones)
                {
                    int lowVal = (int)low;
                    int highVal = (lowVal + intervalInSemitones) % 12;
                    return (Enums.ToneEnum)highVal;
                }

                public static Enums.ToneEnum GetPreviousTone(Enums.ToneEnum high, Enums.Interval.IntervalFlags interval)
                {
                    int highVal = (int)high;
                    int diff = interval.ToBitNumber();
                    int lowVal = (highVal - diff);
                    if (lowVal < 0)
                    {
                        lowVal += 12;
                    }

                    return (Enums.ToneEnum)lowVal;
                }
            }


        }
    }
}
