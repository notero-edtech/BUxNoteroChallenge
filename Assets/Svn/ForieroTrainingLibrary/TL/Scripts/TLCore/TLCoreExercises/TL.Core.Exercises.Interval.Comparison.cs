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
                public static partial class Comparison
                {
                    static partial class CoreComparison
                    {
                        public static void Generate()
                        {
                            var intervalList = Utilities.Intervals.MergeIntervalFlags(new List<Enums.Interval.IntervalFlags> {
                                Interval.settings.comparisonSettings.intervalFlags1st,
                                Interval.settings.comparisonSettings.intervalFlags2nd,
                                Interval.settings.comparisonSettings.intervalFlags3rd
                            });

                            data.lowToneA = Utilities.RandomEnum<Enums.ToneEnum>();
                            data.intervalA = Utilities.RandomItemFromList<int>(intervalList);
                            data.highToneA = Utilities.Intervals.GetNextTone(data.lowToneA, data.intervalA);

                            data.lowToneB = Utilities.RandomEnum<Enums.ToneEnum>();
                            data.intervalB = Utilities.RandomItemFromList<int>(intervalList);
                            data.highToneB = Utilities.Intervals.GetNextTone(data.lowToneB, data.intervalB);

                            if (data.intervalA > data.intervalB)
                            {
                                data.correctAnswer = QuestionAnswerEnum.Larger;
                            }
                            else
                            if (data.intervalA < data.intervalB)
                            {
                                data.correctAnswer = QuestionAnswerEnum.Smaller;
                            }
                            else
                            {
                                data.correctAnswer = QuestionAnswerEnum.Equal;
                            }

                            data.lowPitchA = Exercises.settings.pitchRange.GetRandomPitchFromTone(data.lowToneA);
                            data.highPitchA = data.lowPitchA + (int)data.intervalA;

                            data.lowPitchB = Exercises.settings.pitchRange.GetRandomPitchFromTone(data.lowToneB);
                            data.highPitchB = data.lowPitchB + (int)data.intervalA;
                        }
                    }
                }
            }
        }
    }
}

