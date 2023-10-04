/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Core.Extensions;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Melody
            {
                public static partial class Dictation
                {
                    public static ExerciseSettings.MelodySettings.MelodyDictationSettings settings { get { return Exercises.settings.melodySettings.dictationSettings; } }
                    public static ExerciseOptions.MelodyOptions.MelodyDictationOptions options { get { return Exercises.options.melodyOptions.dictationOptions; } }

                    public static void Generate()
                    {

                    }

                    public static void Execute()
                    {

                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (settings.toneFlags.IsNone(settings.toneFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.TonesMissing;
                        }

                        if (settings.keyFlags.IsNone(settings.keyFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.KeysMissing;
                        }

                        return Enums.SettingsValidationErrorEnum.None;
                    }

                }
            }
        }
    }
}
