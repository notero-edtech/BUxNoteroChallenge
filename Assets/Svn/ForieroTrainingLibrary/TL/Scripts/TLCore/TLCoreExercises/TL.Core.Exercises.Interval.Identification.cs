/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{

    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Interval
            {
                public static partial class Identification
                {
                    static partial class CoreIdentification
                    {

                        public static void Generate()
                        {
                            var intervalList = Utilities.Intervals.MergeIntervalFlags(new List<Enums.Interval.IntervalFlags> {
                                Interval.settings.comparisonSettings.intervalFlags1st,
                                Interval.settings.comparisonSettings.intervalFlags2nd,
                                Interval.settings.comparisonSettings.intervalFlags3rd
                            });

                            data.interval = Utilities.RandomItemFromList<int>(intervalList);

                            data.lowTone = Utilities.RandomEnum<Enums.ToneEnum>();
                            data.highTone = Utilities.Intervals.GetNextTone(data.lowTone, data.interval);

                            data.lowPitch = Exercises.settings.pitchRange.GetRandomPitchFromTone(data.lowTone);
                            data.highPitch = data.lowPitch + (int)data.interval;
                        }



                        /*public static void PlayInterval(Action onFinish)
                        {
                                                       Exercises.settings.intervalSettings.playMode.PlayInterval(_lowVal, _highVal, onFinish);
                        }*/
                    }
                }
            }
        }
    }
}

