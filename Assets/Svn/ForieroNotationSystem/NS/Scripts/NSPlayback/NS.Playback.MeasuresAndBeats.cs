/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public class Measure
        {
            public Measure(int number, float time, TimeSignatureStruct timeSignature)
            {
                this.number = number;
                this.time = time;
                this.timeSignatureStruct = timeSignature;
            }

            public readonly int number;
            public readonly float time;
            public float totalTime;
            public TimeSignatureEnum timeSignatureEnum = TimeSignatureEnum.Undefined;
            public TimeSignatureStruct timeSignatureStruct = new();
            public KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined;

            public Measure nextMeasure;

            // int = beat number in measure //
            public Dictionary<int, Beat> beats = new Dictionary<int, Beat>();

            public Beat GetBeatByTime(float time)
            {
                if (beats.Count < 1) { return null; }

                var min = int.MaxValue;
                var max = int.MinValue;

                foreach (var key in beats.Keys)
                {
                    if (key < min) min = key;
                    if (key > max) max = key;
                }

                var lastBeat = beats[min];

                for (var i = min; i <= max; i++)
                {
                    if (NS.debug) { Assert.IsTrue(beats.ContainsKey(i), string.Format("Beat not found -> Min : {0} Max : {1} beat = {2}", min, max, i)); }
                    var beat = beats[i];
                    if (time <= beat.time) { return lastBeat; } else { lastBeat = beat; }
                }

                return lastBeat;
            }

            public Beat GetNextBeatByTime(float time)
            {
                var result = GetBeatByTime(time);
                if (result != null && result.number < beats.Count) { return beats[result.number + 1]; }
                return null;
            }
        }

        public class Beat
        {
            public Beat(int number, float time, Measure measure)
            {
                this.number = number;
                this.time = time;
                this.measure = measure;
            }

            public readonly int number;
            public readonly float time;
            public float totalTime;
            public readonly Measure measure;

            public Beat nextBeat;

            public float timeInMeasure
            {
                get
                {
                    return time - measure.time;
                }
            }
        }

        // int = measure number //
        public static Dictionary<int, Measure> measures = new Dictionary<int, Measure>();
        private static readonly Comparer<float> comparer = Comparer<float>.Default;

        public static Measure GetMeasureByTime(float time)
        {
            if (measures.Count == 0) return null;

            // binary search for nearest valuu //
            var lo = 1;
            var hi = measures.Count;
            var index = -1;

            while (lo <= hi)
            {
                var median = lo + (hi - lo >> 1);
                var num = comparer.Compare(measures[median].time, time);
                if (num == 0) { index = median; break; }
                if (num < 0) { lo = median + 1; } else { hi = median - 1; }
            }

            if (index == -1) index = lo;

            index--;

            index = Mathf.Clamp(index, 1, measures.Count);

            if (!measures.ContainsKey(index)) Debug.LogError("Measure binary searhd errer : " + index);

            return measures[index];

            // var lastMeasure = measures[1];

            //for (int i = 2; i <= measures.Count; i++)
            //{
            //    var measure = measures[i];

            //    if (time <= measure.time)
            //    {
            //        return lastMeasure;
            //    }

            //    lastMeasure = measure;
            //}

            //return lastMeasure;
        }

        public static Measure GetNextMeasureByTime(float time)
        {
            var result = GetMeasureByTime(time);
            if (result != null && result.number < measures.Count) { return measures[result.number + 1]; }
            return null;
        }
    }
}
