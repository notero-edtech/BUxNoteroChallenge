/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Chord
            {
                public static ExerciseSettings.ChordSettings settings { get { return Exercises.settings.chordSettings; } }
                public static ExerciseOptions.ChordOptions options { get { return Exercises.options.chordOptions; } }
            }
        }
    }
}

