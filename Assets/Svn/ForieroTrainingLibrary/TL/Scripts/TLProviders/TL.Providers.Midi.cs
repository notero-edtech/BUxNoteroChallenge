/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training.Classes.Providers;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Providers
        {
            public static MidiProvider midi = new MidiProvider();

            public static partial class Midi
            {
                public static void NoteDispatch(int interval1Low, float toneDuration, float toneGap, int attack, int instrumentChannel, Action AStart = null, Action AStop = null)
                {
                    midi.NoteDispatch(interval1Low, toneDuration, toneGap, attack, instrumentChannel, AStart, AStop);
                }

                public static void Rhythm(double scheduleTime = 0)
                {
                    midi.SchedulePercussion(Exercises.settings.rhythm, Exercises.settings.rhythmAttack, (float)scheduleTime);
                }

                public static void MetronomeHeavy(double scheduleTime = 0)
                {
                    midi.SchedulePercussion(Exercises.settings.metronomHeavy, Exercises.settings.metronomHeavyAttack, (float)scheduleTime);
                }

                public static void MetronomeLight(double scheduleTime = 0)
                {
                    midi.SchedulePercussion(Exercises.settings.metronomLight, Exercises.settings.metronomLightAttack, (float)scheduleTime);
                }

                public static void Reset()
                {
                    midi.Reset();
                }

                public static float ToneGap(float toneGap, float multiplier)
                {
                    return multiplier * toneGap;
                }

                // used by all chord exercises, plays an chord based on the settings
                // pitches must be ordered from low to high
                public static void PlayPitches(Enums.PlayModeFlags playMode, int[] pitches, float toneDuration, float toneGap, Action onFinish)
                {
                    float delay = 0.5f;
                    switch (playMode)
                    {
                        case Enums.PlayModeFlags.Harmonic:
                            {
                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 0f), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == pitches.Length - 1 ? onFinish : null);
                                }
                                break;
                            }

                        case Enums.PlayModeFlags.Ascending:
                            {
                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, i), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == pitches.Length - 1 ? onFinish : null);
                                }
                                break;
                            }

                        case Enums.PlayModeFlags.Descending:
                            {
                                int n = 0;
                                for (int i = pitches.Length - 1; i >= 0; i--)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, n), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == 0 ? onFinish : null);
                                    n++;
                                }
                                break;
                            }

                        case Enums.PlayModeFlags.HarmonicThenAscending:
                            {
                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 0f), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, null);
                                }


                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 1 + i + delay), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == pitches.Length - 1 ? onFinish : null);
                                }

                                break;
                            }

                        case Enums.PlayModeFlags.HarmonicThenDescending:
                            {
                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 0f), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, null);
                                }

                                int n = 0;
                                for (int i = pitches.Length - 1; i >= 0; i--)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 1 + n + delay), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == 0 ? onFinish : null);
                                    n++;
                                }
                                break;
                            }

                        case Enums.PlayModeFlags.AscendingThenHarmonic:
                            {
                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, i), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, null);
                                }

                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 1 + delay), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == pitches.Length - 1 ? onFinish : null);
                                }

                                break;
                            }

                        case Enums.PlayModeFlags.DescendingThenHarmonic:
                            {
                                int n = 0;
                                for (int i = pitches.Length - 1; i >= 0; i--)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, n), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == 0 ? onFinish : null);
                                    n++;
                                }

                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 1 + delay), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == pitches.Length - 1 ? onFinish : null);
                                }

                                break;
                            }

                        case Enums.PlayModeFlags.AscendingThenDescending:
                            {
                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, i), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, null);
                                }

                                int n = 0;
                                for (int i = pitches.Length - 1; i >= 0; i--)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 1 + n + delay), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == 0 ? onFinish : null);
                                    n++;
                                }
                                break;
                            }

                        case Enums.PlayModeFlags.DescendingThenAscending:
                            {
                                int n = 0;
                                for (int i = pitches.Length - 1; i >= 0; i--)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, n), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == 0 ? onFinish : null);
                                    n++;
                                }

                                for (int i = 0; i < pitches.Length; i++)
                                {
                                    NoteDispatch(pitches[i], toneDuration, ToneGap(toneGap, 1 + i + delay), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, i == pitches.Length - 1 ? onFinish : null);
                                }
                                break;
                            }

                    }
                }

                // used by all interval exercises, plays an interval based on the settings
                public static void PlayInterval(Enums.PlayModeFlags playMode, int lowPitch, int highPitch, float toneDuration, float toneGap, Action onFinish)
                {
                    PlayPitches(playMode, new int[] { lowPitch, highPitch }, toneDuration, toneGap, onFinish);
                }

                //public static void Play(this Exercises.Chord.Identification.ChordIdentificationData data, Action onFinish)
                //{
                //    PlayChord(Exercises.Chord.settings.playMode, data.pitches, onFinish);
                //}


                //public static void Play(this Exercises.Chord.Inversions.ChordInversionData data, Action onFinish = null)
                //{
                //    for (int i = 0; i < data.toneValues.Length; i++)
                //    {
                //        NoteDispatch(data.toneValues[i], Exercises.settings.toneDuration, Exercises.settings.ToneGap(i), Exercises.settings.instrumentAttack, Exercises.settings.instrumentChannel, null, (i == data.toneValues.Length - 1) ? onFinish : null);
                //    }
                //}
            }
        }
    }
}
