/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using ForieroEngine.Music.Training.Classes;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;
using UnityEngine;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Calibration
        {
            public static class Clap
            {
                public enum ErrorEnum
                {
                    UnplugHeadphonesOrRaiseVolume,
                    UnequalClapDetected,
                    Undefined = int.MaxValue
                }

                static float time = 0;
                static bool detected = false;
                static bool calibrating = false;

                static List<double> scheduledClaps = new List<double>();
                static List<double> detectedClaps = new List<double>();

                public static Action<double> OnLatency;
                public static Action<ErrorEnum> OnError;

                public static void Calibrate()
                {
                    time = 0;
                    detected = false;
                    calibrating = false;

                    scheduledClaps.Clear();
                    detectedClaps.Clear();

                    TL.Inputs.OnClap -= OnClap;
                    TL.Inputs.OnUpdate -= OnUpdate;

                    TL.Inputs.OnClap += OnClap;
                    TL.Inputs.OnUpdate += OnUpdate;

                    MidiOut.SchedulePercussion(PercussionEnum.HandClap, 127, AudioSettings.dspTime + 0.5, true);
                }

                static void OnClap()
                {
                    if (detected)
                    {
                        detectedClaps.Add(AudioSettings.dspTime);
                    }
                    else
                    {
                        detected = true;
                    }
                }

                static void OnUpdate()
                {
                    time += Time.deltaTime;

                    if (time > 1f)
                    {
                        if (detected)
                        {
                            if (calibrating)
                            {
                                if (time > 6)
                                {
                                    Evaluate();
                                }
                            }
                            else
                            {
                                Run();
                            }
                        }
                        else
                        {
                            OnError?.Invoke(ErrorEnum.UnplugHeadphonesOrRaiseVolume);
                            Debug.LogError("Unplug headphones or raise volume!!!");
                            TL.Inputs.OnClap -= OnClap;
                            TL.Inputs.OnUpdate -= OnUpdate;
                        }

                    }
                }

                static void Run()
                {
                    for (int i = 0; i < 10; i++)
                    {
                        double scheduledTime = AudioSettings.dspTime + 0.5 * i;
                        scheduledClaps.Add(scheduledTime);
                        MidiOut.SchedulePercussion(PercussionEnum.HandClap, 127, scheduledTime, true);
                    }

                    calibrating = true;
                }

                static void Evaluate()
                {
                    TL.Inputs.OnClap -= OnClap;
                    TL.Inputs.OnUpdate -= OnUpdate;

                    if (scheduledClaps.Count == detectedClaps.Count)
                    {
                        double latency = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            latency += detectedClaps[i] - scheduledClaps[i];
                        }
                        latency /= 10.0;

                        Debug.Log("CLAP LATENCY : " + latency.ToString());

                        OnLatency?.Invoke(latency);
                    }
                    else
                    {
                        OnError?.Invoke(ErrorEnum.UnequalClapDetected);
                        Debug.LogError("Unequal claps count detected!!!");
                    }
                }
            }
        }
    }
}

//namespace ForieroEngine.Music.Training
//{
//    public static partial class TL
//    {
//        public static partial class Calibration
//        {
//            public static class Clap
//            {
//                public static double delay = -1;

//                private static List<double> delaySamples = new List<double>();

//                private static bool isCalibrating;
//                private static double calibrationTime;

//                public static void StartCalibration()
//                {
//                    delay = -1;
//                    delaySamples.Clear();

//                    TL.Providers.Metronome.bpm = 40;
//                    TL.Providers.Metronome.Start();

//                    TL.Inputs.OnUpdate = UpdateCalibration;
//                    TL.Inputs.OnClap = CalibrationOnClap;
//                    calibrationTime = TL.Providers.Metronome.totalTime;

//                    isCalibrating = true;
//                }

//                public static void AddCalibrationSample(double delay)
//                {
//                    UnityEngine.Debug.Log("new delay: " + delay);
//                    delaySamples.Add(delay);
//                }

//                public static bool IsCalibrated()
//                {
//                    return delay >= 0;
//                }

//                public static bool IsCalibrating()
//                {
//                    return isCalibrating;
//                }

//                //https://en.wikipedia.org/wiki/Chebyshev%27s_inequality
//                public static void StopCalibration()
//                {
//                    isCalibrating = false;

//                    TL.Inputs.OnUpdate = null;
//                    TL.Providers.Metronome.Stop();

//                    /*var deviation = TL.Utilities.Statistics.StandardDeviation(delaySamples);
//                    int count = 0;
//                    foreach (var sample in delaySamples)
//                    {
//                        var dist = Math.Abs(sample - deviation);
//                    }*/

//                    double sum = 0;
//                    foreach (var sample in delaySamples)
//                    {
//                        sum += sample;
//                    }

//                    sum /= (double)delaySamples.Count;

//                    /*
//                     * The most common way of having a Robust (the usual word meaning resistant to bad data) average is to use the median. 
//                     * This is just the middle value in the sorted list (of half way between the middle two values)
//                     */
//                    delaySamples.Sort((x, y) => x.CompareTo(y));

//                    if (delaySamples.Count == 0)
//                    {
//                        return;
//                    }

//                    var midValue = delaySamples[delaySamples.Count / 2];

//                    delay = (midValue + sum) * 0.5f; // average with median

//                    UnityEngine.Debug.Log("delay avg: " + delay);
//                }



//                private static void CalibrationOnClap()
//                {
//                    var delay = Math.Abs(TL.Providers.Metronome.totalTime - calibrationTime);
//                    AddCalibrationSample(delay);
//                }

//                private static void UpdateCalibration()
//                {
//                    var delta = TL.Providers.Metronome.totalTime - calibrationTime;

//                    if (delta < TL.Utilities.Rhythms.BeatsToSeconds(TL.Providers.Metronome.bpm))
//                    {
//                        return;
//                    }

//                    calibrationTime = TL.Providers.Metronome.totalTime;

//                    TL.Providers.Midi.Rhythm();
//                }

//            }
//        }
//    }
//}

