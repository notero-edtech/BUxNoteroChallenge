/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using UnityEngine;
using ForieroEngine.Music.Training.Classes.Providers;

public partial class TLUnityMicrophone : MonoBehaviour
{
    public static TLUnityMicrophone instance;

    AudioClip recordingClip;

    int samplePosition = 0;
    int lastSamplePosition = 0;

    int sampleDifference = 0;

    float[] samples = new float[0];

    private int minFreq, maxFreq;

    public void StartRecording(string deviceName = null)
    {
        #if !UNITY_WEBGL
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

        if ((minFreq + maxFreq) == 0)
        {
            maxFreq = 44100;
        }

        TLUnityMicrophoneProvider.provider.Initialize(maxFreq);

        recordingClip = Microphone.Start(deviceName, true, 100, maxFreq);

        while (Microphone.GetPosition(null) < 0) { }

        Debug.Log("TLUnityMicrophone - StartRecording");
        #endif
    }

    public void StopRecording(string deviceName = null)
    {
        #if !UNITY_WEBGL
        Microphone.End(deviceName);
        recordingClip = null;

        Debug.Log("TLUnityMicrophone - EndRecording");
        #endif
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        #if !UNITY_WEBGL
        if (recordingClip)
        {
            samplePosition = Microphone.GetPosition(null);

            sampleDifference = samplePosition - lastSamplePosition;

            if (sampleDifference > 0)
            {
                samples = new float[sampleDifference * recordingClip.channels];

                recordingClip.GetData(samples, lastSamplePosition);

                TLUnityMicrophoneProvider.provider.Update(samples);
            }

            lastSamplePosition = samplePosition;
        }
        #endif
    }
}
