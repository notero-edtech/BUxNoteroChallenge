/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.Detection;
using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Chord
            {
                public static partial class Singing
                {
                    public static ExerciseSettings.ChordSettings.ChordSingingSettings settings { get { return Exercises.settings.chordSettings.singingSettings; } }
                    public static ExerciseOptions.ChordOptions.ChordSingingOptions options { get { return Exercises.options.chordOptions.singingOptions; } }

                    public static void Generate()
                    {
                        CoreSinging.Generate();
                    }

                    private static Action onFinishedCallback;

                    public static void Execute(Action onFinished)
                    {
                        onFinishedCallback = onFinished;

                        TL.Inputs.OnMidiOn = OnMidiOn;
                        TL.Inputs.OnMidiOff = OnMidiOff;
                        TL.Inputs.OnUpdate = OnUpdate;

                        TL.Providers.Metronome.Start();
                    }

                    private static TL.Enums.AnswerEnum CheckAnswer(HashSet<int> chord)
                    {
                        var rootTone = data.tones[0];

                        var temp = chord.ToList();
                        temp.Sort((x, y) => x.CompareTo(y));

                        var lowestPitch = temp[0];
                        var inputRoot = Utilities.Midi.FromIndex(lowestPitch);
                        if (inputRoot != rootTone)
                        {
                            return Enums.AnswerEnum.Wrong;
                        }

                        for (int i = 1; i < temp.Count; i++)
                        {
                            var A = Utilities.Midi.FromIndex(temp[i - 1]);
                            var B = Utilities.Midi.FromIndex(temp[i]);

                            var correctInterval = Utilities.Intervals.GetIntervalBetweenTones(data.tones[i - 1], data.tones[i]);
                            var inputInterval = Utilities.Intervals.GetIntervalBetweenTones(A, B);

                            if (inputInterval != correctInterval)
                            {
                                return Enums.AnswerEnum.Wrong;
                            }
                        }

                        return Enums.AnswerEnum.Correct;
                    }

                    private static void OnMidiOn(int note)
                    {
                        ChordDetection.InputPress(note);
                    }

                    private static void OnMidiOff(int note)
                    {
                        ChordDetection.InputRelease(note);
                    }

                    private static void OnUpdate()
                    {
                        var chord = ChordDetection.InputUpdate(Chord.settings.singingSettings.BPM);
                        if (chord.Count == data.tones.Length)
                        {
                            data.answer = CheckAnswer(chord);

                            TL.Providers.Metronome.Stop();

                            if (onFinishedCallback != null)
                            {
                                onFinishedCallback();
                            }

                            TL.Inputs.OnUpdate = null;
                        }
                    }

                    public static void Evaluate(Action<Enums.AnswerEnum> onAnswer)
                    {
                        onAnswer(data.answer);
                    }

                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (TL.Exercises.Chord.Singing.settings.chordTypeFlags.IsNone(TL.Exercises.Chord.Singing.settings.chordTypeFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.ProgressionsMissing;
                        }

                        if (TL.Exercises.Chord.Singing.settings.chordTypeFlags.IsNone(TL.Exercises.Chord.Progressions.settings.toneFlags))
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
