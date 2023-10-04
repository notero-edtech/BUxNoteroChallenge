/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using ForieroEngine.Music.Training.Classes;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Calibration
        {
            public static class Tap
            {
                public static double delay = -1;

                private static List<double> delaySamples = new List<double>();

                private static bool isCalibrating;
                private static double calibrationTime;

                public static void StartCalibration()
                {
                    delay = -1;
                    delaySamples.Clear();

                    TL.Providers.Metronome.bpm = 40;
                    TL.Providers.Metronome.Start();

                    TL.Inputs.OnUpdate = UpdateCalibration;
                    TL.Inputs.OnTapDown = CalibrationOnTap;
                    calibrationTime = TL.Providers.Metronome.totalTime;

                    isCalibrating = true;
                }

                public static void AddCalibrationSample(double delay)
                {
                    UnityEngine.Debug.Log("new delay: " + delay);
                    delaySamples.Add(delay);
                }

                public static bool IsCalibrated()
                {
                    return delay >= 0;
                }

                public static bool IsCalibrating()
                {
                    return isCalibrating;
                }

                //https://en.wikipedia.org/wiki/Chebyshev%27s_inequality
                public static void StopCalibration()
                {
                    isCalibrating = false;

                    TL.Inputs.OnUpdate = null;
                    TL.Providers.Metronome.Stop();

                    /*var deviation = TL.Utilities.Statistics.StandardDeviation(delaySamples);
                    int count = 0;
                    foreach (var sample in delaySamples)
                    {
                        var dist = Math.Abs(sample - deviation);
                    }*/

                    double sum = 0;
                    foreach (var sample in delaySamples)
                    {
                        sum += sample;
                    }

                    sum /= (double)delaySamples.Count;

                    /*
                     * The most common way of having a Robust (the usual word meaning resistant to bad data) average is to use the median. 
                     * This is just the middle value in the sorted list (of half way between the middle two values)
                     */
                    delaySamples.Sort((x, y) => x.CompareTo(y));

                    if (delaySamples.Count == 0)
                    {
                        return;
                    }

                    var midValue = delaySamples[delaySamples.Count / 2];

                    delay = (midValue + sum) * 0.5f; // average with median

                    UnityEngine.Debug.Log("delay avg: " + delay);
                }



                private static void CalibrationOnTap()
                {
                    var delay = Math.Abs(TL.Providers.Metronome.totalTime - calibrationTime);
                    TL.Calibration.Tap.AddCalibrationSample(delay);
                }

                private static void UpdateCalibration()
                {
                    var delta = TL.Providers.Metronome.totalTime - calibrationTime;

                    if (delta < TL.Utilities.Rhythms.BeatsToSeconds(TL.Providers.Metronome.bpm))
                    {
                        return;
                    }

                    calibrationTime = TL.Providers.Metronome.totalTime;

                    TL.Providers.Midi.Rhythm();
                }

            }
        }
    }
}

