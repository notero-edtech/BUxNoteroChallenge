/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training.Core.Extensions
{
    public static partial class TLCoreIntervalExtensions
    {
        public static string ToName(this TL.Enums.Interval.IntervalFlags interval)
        {
            string result = "";
            switch (interval)
            {
                case TL.Enums.Interval.IntervalFlags.Unison:
                    result = "Unisono";
                    break;
                case TL.Enums.Interval.IntervalFlags.Minor2nd:
                    result = "Minor 2nd";
                    break;
                case TL.Enums.Interval.IntervalFlags.Major2nd:
                    result = "Major 2nd";
                    break;
                case TL.Enums.Interval.IntervalFlags.Minor3rd:
                    result = "Minor 3rd";
                    break;
                case TL.Enums.Interval.IntervalFlags.Major3rd:
                    result = "Major 3rd";
                    break;
                case TL.Enums.Interval.IntervalFlags.Perfect4th:
                    result = "Perfect 4th";
                    break;
                //case TL.Enums.Interval.IntervalFlags.Diminished5th:
                //result = "Diminished 5th";
                //break;
                case TL.Enums.Interval.IntervalFlags.Perfect5th:
                    result = "Perfect 5th";
                    break;
                case TL.Enums.Interval.IntervalFlags.Minor6th:
                    result = "Minor 6th";
                    break;
                case TL.Enums.Interval.IntervalFlags.Major6th:
                    result = "Major 6th";
                    break;
                case TL.Enums.Interval.IntervalFlags.Minor7th:
                    result = "Minor 7th";
                    break;
                case TL.Enums.Interval.IntervalFlags.Major7th:
                    result = "Major 7th";
                    break;
            }
            return result;
        }

        public static void Include(this TL.Enums.Interval.IntervalFlags interval, bool value)
        {
            intervalInclusion[(int)interval] = value;
        }

        public static bool Included(this TL.Enums.Interval.IntervalFlags interval)
        {
            return intervalInclusion[(int)interval];
        }

        public static bool[] intervalInclusion = new bool[System.Enum.GetValues(typeof(TL.Enums.Interval.IntervalFlags)).Length];

        public static string ToName(this TL.Enums.Chord.ChordInversionTypeFlags inversion)
        {
            switch (inversion)
            {
                case TL.Enums.Chord.ChordInversionTypeFlags.Root: return "Root";
                case TL.Enums.Chord.ChordInversionTypeFlags.First: return "1st Inversion";
                case TL.Enums.Chord.ChordInversionTypeFlags.Second: return "2nd Inversion";
                case TL.Enums.Chord.ChordInversionTypeFlags.Third: return "3rd Inversion";
                default: return "Invalid";
            }

        }

        public static TL.Enums.Interval.IntervalFlags GetIntervalBetweenTones(TL.Enums.ToneEnum low, TL.Enums.ToneEnum high)
        {
            int lowVal = (int)low;
            int highVal = (int)high;

            if (highVal < lowVal)
            {
                highVal += 12; // jump to next octave
            }

            int diff = highVal - lowVal;
            return (TL.Enums.Interval.IntervalFlags)diff;
        }

        public static TL.Enums.ToneEnum GetNextTone(TL.Enums.ToneEnum low, TL.Enums.Interval.IntervalFlags interval)
        {
            int lowVal = (int)low;
            int diff = (int)interval;
            int highVal = (lowVal + diff) % 12;
            return (TL.Enums.ToneEnum)highVal;
        }

        public static TL.Enums.ToneEnum GetPreviousTone(TL.Enums.ToneEnum high, TL.Enums.Interval.IntervalFlags interval)
        {
            int highVal = (int)high;
            int diff = (int)interval;
            int lowVal = (highVal - diff);
            if (lowVal < 0)
            {
                lowVal += 12;
            }

            return (TL.Enums.ToneEnum)lowVal;
        }


        // used by all interval exercises, plays an interval based on the settings
        public static void PlayInterval(this TL.Enums.PlayModeFlags playMode, int lowPitch, int highPitch, Action onFinish)
        {
            float delay = 0.5f;
            switch (playMode)
            {
                case TL.Enums.PlayModeFlags.Harmonic:
                    {
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.Ascending:
                    {
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.Descending:
                    {
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.HarmonicThenAscending:
                    {
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);

                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.HarmonicThenDescending:
                    {
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);

                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.AscendingThenHarmonic:
                    {
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);

                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.DescendingThenHarmonic:
                    {
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);

                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.AscendingThenDescending:
                    {
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);

                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

                case TL.Enums.PlayModeFlags.DescendingThenAscending:
                    {
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 0f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);

                        TL.Providers.Midi.NoteDispatch(lowPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 1f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, null);
                        TL.Providers.Midi.NoteDispatch(highPitch, TL.Exercises.Interval.settings.toneDuration, TL.Providers.Midi.ToneGap(TL.Exercises.Interval.settings.toneGap, delay + 2f), TL.Exercises.settings.instrumentAttack, TL.Exercises.settings.instrumentChannel, null, onFinish);
                        break;
                    }

            }

        }

    }
}

