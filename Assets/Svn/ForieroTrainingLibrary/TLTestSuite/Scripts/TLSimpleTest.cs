/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.Music.Training;
using ForieroEngine.Music.Training.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TLSimpleTest : MonoBehaviour
{
    public enum ExerciseEnum
    {
        IntervalComparison,
        IntervalIdentification,
        ChordIdentification,
        ChordInversions,
        ChordProgressions,
        ScaleIdentification,
        RhythmDictation,
        RhythmErrorDetection,
        RhythmImitation,
        RhythmSightReading,
        MelodyDictation,
        MelodyImitation,
        MelodySightSinging,
        Undefined = int.MaxValue
    }


    private ExerciseEnum currentExercise = ExerciseEnum.Undefined;
    private Action guiCallback;

    private TL.Enums.AnswerEnum currentAnswer = TL.Enums.AnswerEnum.Unanswered;
    private bool isUIBlocked = false;

    public TLSettingsScriptableObject settings;

    private void BlockUI()
    {
        isUIBlocked = true;
        TL.Inputs.block = true;
        TL.Inputs.blockUpdate = true;
    }

    private void UnblockUI()
    {
        isUIBlocked = false;
        TL.Inputs.block = false;
        TL.Inputs.blockUpdate = false;
    }

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitUntil(() =>
           MIDI.initialized
        );

        TL.Detection.detectionFlags = TL.Enums.Microphone.DetectionFlags.Clap;

        var tones = TL.Utilities.Chords.GetTonesFromChord(TL.Enums.Chord.ChordTypeFlags.Minor, TL.Enums.ToneEnum.A, TL.Enums.Chord.ChordInversionTypeFlags.Root);
        foreach (var tone in tones)
        {
            Debug.Log("tone " + tone);
        }

        Init();
    }

    private void Init()
    {
        TL.Exercises.settings = settings.exerciseSettings;

        var intervalSettings = TL.Exercises.settings.intervalSettings;
        intervalSettings.identificationSettings.intervalFlags1st.Add(TL.Enums.Interval.IntervalFlags.Major2nd);
        intervalSettings.identificationSettings.intervalFlags1st.Add(TL.Enums.Interval.IntervalFlags.Major3rd);
        intervalSettings.identificationSettings.intervalFlags1st.Add(TL.Enums.Interval.IntervalFlags.Perfect5th);

        var imitationSettings = TL.Exercises.Rhythm.Imitation.settings;
        imitationSettings.noteFlags = 0;
        //imitationSettings.noteFlags = imitationSettings.noteFlags.Add(TL.Enums.NoteAndRestFlags.Half);
        imitationSettings.noteFlags = imitationSettings.noteFlags.Add(TL.Enums.NoteAndRestFlags.Quarter);
        //imitationSettings.noteFlags = imitationSettings.noteFlags.Add(TL.Enums.NoteAndRestFlags.Item8th);

        imitationSettings.tupletNoteFlags = 0;
        imitationSettings.tupletNoteFlags = imitationSettings.tupletNoteFlags.Add(TL.Enums.TupletNoteAndRestFlags.Item8thTuplet3);

        var compSettings = TL.Exercises.Interval.settings.comparisonSettings;
        compSettings.intervalFlags1st = TL.Enums.Interval.IntervalFlags.Perfect5th;

        //var chordSettings = TL.Exercises.settings.chordSettings;
        /*chordSettings.chordsToInclude.Add(TL.Enums.Chord.ChordTypeFlags.Major);
        chordSettings.chordsToInclude.Add(TL.Enums.Chord.ChordTypeFlags.Minor);
        chordSettings.inversionsToInclude.Add(TL.Enums.Chord.ChordInversionTypeFlags.Root);
        chordSettings.inversionsToInclude.Add(TL.Enums.Chord.ChordInversionTypeFlags.First);
        chordSettings.inversionsToInclude.Add(TL.Enums.Chord.ChordInversionTypeFlags.Second);*/

        var tones = TL.Utilities.Scales.GetScaleTones(TL.Enums.ToneEnum.C, TL.Enums.Scale.ScaleFlags.Major);
        string s = "scale: ";
        foreach (var tone in tones)
        {
            s += tone + " ";
        }
        Debug.Log(s);
    }

    private void onEvaluate(TL.Enums.AnswerEnum answer)
    {
        currentAnswer = answer;
    }

    private void onFinished()
    {
        switch (currentExercise)
        {
            case ExerciseEnum.IntervalIdentification: TL.Exercises.Interval.Identification.Evaluate(onEvaluate); break;
            case ExerciseEnum.IntervalComparison: TL.Exercises.Interval.Comparison.Evaluate(onEvaluate); break;
            case ExerciseEnum.ChordIdentification: TL.Exercises.Chord.Identification.Evaluate(onEvaluate); break;
            case ExerciseEnum.ChordInversions: TL.Exercises.Chord.Inversions.Evaluate(onEvaluate); break;
            case ExerciseEnum.ChordProgressions: TL.Exercises.Chord.Progressions.Evaluate(onEvaluate); break;
            case ExerciseEnum.RhythmImitation: TL.Exercises.Rhythm.Imitation.Evaluate(onEvaluate); break;
            case ExerciseEnum.ScaleIdentification: TL.Exercises.Scale.Identification.Evaluate(onEvaluate); break;
            default:
                {
                    Debug.LogError("onFinished not supported yet for " + currentExercise);
                    break;
                }

        }

    }

    private void Update()
    {
        /*MidiMessage msg;
        if (MidiINPlugin.PopMessage(out msg)!=0)
        {
            Debug.Log("Got midi input");
        }*/
    }

    void OnGUI()
    {
        if (isUIBlocked)
        {
            GUI.Label(new Rect(300, 10, 300, 40), "Wait...");
            return;
        }

        if (currentExercise != ExerciseEnum.Undefined)
        {
            if (currentAnswer != TL.Enums.AnswerEnum.Unanswered)
            {
                GUI.Label(new Rect(questionX, questionY + 10, 150, 60), currentAnswer.ToString());

                if (GUI.Button(new Rect(questionX, questionY + 100, 150, 60), "Back"))
                {
                    currentAnswer = TL.Enums.AnswerEnum.Unanswered;
                    currentExercise = ExerciseEnum.Undefined;
                }
            }
            else
            {
                guiCallback();
            }


        }
        else
        {
            if (GUI.Button(new Rect(10, 10, 150, 50), "Interval Comparison"))
            {
                currentExercise = ExerciseEnum.IntervalComparison;
                TL.Exercises.Interval.Comparison.Generate();
                TL.Exercises.Interval.Comparison.Execute(onFinished);
                guiCallback = onIntervalComparison;
            }

            if (GUI.Button(new Rect(10, 110, 150, 50), "Interval Identification"))
            {
                currentExercise = ExerciseEnum.IntervalIdentification;
                TL.Exercises.Interval.Identification.Generate();
                TL.Exercises.Interval.Identification.Execute(onFinished);
                guiCallback = onIntervalIdentification;
            }

            if (GUI.Button(new Rect(10, 210, 150, 50), "Chord Identification"))
            {
                currentExercise = ExerciseEnum.ChordIdentification;
                TL.Exercises.Chord.Identification.Generate();
                TL.Exercises.Chord.Identification.Execute(onFinished);
                guiCallback = onChordIdentification;
            }

            if (GUI.Button(new Rect(10, 310, 150, 50), "Chord Inversions"))
            {
                currentExercise = ExerciseEnum.ChordInversions;
                TL.Exercises.Chord.Inversions.Generate();
                TL.Exercises.Chord.Inversions.Execute(onFinished);
                guiCallback = onChordInversions;
            }

            if (GUI.Button(new Rect(10, 410, 150, 50), "Chord Progressions"))
            {
                currentExercise = ExerciseEnum.ChordProgressions;
                TL.Exercises.Chord.Progressions.Generate();
                TL.Exercises.Chord.Progressions.Execute(onFinished);
                guiCallback = onChordProgressions;
            }

            if (GUI.Button(new Rect(10, 510, 150, 50), "Scale Identification"))
            {
                currentExercise = ExerciseEnum.ScaleIdentification;
                TL.Exercises.Scale.Identification.Generate();
                TL.Exercises.Scale.Identification.Execute(onFinished);
                guiCallback = onScaleIdentification;
            }

            if (GUI.Button(new Rect(10, 610, 150, 50), "Rhythm Imitation"))
            {
                currentExercise = ExerciseEnum.RhythmImitation;
                TL.Exercises.Rhythm.Imitation.Generate();
                guiCallback = onRhythms;
            }

        }
    }

    private const int questionX = 350;
    private const int questionY = 100;

    void onIntervalComparison()
    {

        if (GUI.Button(new Rect(100, 100, 150, 60), "Play Interval A"))
        {
            BlockUI();
            var data = TL.Exercises.Interval.Comparison.data;
            TL.Providers.Midi.PlayInterval(TL.Exercises.Interval.settings.comparisonSettings.playModeFlags, data.lowPitchA, data.highPitchA, TL.Exercises.Interval.settings.toneDuration, TL.Exercises.Interval.settings.toneGap, () =>
            {
                UnblockUI();
            });
        }

        if (GUI.Button(new Rect(100, 200, 150, 60), "Play Interval B"))
        {
            BlockUI();
            var data = TL.Exercises.Interval.Comparison.data;
            TL.Providers.Midi.PlayInterval(TL.Exercises.Interval.settings.comparisonSettings.playModeFlags, data.lowPitchB, data.highPitchB, TL.Exercises.Interval.settings.toneDuration, TL.Exercises.Interval.settings.toneGap, () =>
            {
                UnblockUI();
            });
        }

        GUI.Label(new Rect(questionX, questionY, 300, 60), "Is interval A smaller, larger or equal to interval B?");

        if (GUI.Button(new Rect(questionX, questionY + 100, 150, 60), "Smaller"))
        {
            TL.Exercises.Interval.Comparison.Answer(TL.Exercises.Interval.Comparison.QuestionAnswerEnum.Smaller);
        }

        if (GUI.Button(new Rect(questionX, questionY + 200, 150, 60), "Larger"))
        {
            TL.Exercises.Interval.Comparison.Answer(TL.Exercises.Interval.Comparison.QuestionAnswerEnum.Larger);
        }

        if (GUI.Button(new Rect(questionX, questionY + 300, 150, 60), "Equal"))
        {
            TL.Exercises.Interval.Comparison.Answer(TL.Exercises.Interval.Comparison.QuestionAnswerEnum.Equal);
        }
    }

    void onIntervalIdentification()
    {

        if (GUI.Button(new Rect(100, 100, 150, 60), "Play Interval"))
        {
            BlockUI();
            var data = TL.Exercises.Interval.Identification.data;
            TL.Providers.Midi.PlayInterval(TL.Exercises.Interval.settings.identificationSettings.playModeFlags, data.lowPitch, data.highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Exercises.Interval.settings.toneGap, () =>
            {
                UnblockUI();
            });
        }

        GUI.Label(new Rect(questionX, questionY + 10, 300, 40), "Select the correct interval name");

        var values = Enum.GetValues(typeof(TL.Enums.Interval.IntervalFlags));
        int yPos = questionY + 40;
        foreach (TL.Enums.Interval.IntervalFlags interval in values)
        {
            if (GUI.Button(new Rect(questionX, yPos, 150, 30), "Answer with " + interval))
            {
                TL.Exercises.Interval.Identification.Answer((int)interval);
            }
            yPos += 40;
        }
    }

    void onChordIdentification()
    {
        if (GUI.Button(new Rect(100, 100, 150, 60), "Play Chord"))
        {
            BlockUI();

            var data = TL.Exercises.Chord.Identification.data;

            TL.Providers.Midi.PlayPitches(TL.Exercises.Chord.settings.identificationSettings.playModeFlags, data.pitches, TL.Exercises.Chord.settings.toneDuration, TL.Exercises.Chord.settings.toneGap, () =>
            {
                UnblockUI();
            });
        }

        GUI.Label(new Rect(questionX, questionY, 300, 40), "Input the correct chord (TODO)");
    }

    void onScaleIdentification()
    {
        if (GUI.Button(new Rect(100, 100, 150, 60), "Play Scale"))
        {
            BlockUI();

            var data = TL.Exercises.Scale.Identification.data;

            TL.Providers.Midi.PlayPitches(TL.Enums.PlayModeFlags.Ascending, data.pitches, TL.Exercises.Scale.settings.toneDuration, TL.Exercises.Scale.settings.toneGap, () =>
            {
                UnblockUI();
            });
        }

        var values = Enum.GetValues(typeof(TL.Enums.Scale.ScaleFlags));
        int baseY = questionY + 40;
        int yPos = baseY;
        int xPos = questionX;
        foreach (TL.Enums.Scale.ScaleFlags scale in values)
        {
            if (GUI.Button(new Rect(xPos, yPos, 250, 30), "Answer with " + scale))
            {
                TL.Exercises.Scale.Identification.Answer(scale);
            }
            yPos += 40;

            if (yPos > baseY + 300)
            {
                yPos = baseY;
                xPos += 300;
            }
        }
    }

    void onChordInversions()
    {
        if (GUI.Button(new Rect(100, 100, 150, 60), "Play Inversion"))
        {
            BlockUI();

            var data = TL.Exercises.Chord.Inversions.data;

            TL.Providers.Midi.PlayPitches(TL.Enums.PlayModeFlags.Ascending, data.pitches, TL.Exercises.Chord.settings.toneDuration, TL.Exercises.Chord.settings.toneGap, () =>
            {
                UnblockUI();
            });
        }

        var values = Enum.GetValues(typeof(TL.Enums.Chord.ChordInversionTypeFlags));
        int yPos = questionY + 40;
        foreach (TL.Enums.Chord.ChordInversionTypeFlags inversion in values)
        {
            if (GUI.Button(new Rect(questionX, yPos, 250, 30), "Answer with " + inversion))
            {
                TL.Exercises.Chord.Inversions.Answer(inversion);
            }
            yPos += 40;
        }
    }

    void onChordProgressions()
    {
        if (GUI.Button(new Rect(100, 100, 150, 60), "Play Progression"))
        {
            BlockUI();
            var data = TL.Exercises.Chord.Progressions.data;

            TL.Providers.Midi.PlayPitches(TL.Enums.PlayModeFlags.Ascending, data.pitches, TL.Exercises.Chord.settings.toneDuration, TL.Exercises.Chord.settings.toneGap, () =>
            {
                UnblockUI();
            });
        }

        GUI.Label(new Rect(questionX, questionY, 300, 40), "Input the correct progression (TODO)");
    }


    private void DrawRhythmEvent(double baseX, float y, float ofs, ForieroEngine.Music.Training.Core.Classes.Rhythms.RhythmEvent item)
    {
        var len = (item.endTime - item.startTime) * 100;
        var h = 20;

        if (item.isRest)
        {
            h /= 2;
            y += h;
        }

        GUI.Box(new Rect((float)(baseX + item.startTime * 100 + ofs), y, (float)(len - ofs * 2), h), GUIContent.none);
    }


    void onRhythms()
    {
        float baseX = questionX;
        var y = questionY + 200;
        float ofs = 0;

        var data = TL.Exercises.Rhythm.Imitation.data;

        foreach (var item in data.questionEvents)
        {
            DrawRhythmEvent(baseX, y, ofs, item);
        }

        if (data.isRecordingInput)
        {
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                rhythmEventTime = Time.time;
                rhythmEventStarted = true;
            }
            else
            if (Input.GetKeyUp(KeyCode.Space) && rhythmEventStarted)
            {
                var evt = new TL.RhythmEvent();
                evt.startTime = rhythmEventTime - recordTime;
                evt.endTime = Time.time - recordTime;
                evt.isRest = false;
                rhythmEvents.Add(evt);

                rhythmEventStarted = false;
            }*/

            foreach (var item in data.rhythmInput)
            {
                DrawRhythmEvent(baseX, y + 25, ofs, item);
            }

            var delta = TL.Exercises.Rhythm.Imitation.GetCurrentTime();

            GUI.Box(new Rect((float)(baseX + delta * 100), y - 20, 10, 20), GUIContent.none);

            /*if (delta > lastTime + 0.1f)
            {
                recordingInput = false;

                if (TL.CompareRhythms(TL.Exercises.Rhythm.Imitation.events, rhythmEvents, 0.25f))
                {
                    currentAnswer = AnswerEnum.Correct;
                }
                else
                {
                    currentAnswer = AnswerEnum.Wrong;
                }
            }*/

            return;
        }

        if (GUI.Button(new Rect(questionX, questionY, 150, 60), "Play Rhythm"))
        {
            BlockUI();

            TL.Exercises.Rhythm.Imitation.PlayRhythm(() =>
           {
               UnblockUI();
           });
        }

        if (GUI.Button(new Rect(100, 300, 150, 60), "Input Rythm"))
        {
            TL.Exercises.Rhythm.Imitation.Execute(onFinished);
        }

    }

}
