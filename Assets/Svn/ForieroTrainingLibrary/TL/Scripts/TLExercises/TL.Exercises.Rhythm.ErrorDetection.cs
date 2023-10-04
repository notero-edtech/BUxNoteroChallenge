/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Core.Extensions;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Rhythm
            {
                public static partial class ErrorDetection
                {
                    public static ExerciseSettings.RhythmSettings.RhythmErrorDetectionSettings settings { get { return Exercises.settings.rhythmSettings.errorDetectionSettings; } }
                    public static ExerciseOptions.RhythmOptions.RhythmErrorDetectionOptions options { get { return Exercises.options.rhythmOptions.errorDetectionOptions; } }

                    public static void Generate()
                    {

                    }

                    public static void Execute()
                    {

                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (settings.noteFlags.IsNone(settings.noteFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.NotesMissing;
                        }

                        if (settings.restFlags.IsNone(settings.restFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.NotesMissing;
                        }

                        if (settings.beatsPerMeasure.IsNone(settings.beatsPerMeasure) || settings.notesPerBeat.IsNone(settings.notesPerBeat))
                        {
                            return Enums.SettingsValidationErrorEnum.TimeSignaturesMissing;
                        }

                        return Enums.SettingsValidationErrorEnum.None;
                    }

                }
            }
        }
    }
}
