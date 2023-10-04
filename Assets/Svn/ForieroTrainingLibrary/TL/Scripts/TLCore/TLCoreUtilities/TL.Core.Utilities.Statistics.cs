/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
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
            public static class Statistics
            {
                public static double Mean(List<double> values)
                {
                    return values.Count == 0 ? 0 : Mean(values, 0, values.Count);
                }

                public static double Mean(List<double> values, int start, int end)
                {
                    double s = 0;

                    for (int i = start; i < end; i++)
                    {
                        s += values[i];
                    }

                    return s / (end - start);
                }

                public static double Variance(List<double> values)
                {
                    return Variance(values, Mean(values), 0, values.Count);
                }

                public static double Variance(List<double> values, double mean)
                {
                    return Variance(values, mean, 0, values.Count);
                }

                public static double Variance(List<double> values, double mean, int start, int end)
                {
                    double variance = 0;

                    for (int i = start; i < end; i++)
                    {
                        variance += Math.Pow((values[i] - mean), 2);
                    }

                    int n = end - start;
                    if (start > 0) n -= 1;

                    return variance / (n);
                }

                public static double StandardDeviation(List<double> values)
                {
                    return values.Count == 0 ? 0 : StandardDeviation(values, 0, values.Count);
                }

                public static double StandardDeviation(List<double> values, int start, int end)
                {
                    double mean = Mean(values, start, end);
                    double variance = Variance(values, mean, start, end);

                    return Math.Sqrt(variance);
                }
            }
        }
    }
}
