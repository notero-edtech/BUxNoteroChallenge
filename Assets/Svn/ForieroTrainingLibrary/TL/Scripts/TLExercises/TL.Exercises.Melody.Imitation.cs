/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Detection;
using ForieroEngine.Music.Training.Core.Extensions;
using System;
using System.Collections;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Exercises
        {
            public static partial class Melody
            {
                public static partial class Imitation
                {
                    public static ExerciseSettings.MelodySettings.MelodyImitationSettings settings { get { return Exercises.settings.melodySettings.imitationSettings; } }
                    public static ExerciseOptions.MelodyOptions.MelodyImitationOptions options { get { return Exercises.options.melodyOptions.imitationOptions; } }

                    public static void Generate()
                    {
                        CoreImitation.Generate();
                    }

                    private static Action onFinishedCallback;

                    private static PitchEvaluator pitchEvaluator;

                    public static void Execute(Action onFinished)
                    {
                        Inputs.OnMidiOn = OnMelodyMidiOn;
                        Inputs.OnPitch = OnMelodyPitch;

                        Inputs.OnUpdate = OnMelodyUpdate;

                        TL.Providers.Metronome.bpm = Melody.settings.imitationSettings.BPM;
                        TL.Providers.Metronome.Start();

                        pitchEvaluator = new PitchEvaluator();

                        onFinishedCallback = onFinished;
                    }


                    private static void OnMelodyMidiOn(int pitch)
                    {
                        AddPitch(pitch);
                    }

                    private static void OnMelodyPitch(int pitch, int offsetInCents)
                    {
                        pitchEvaluator.Update(pitch);
                    }

                    private static void AddPitch(int pitch)
                    {
                        data.midiDataAnswer.Add(Classes.MidiStruct.Note(pitch, Enums.NoteAndRestFlags.Quarter));
                    }

                    private static void OnMelodyUpdate()
                    {
                        var curTime = Providers.Metronome.totalTime;

                        if (pitchEvaluator.detectedPitches.Count > 0)
                        {
                            AddPitch(pitchEvaluator.detectedPitches[0]);
                        }

                        var beatDuration = TL.Utilities.Rhythms.BeatsToSeconds(Melody.settings.imitationSettings.BPM);

                        if (curTime - data.lastTime >= beatDuration)
                        {
                            bool compareResult = TL.Utilities.CompareMidiLists(data.measures[0].notes, data.midiDataAnswer);
                            data.answer = compareResult ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong;

                            if (onFinishedCallback != null)
                            {
                                onFinishedCallback();
                            }

                            TL.Providers.Metronome.Stop();
                            TL.Inputs.OnUpdate = null;
                        }

                    }

                    public static void Evaluate(Action<Enums.AnswerEnum> onAnswer)
                    {
                        onAnswer(data.answer);
                    }


                    public static Enums.SettingsValidationErrorEnum CheckSettings()
                    {
                        if (settings.toneFlags.IsNone(settings.toneFlags))
                        {
                            return Enums.SettingsValidationErrorEnum.TonesMissing;
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
