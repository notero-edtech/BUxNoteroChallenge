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
                public static partial class Imitation
                {
                    public static ExerciseSettings.RhythmSettings.RhythmImitationSettings settings { get { return Exercises.settings.rhythmSettings.imitationSettings; } }
                    public static ExerciseOptions.RhythmOptions.RhythmImitationOptions options { get { return Exercises.options.rhythmOptions.imitationOptions; } }

                    public static void Generate()
                    {
                        CoreImmitation.Generate();
                    }

                    private static Action onFinishedCallback;
                    private static bool clapMode;

                    public static void Execute(Action onFinished)
                    {
                        Inputs.OnTapDown = OnImitationTapDown;
                        Inputs.OnTapUp = OnImitationTapUp;
                        Inputs.OnClap = OnImitationClap;
                        Inputs.OnUpdate = OnImitationUpdate;

                        Inputs.OnMidiOn = OnImitationMidiOn;
                        Inputs.OnMidiOff = OnImitationMidiOff;

                        TL.Providers.Metronome.bpm = Rhythm.settings.imitationSettings.BPM;
                        TL.Providers.Metronome.Start();

                        data.isRecordingInput = true;
                        data.questionEvents = Utilities.Rhythms.AnalyseRhythmEvents(data.rhythm, Rhythm.settings.imitationSettings.BPM);

                        clapMode = false;

                        onFinishedCallback = onFinished;
                    }

                    public static double GetCurrentTime()
                    {
                        return TL.Providers.Metronome.totalTime;
                    }


                    private static void BeginCapture(Data.RhythmCaptureModeEnum mode)
                    {
                        if (!data.isRecordingInput)
                        {
                            return;
                        }

                        if (data.rhythmCaptureMode != Data.RhythmCaptureModeEnum.None)
                        {
                            return;
                        }


                        data.rhythmCaptureMode = mode;

                        double lastTime;

                        if (data.rhythmInput.Count > 0)
                        {
                            lastTime = data.rhythmInput[data.rhythmInput.Count - 1].endTime;
                        }
                        else
                        {
                            lastTime = 0;
                        }

                        var delta = GetCurrentTime() - lastTime;
                        var minDuration = TL.Utilities.Rhythms.BeatsToSeconds(Rhythm.settings.imitationSettings.BPM) / 2;
                        if (delta > minDuration) // insert a rest
                        {
                            var evt = new RhythmEvent();
                            evt.startTime = lastTime;
                            evt.endTime = GetCurrentTime();
                            evt.isRest = true;
                            data.rhythmInput.Add(evt);
                        }

                        {
                            var evt = new RhythmEvent();
                            evt.startTime = GetCurrentTime();
                            evt.endTime = evt.startTime;
                            evt.isRest = false;

                            data.rhythmInput.Add(evt);
                            TL.Providers.Midi.Rhythm();
                        }
                    }

                    private static void EndCapture(Data.RhythmCaptureModeEnum mode)
                    {
                        if (!data.isRecordingInput)
                        {
                            return;
                        }

                        if (data.rhythmCaptureMode != mode)
                        {
                            return;
                        }

                        data.rhythmCaptureMode = Data.RhythmCaptureModeEnum.None;
                    }

                    private static void OnImitationMidiOn(int note)
                    {
                        BeginCapture(Data.RhythmCaptureModeEnum.Midi);
                    }

                    private static void OnImitationMidiOff(int obj)
                    {
                        EndCapture(Data.RhythmCaptureModeEnum.Midi);
                    }

                    private static void OnImitationTapDown()
                    {
                        TL.Inputs.OnClap = null; // if they tap we need to unhook microphone clapping detection since by tapping I will play clapping sound and it could clashes
                        BeginCapture(Data.RhythmCaptureModeEnum.Tap);

                    }

                    private static void OnImitationTapUp()
                    {
                        EndCapture(Data.RhythmCaptureModeEnum.Tap);
                    }

                    // clap does not have "off" event, so we say a clap has a duration of one beat
                    private static void OnImitationClap()
                    {
                        var curTime = GetCurrentTime();

                        var lastIndex = data.rhythmInput.Count - 1;
                        if (lastIndex >= 0 && curTime < data.rhythmInput[lastIndex].endTime)
                        {
                            return; // sometimes clapping algorhtm finds multiple claps at once, this prevents them being added as input
                        }

                        var evt = new RhythmEvent();
                        evt.startTime = curTime;
                        evt.endTime = evt.startTime + TL.Utilities.Rhythms.BeatsToSeconds(Rhythm.settings.imitationSettings.BPM) / 4;
                        evt.isRest = false;

                        clapMode = true;

                        data.rhythmInput.Add(evt);

                        TL.Providers.Midi.Rhythm();
                    }

                    private static void OnImitationUpdate()
                    {
                        var curTime = GetCurrentTime();

                        var lastTime = data.questionEvents[data.questionEvents.Count - 1].endTime;

                        if (data.rhythmCaptureMode != Data.RhythmCaptureModeEnum.None)
                        {
                            var last = data.rhythmInput.Count - 1;
                            var evt = data.rhythmInput[last];
                            evt.endTime = GetCurrentTime();
                            data.rhythmInput[last] = evt;
                        }

                        if (curTime - lastTime >= 1.0f)
                        {
                            bool compareResult;

                            float errorPercent;

                            switch (Rhythm.settings.evaluation)
                            {
                                case Enums.EvaluationEnum.Kind: errorPercent = 0.3f; break;
                                case Enums.EvaluationEnum.Normal: errorPercent = 0.25f; break;
                                //case Enums.EvaluationEnum.Strict: 
                                default: errorPercent = 0.15f; break;
                            }

                            if (clapMode)
                            {
                                compareResult = Utilities.Rhythms.CompareClapping(data.questionEvents, data.rhythmInput, errorPercent);
                            }
                            else
                            {
                                compareResult = Utilities.Rhythms.CompareRhythms(data.questionEvents, data.rhythmInput, errorPercent);
                            }

                            data.answer = compareResult ? Enums.AnswerEnum.Correct : Enums.AnswerEnum.Wrong;

                            if (onFinishedCallback != null)
                            {
                                onFinishedCallback();
                            }

                            Providers.Metronome.Stop();
                            Inputs.OnUpdate = null;
                        }

                    }

#pragma warning disable 414
                    static Action onRhythmFinished;
#pragma warning restore 414

                    public static void PlayRhythm(Action onFinished)
                    {
                        onRhythmFinished = onFinished;

                        double time = 0;
                        bool insideTie = false;

                        foreach (var measure in data.measures)
                        {
                            foreach (var item in measure.notes)
                            {
                                if (!item.isRest && !insideTie)
                                {
                                    TL.Providers.Midi.Rhythm(time);
                                }

                                double multiplier = 1;

                                if (item.tighEnum == Classes.MidiStruct.TighEnum.Begin)
                                {
                                    insideTie = true;
                                }
                                else
                                if (item.tighEnum == Classes.MidiStruct.TighEnum.End)
                                {
                                    insideTie = false;
                                }

                                var curDuration = item.duration;

                                if (item.tupletEnum != Classes.MidiStruct.TupletEnum.Undefined)
                                {
                                    multiplier = 1.0 / (double)item.tupletValue;
                                    curDuration = TL.Utilities.Rhythms.GetDoubleDurationOf(curDuration);
                                }

                                time += TL.Utilities.Rhythms.NoteDurationToSeconds(curDuration, TL.Exercises.Rhythm.settings.imitationSettings.BPM) * multiplier;
                            }
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
