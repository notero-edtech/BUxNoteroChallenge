/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Interval
            {
                public static partial class Singing
                {
                    static partial class CoreSinging
                    {
                        public static void Generate()
                        {
                            var intervalList = Utilities.Intervals.MergeIntervalFlags(new List<Enums.Interval.IntervalFlags> {
                                Interval.settings.singingSettings.intervalFlags1st,
                                Interval.settings.singingSettings.intervalFlags2nd,
                            });

                            data.interval = Utilities.RandomItemFromList<int>(intervalList);

                            data.lowTone = Utilities.RandomEnum<Enums.ToneEnum>();
                            data.highTone = Utilities.Intervals.GetNextTone(data.lowTone, data.interval);

                            data.lowPitch = Exercises.settings.pitchRange.GetRandomPitchFromTone(data.lowTone);
                            data.highPitch = data.lowPitch + (int)data.interval;
                        }
                    }
                }
            }
        }
    }
}

