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
            public static partial class Chord
            {
                public static partial class Inversions
                {
                    public static ExerciseSettings.ChordSettings.ChordInversionsSettings settings { get { return Exercises.settings.chordSettings.inversionsSettings; } }
                    public static ExerciseOptions.ChordOptions.ChordInversionsOptions options { get { return Exercises.options.chordOptions.inversionsOptions; } }

                    public static void Generate()
                    {
                        CoreInversions.Generate();
                    }

                    private static Action onFinishedCallback;

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;
                    }


                    public static void Evaluate(Action<Enums.AnswerEnum> onAnswer)
                    {
                        onAnswer(data.answer);
                    }

                    public static void Answer(Enums.Chord.ChordInversionTypeFlags inversion)
                    {
                        data.answer = inversion == data.inversion ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong;

                        if (onFinishedCallback != null)
                        {
                            onFinishedCallback();
                        }
                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (TL.Exercises.Chord.Inversions.settings.chordTypeFlags.IsNone(TL.Exercises.Chord.Inversions.settings.chordTypeFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.ChordsMissing;
                        }

                        if (TL.Exercises.Chord.Inversions.settings.toneFlags.IsNone(TL.Exercises.Chord.Inversions.settings.toneFlags))
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
