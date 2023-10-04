/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Rhythm
            {
                public static ExerciseSettings.RhythmSettings settings { get { return Exercises.settings.rhythmSettings; } }
                public static ExerciseOptions.RhythmOptions options { get { return Exercises.options.rhythmOptions; } }
            }
        }
    }
}

