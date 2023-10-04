/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Scale
            {
                public static ExerciseSettings.ScaleSettings settings { get { return Exercises.settings.scaleSettings; } }
                public static ExerciseOptions.ScaleOptions options { get { return Exercises.options.scaleOptions; } }
            }
        }
    }
}

