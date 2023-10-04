/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Chord
            {
                public static partial class Progressions
                {
                    public static ExerciseSettings.ChordSettings.ChordProgressionsSettings settings { get { return Exercises.settings.chordSettings.progressionsSettings; } }
                    public static ExerciseOptions.ChordOptions.ChordProgressionsOptions options { get { return Exercises.options.chordOptions.progressionsOptions; } }

                    public static void Generate()
                    {
                        CoreProgressions.Generate();
                    }

#pragma warning disable 414
                    static Action onFinishedCallback;
#pragma warning restore 414

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;
                    }

                    public static void Evaluate(Action<Enums.AnswerEnum> onAnswer)
                    {
                        onAnswer(data.answer);
                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (TL.Exercises.Chord.Progressions.settings.chordProgressionFlags.IsNone(TL.Exercises.Chord.Progressions.settings.chordProgressionFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.ProgressionsMissing;
                        }

                        if (TL.Exercises.Chord.Progressions.settings.toneFlags.IsNone(TL.Exercises.Chord.Progressions.settings.toneFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.TonesMissing;
                        }

                        return Enums.SettingsValidationErrorEnum.None;
                    }


                }
            }
        }
    }
}
