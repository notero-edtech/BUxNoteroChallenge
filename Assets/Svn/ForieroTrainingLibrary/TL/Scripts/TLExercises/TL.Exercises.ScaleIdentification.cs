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
            public static partial class Scale
            {
                public static partial class Identification
                {
                    public static ExerciseSettings.ScaleSettings.ScaleIdentificationSettings settings { get { return Exercises.settings.scaleSettings.identificationSettings; } }
                    public static ExerciseOptions.ScaleOptions.ScaleIdentificationOptions options { get { return Exercises.options.scaleOptions.identificationOptions; } }

                    public static void Generate()
                    {
                        CoreIdentification.Generate();
                    }

                    private static Action onFinishedCallback;

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;
                    }

                    public static void Answer(List<Enums.ToneEnum> tones)
                    {
                        Enums.AnswerEnum result;

                        if (tones.Count != data.tones.Length)
                        {
                            result = Enums.AnswerEnum.Wrong;
                        }
                        else
                        {
                            result = Enums.AnswerEnum.Correct;
                            for (int i = 0; i < tones.Count; i++)
                            {
                                if (tones[i] != data.tones[i])
                                {
                                    result = Enums.AnswerEnum.Wrong;
                                    break;
                                }
                            }
                        }

                        data.answer = result;
                        if (onFinishedCallback != null)
                        {
                            onFinishedCallback();
                        }
                    }

                    public static void Answer(Enums.Scale.ScaleFlags scale)
                    {
                        data.answer = (scale == data.scale) ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong;
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
                        if (settings.keyFlags.IsNone(settings.keyFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.KeysMissing;
                        }

                        if (settings.playModeFlags.IsNone(settings.playModeFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.PlayModesMissing;
                        }

                        if (settings.scaleFlags.IsNone(settings.scaleFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.ScalesMissing;
                        }

                        return Enums.SettingsValidationErrorEnum.None;
                    }

                }

            }
        }
    }
}


