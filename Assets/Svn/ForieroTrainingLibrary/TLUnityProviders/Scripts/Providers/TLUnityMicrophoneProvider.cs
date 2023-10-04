/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.Training.Classes.Providers
{
    public class TLUnityMicrophoneProvider : MicrophoneProvider
    {
        public static TLUnityMicrophone tlUnityMicrophone;
        public static MicrophoneProvider provider;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            System.Diagnostics.Stopwatch stopWatch = Debug.isDebugBuild ? System.Diagnostics.Stopwatch.StartNew() : null;
            provider = new TLUnityMicrophoneProvider() as MicrophoneProvider;
            TL.Providers.microphone = provider;

            GameObject go = Resources.Load<GameObject>("TL/TLUnityMicrophone");

            tlUnityMicrophone = GameObject.Instantiate(go).GetComponent<TLUnityMicrophone>();

            GameObject.DontDestroyOnLoad(tlUnityMicrophone);
            if(Debug.isDebugBuild) Debug.Log("METHOD STOPWATCH (TLUnityMicrophoneProvider - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

        public override void Start(string deviceName = null)
        {
            Debug.Log("MicrophoneProvider - Start");
            if (tlUnityMicrophone)
            {
                tlUnityMicrophone.StartRecording(deviceName);
            }
        }

        public override void Stop(string deviceName = null)
        {
            Debug.Log("MicrophoneProvider - Stop");
            if (tlUnityMicrophone)
            {
                tlUnityMicrophone.StopRecording(deviceName);
            }
        }

        public override void Reset()
        {
            Debug.Log("MicrophoneProvider - RESET");
        }


    }
}
