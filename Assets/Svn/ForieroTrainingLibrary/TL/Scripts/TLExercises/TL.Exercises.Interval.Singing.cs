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
                public static partial class Singing
                {
                    public static ExerciseSettings.IntervalSettings.IntervalSingingSettings settings { get { return Exercises.settings.intervalSettings.singingSettings; } }
                    public static ExerciseOptions.IntervalOptions.IntervalSingingOptions options { get { return Exercises.options.intervalOptions.singingOptions; } }

                    public static void Generate()
                    {
                        Interval.Singing.CoreSinging.Generate();
                    }

                    private static Action onFinishedCallback;

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;
                        TL.Inputs.OnPitch = OnPitch;

                        lastPitch = -1;

                        Providers.Metronome.Start();
                    }

                    private static int lastPitch;
                    private static double lastTime;

                    private static void OnPitch(int pitch, int offset)
                    {
                        UnityEngine.Debug.Log("note in: " + pitch);

                        if (lastPitch == -1 || (Providers.Metronome.totalTime - lastTime > 4 * TL.Utilities.Rhythms.BeatsToSeconds(Interval.settings.singingSettings.BPM)))
                        {
                            lastPitch = pitch;
                            lastTime = Providers.Metronome.totalTime;
                            return;
                        }

                        var interval = Utilities.Intervals.GetIntervalBetweenPitches(lastPitch, pitch);

                        data.answer = (interval == data.interval) ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong;

                        if (onFinishedCallback != null)
                        {
                            onFinishedCallback();
                        }

                        Providers.Metronome.Stop();
                    }

                    public static void Evaluate(Action<Enums.AnswerEnum> onAnswer)
                    {
                        onAnswer(data.answer);
                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (settings.intervalFlags1st.IsNone(settings.intervalFlags1st) && settings.intervalFlags2nd.IsNone(settings.intervalFlags2nd))
                        {
                            return Enums.SettingsValidationErrorEnum.IntervalsMissing;
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
