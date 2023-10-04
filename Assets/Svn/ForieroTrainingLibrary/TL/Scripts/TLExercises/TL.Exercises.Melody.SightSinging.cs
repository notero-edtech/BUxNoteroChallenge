/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Core.Extensions;
using System;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Melody
            {
                public static partial class SightSinging
                {
                    public static ExerciseSettings.MelodySettings.MelodySightSingingSettings settings { get { return Exercises.settings.melodySettings.sightSingingSettings; } }
                    public static ExerciseOptions.MelodyOptions.MelodySightSingingOptions options { get { return Exercises.options.melodyOptions.sightSingingOptions; } }

                    public static void Generate()
                    {

                    }

                    public static void Execute()
                    {
                        TL.Inputs.OnPitch = OnPitch;
                    }

                    private static void OnPitch(int pitch, int offsetInCents)
                    {
                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (settings.noteFlags.IsNone(settings.noteFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.NotesMissing;
                        }

                        return Enums.SettingsValidationErrorEnum.None;
                    }


                }
            }
        }
    }
}
