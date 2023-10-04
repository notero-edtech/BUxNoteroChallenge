/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.Training.Core.Classes.Rhythms;
using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Rhythm
            {
                public static partial class Dictation
                {
                    public static ExerciseSettings.RhythmSettings.RhythmDictationSettings settings { get { return Exercises.settings.rhythmSettings.dictationSettings; } }
                    public static ExerciseOptions.RhythmOptions.RhythmDictationOptions options { get { return Exercises.options.rhythmOptions.dictationOptions; } }

                    public static void Generate()
                    {

                        CoreDictation.Generate();
                    }

                    private static Action onFinishedCallback;
                    private static bool clapMode;

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;
                    }

                    private static bool CompareInput()
                    {
                        if (data.rhythmInput.Count != data.rhythm.Count)
                        {
                            return false;
                        }

                        return true;
                    }

                    public static void VerifyInput()
                    {
                        var compareResult = CompareInput();
                        data.answer = compareResult ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong;

                        if (onFinishedCallback != null)
                        {
                            onFinishedCallback();
                        }

                        TL.Providers.Metronome.Stop();
                        TL.Inputs.OnUpdate = null;

                    }

                    public static void AddInput(Enums.NoteAndRestFlags duration, bool isRest)
                    {
                        var evt = new RhythmItem();
                        evt.duration = duration;
                        evt.isRest = isRest;

                        data.rhythmInput.Add(evt);
                    }

                    public static void RemoveInput()
                    {
                        if (data.rhythmInput.Count > 0)
                        {
                            data.rhythmInput.RemoveAt(data.rhythmInput.Count - 1);
                        }

                    }

#pragma warning disable 414
                    static Action onRhythmFinished;
#pragma warning restore 414

                    public static void PlayRhythm(Action onFinished)
                    {
                        onRhythmFinished = onFinished;

                        double time = 0;
                        foreach (var item in data.rhythm)
                        {
                            if (!item.isRest)
                            {
                                UnityEngine.Debug.Log("ry : " + time);
                                TL.Providers.Midi.Rhythm(time);
                            }

                            time += TL.Utilities.Rhythms.NoteDurationToSeconds(item.duration, TL.Exercises.Rhythm.settings.imitationSettings.BPM);
                        }

                        TL.Providers.Metronome.ScheduleEvent(time, onFinished);
                    }

                    public static void Evaluate(Action<Enums.AnswerEnum> onAnswer)
                    {
                        onAnswer(data.answer);
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
