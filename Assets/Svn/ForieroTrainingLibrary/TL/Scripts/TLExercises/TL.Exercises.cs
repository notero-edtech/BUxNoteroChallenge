/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            static ExerciseSettings _settings = null;

            public static ExerciseSettings settings
            {
                get => _settings ??= new ();
                set => _settings = value;
            }

            static ExerciseOptions _options = null;

            public static ExerciseOptions options
            {
                get => _options ??= new ();
                set => _options = value;
            }
        }
    }
}

