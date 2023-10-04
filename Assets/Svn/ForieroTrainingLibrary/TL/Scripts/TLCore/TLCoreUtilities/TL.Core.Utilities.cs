/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training.Classes;
using System;
using System.Collections.Generic;

using ForieroEngine.Music.Training.Core.Extensions;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Utilities
        {
            /// <summary>
            /// Returns a value between min and max - 1 
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns></returns>
            public static int RandomInt(int min, int max)
            {
                return UnityEngine.Random.Range(min, max);
            }

            public static T RandomEnum<T>()
            {
                var v = Enum.GetValues(typeof(T));
                return (T)v.GetValue(RandomInt(0, v.Length));
            }

            public static T RandomItemFromList<T>(List<T> list)
            {
                return (T)list[RandomInt(0, list.Count)];
            }

            public static List<T> EnumFlagsToList<T>(System.Enum flags)
            {
                var list = new List<T>();
                var vals = Enum.GetValues(typeof(T));
                foreach (T value in vals)
                {
                    if (flags.Has(value))
                    {
                        list.Add(value);
                    }
                }

                return list;
            }

            public static T RandomEnumFromFlags<T>(System.Enum flags)
            {
                var list = EnumFlagsToList<T>(flags);

                if (list.Count == 0)
                {
                    throw new Exception("Cannot pick random " + typeof(T).Name + ", the list is empty");
                }

                return (T)list[RandomInt(0, list.Count)];
            }

            public static bool RandomBool(int percent = 50)
            {
                return (UnityEngine.Random.Range(0, 100) <= percent);
            }

            public static void ShuffleList<T>(IList<T> list, Random rnd)
            {
                for (var i = 0; i < list.Count; i++)
                    SwapItemsInList(list, i, rnd.Next(i, list.Count));
            }

            public static void SwapItemsInList<T>(IList<T> list, int i, int j)
            {
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            public static double GetStartTimeOfMidiStruct(IEnumerable<MidiStruct> items, int itemIndex, double bpm)
            {
                double curTime = 0;

                int curIndex = 0;

                foreach (var item in items)
                {
                    if (curIndex == itemIndex)
                    {
                        return curTime;
                    }

                    var duration = TL.Utilities.Rhythms.NoteDurationToSeconds(item.duration, bpm);
                    curTime += duration;
                }

                return curTime;
            }

            public static double GetEndTimeOfMidiStruct(IEnumerable<TL.Exercises.Data.Measure> measures, int itemIndex, double bpm)
            {
                double curTime = 0;

                int curIndex = 0;

                foreach (var measure in measures)
                {
                    foreach (var item in measure.notes)
                    {
                        var duration = TL.Utilities.Rhythms.NoteDurationToSeconds(item.duration, bpm);
                        curTime += duration;

                        if (curIndex == itemIndex)
                        {
                            return curTime;
                        }
                    }
                }

                return curTime;
            }

            public static bool CompareMidiLists(List<MidiStruct> A, List<MidiStruct> B)
            {
                if (A.Count != B.Count)
                {
                    return false;
                }

                for (int i=0; i<A.Count; i++)
                {
                    var itemA = A[i];
                    var itemB = B[i];

                    if (itemA.isRest != itemB.isRest)
                    {
                        return false;
                    }

                    if (itemA.duration != itemB.duration)
                    {
                        return false;
                    }

                    if (itemA.tighEnum != itemB.tighEnum)
                    {
                        return false;
                    }


                    if (itemA.midis.Length != itemB.midis.Length)
                    {
                        return false;
                    }

                    for (int j=0; j<itemA.midis.Length; j++)
                    {
                        if (itemA.midis[j] != itemB.midis[j])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

        }
    }
}
