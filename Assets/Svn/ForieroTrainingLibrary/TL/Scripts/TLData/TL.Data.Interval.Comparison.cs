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
                    public enum QuestionAnswerEnum
                    {
                        Larger,
                        Smaller,
                        Equal
                    }

                    public class Data : Interval.Data
                    {
                        public Enums.ToneEnum lowToneA;
                        public Enums.ToneEnum highToneA;
                        public Enums.ToneEnum lowToneB;
                        public Enums.ToneEnum highToneB;
                        public int intervalA;
                        public int intervalB;
                        public QuestionAnswerEnum correctAnswer;

                        public int lowPitchA;
                        public int highPitchA;

                        public int lowPitchB;
                        public int highPitchB;
                    }

                    public static Data data = new Data();
                }
            }
        }
    }


}
