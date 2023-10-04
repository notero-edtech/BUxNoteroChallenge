/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using UnityEngine;

namespace ForieroEngine.Music.Training.Classes.Providers
{
    public class TLUnityMetronomeProvider : MetronomeProvider
    {
        public static TLUnityMetronome tlUnityMetronome;
        public static MetronomeProvider provider;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            var stopWatch = Debug.isDebugBuild ? System.Diagnostics.Stopwatch.StartNew() : null;
            provider = new TLUnityMetronomeProvider() as MetronomeProvider;
            TL.Providers.metronome = provider;

            GameObject go = Resources.Load<GameObject>("TL/TLUnityMetronome");

            tlUnityMetronome = GameObject.Instantiate(go).GetComponent<TLUnityMetronome>();

            GameObject.DontDestroyOnLoad(tlUnityMetronome);
            if(Debug.isDebugBuild) Debug.Log("METHOD STOPWATCH (TLUnityMetronomeProvider - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

        public override void Reset() { Stop(); UnMute(); }
        public override double Start() => tlUnityMetronome.StartMetronome();
        public override void Stop() => tlUnityMetronome.StopMetronome();
        public override void Mute() => tlUnityMetronome.MuteMetronome();
        public override void UnMute() => tlUnityMetronome.UnMuteMetronome();
        public override void ScheduleEvent(double time, Action onScheduledEvent) => tlUnityMetronome.ScheduleEvent(time, onScheduledEvent);
        public override void CancelScheduledEvents() => tlUnityMetronome.CancelScheduledEvents();
    }
}
