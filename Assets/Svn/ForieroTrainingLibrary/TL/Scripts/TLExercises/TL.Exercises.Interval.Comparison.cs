/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections.Generic;
using System;
using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Interval
            {
                public static partial class Comparison
                {
                    public static ExerciseSettings.IntervalSettings.IntervalComparisonSettings settings { get { return Exercises.settings.intervalSettings.comparisonSettings; } }
                    public static ExerciseOptions.IntervalOptions.IntervalComparisonOptions options { get { return Exercises.options.intervalOptions.comparisonOptions; } }

                    public static void Generate()
                    {

                        CoreComparison.Generate();

                    }

                    private static Action onFinishedCallback;

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;
                    }

                    public static void Answer(QuestionAnswerEnum answer)
                    {
                        data.answer = (answer == data.correctAnswer) ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong;

                        if (onFinishedCallback != null)
                        {
                            onFinishedCallback();
                        }
                    }

                    public static void Evaluate(Action<Enums.AnswerEnum> onAnswer)
                    {
                        onAnswer(data.answer);
                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (settings.intervalFlags1st.IsNone(settings.intervalFlags1st) && settings.intervalFlags2nd.IsNone(settings.intervalFlags2nd) && settings.intervalFlags3rd.IsNone(settings.intervalFlags3rd))
                        {
                            return Enums.SettingsValidationErrorEnum.IntervalsMissing;
                        }

                        if (settings.playModeFlags.IsNone(settings.playModeFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.PlayModesMissing;
                        }

                        return Enums.SettingsValidationErrorEnum.None;
                    }

                }
            }
        }
    }


}
