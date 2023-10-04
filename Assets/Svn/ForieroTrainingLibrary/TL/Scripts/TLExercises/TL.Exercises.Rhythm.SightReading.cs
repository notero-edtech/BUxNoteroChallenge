/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections;
using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Rhythm
            {
                public static partial class SightReading
                {
                    public static ExerciseSettings.RhythmSettings.RhythmSightReadingSettings settings { get { return Exercises.settings.rhythmSettings.sightReadingSettings; } }
                    public static ExerciseOptions.RhythmOptions.RhythmSightReadingOptions options { get { return Exercises.options.rhythmOptions.sightReadingOptions; } }

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
