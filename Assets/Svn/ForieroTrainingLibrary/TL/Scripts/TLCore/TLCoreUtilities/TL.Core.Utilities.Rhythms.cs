/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Classes;
using ForieroEngine.Music.Training.Core.Classes.Rhythms;
using ForieroEngine.Music.Training.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        static partial class Utilities
        {
            public static partial class Rhythms
            {
                public static double BeatsToMiliseconds(double BPM)
                {
                    return 60000 / BPM;
                }

                public static double BeatsToSeconds(double BPM)
                {
                    return BeatsToMiliseconds(BPM) * 0.001;
                }

                private static Dictionary<int, float>  durationMap = null;
                private static void InitDurationMap()
                {
                    if (durationMap != null)
                    {
                        return;
                    }

                    durationMap = new Dictionary<int, float>();
                    durationMap[1] = 8;
                    durationMap[2] = 4;
                    durationMap[3] = 2;
                    durationMap[4] = 1;
                    durationMap[5] = 1 / 2.0f;
                    durationMap[6] = 1 / 4.0f;
                    durationMap[7] = 1 / 8.0f;
                    durationMap[8] = 1 / 16.0f;
                    durationMap[9] = 1 / 32.0f;
                }

                public static double GetNoteRelativeDuration(TL.Enums.DurationFlags duration)
                {
                    InitDurationMap();

                    bool isDotted = false;

                    if (duration.HasFlag(TL.Enums.DurationFlags._dot))
                    {
                        isDotted = true;
                        duration.Remove(TL.Enums.DurationFlags._dot);
                    }

                    var index = duration.ToBitNumber();

                    double result = durationMap[index];


                    if (isDotted )
                    {
                        result *= 1.5;
                    }

                    return result;
                }

                public static double GetNoteRelativeDuration(TL.Enums.NoteAndRestFlags duration)
                {
                    return GetNoteRelativeDuration((Enums.DurationFlags)duration);
                }

                public static double NoteDurationToSeconds(TL.Enums.DurationFlags duration, double bpm)
                {
                    return GetNoteRelativeDuration(duration) * BeatsToSeconds(bpm);
                }

                public static double NoteDurationToSeconds(TL.Enums.NoteAndRestFlags duration, double bpm)
                {
                    return GetNoteRelativeDuration(duration) * BeatsToSeconds(bpm);
                }

                public static double GetNotePerBeatValue(TL.Enums.NotePerBeatFlags notesPerBeat)
                {
                    return notesPerBeat.ToBitNumber();
                }

                //// note, only 4/4 time for now
                public static List<RhythmEvent> AnalyseRhythmEvents(List<RhythmItem> items, double bpm)
                {
                    

                    var result = new List<RhythmEvent>();

                    double curTime = 0;
                    foreach (var item in items)
                    {
                        var evt = new RhythmEvent();
                        evt.startTime = curTime;

                        var duration = NoteDurationToSeconds(item.duration, bpm);

                        curTime += duration;

                        evt.endTime = curTime;
                        evt.isRest = item.isRest;

                       result.Add(evt);
                    }

                    return result;
                }

                // a larger percentage allows for less precision from users, a smaller percentage will require high accuracy from the user
                public static bool CompareRhythms(List<RhythmEvent> A, List<RhythmEvent> B, float maxErrorInPercentage = 0.25f)
                {
                    if (A.Count != B.Count)
                    {
                        return false;
                    }

                    for (int i = 0; i < A.Count; i++)
                    {
                        var itemA = A[i];
                        var itemB = B[i];
                        var duration = itemA.endTime - itemA.startTime;

                        var error = duration * maxErrorInPercentage;

                        if (itemA.isRest != itemB.isRest)
                        {
                            return false;
                        }

                        if (itemB.startTime > itemA.startTime + error)
                        {
                            return false;
                        }

                        if (itemB.startTime < itemA.startTime - error)
                        {
                            return false;
                        }

                        if (itemB.endTime > itemA.endTime + error)
                        {
                            return false;
                        }

                        if (itemB.endTime < itemA.endTime - error)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                public static bool CompareClapping(List<RhythmEvent> A, List<RhythmEvent> B, float maxErrorInPercentage = 0.25f)
                {
                    A.RemoveAll(x => x.isRest);
                    B.RemoveAll(x => x.isRest);


                    if (A.Count != B.Count)
                    {
                        return false;
                    }

                    for (int i = 0; i < A.Count; i++)
                    {
                        var itemA = A[i];
                        var itemB = B[i];
                        var duration = itemA.endTime - itemA.startTime;

                        var error = duration * maxErrorInPercentage;

                        if (itemA.isRest != itemB.isRest)
                        {
                            return false;
                        }

                        if (itemB.startTime > itemA.startTime + error)
                        {
                            return false;
                        }

                        if (itemB.startTime < itemA.startTime - error)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                public static Enums.NoteAndRestFlags GetHalfDurationOf(Enums.NoteAndRestFlags duration)
                {
                    var temp = (int)(duration) * 2;
                    return (Enums.NoteAndRestFlags)temp;
                }

                public static Enums.NoteAndRestFlags GetDoubleDurationOf(Enums.NoteAndRestFlags duration)
                {
                    var temp = (int)(duration) / 2;
                    return (Enums.NoteAndRestFlags)temp;
                }

                //private static float _lastBPM_Tick = -9999;
                //private static int _metronomeTicks = 0;
                //private const float metronomeTickDuration = 100;

                //public static void PlayMetronome(float bpm)
                //{

                //    if (_lastBPM_Tick < 0)
                //    {
                //        _lastBPM_Tick = 0;
                //    }

                //    float delay = BeatsToSeconds(bpm);

                //    float delta = Time.time - _lastBPM_Tick;

                //    if (delta < delay)
                //    {
                //        return;
                //    }

                //    if (_metronomeTicks >= 4)
                //    {
                //        _metronomeTicks = 0;
                //    }

                //    bool strongBeat = (_metronomeTicks == 0);
                //    _metronomeTicks++;


                //    TL.Providers.Midi.NoteDispatch(strongBeat ? 75 : 76, metronomeTickDuration, 0, Exercises.settings.attack, 9, null, null);
                //    _lastBPM_Tick = Time.time;
                //}

                //public static void ResetMetronome()
                //{
                //    _metronomeTicks = 0;
                //    _lastBPM_Tick = -9999;
                //}

            }
        }
    }
}
