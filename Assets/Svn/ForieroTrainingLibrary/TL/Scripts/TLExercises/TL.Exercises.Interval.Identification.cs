/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
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
                public static partial class Identification
                {
                    public static ExerciseSettings.IntervalSettings.IntervalIdentificationSettings settings { get { return Exercises.settings.intervalSettings.identificationSettings; } }
                    public static ExerciseOptions.IntervalOptions.IntervalIdentificationOptions options { get { return Exercises.options.intervalOptions.identificationOptions; } }

                    public static void Generate()
                    {
                        Interval.Identification.CoreIdentification.Generate();
                    }

                    private static Action onFinishedCallback;

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;
                        TL.Inputs.OnMidiOn = OnMidiOn;
                    }

                    private static void OnMidiOn(int note)
                    {
                        UnityEngine.Debug.Log("note in: " + note);
                    }

                    public static void Answer(int answer)
                    {
                        data.answer = (answer == data.interval ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong);

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
