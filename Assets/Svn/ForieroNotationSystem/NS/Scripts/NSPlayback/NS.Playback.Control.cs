/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
#if !UNITY_EDITOR
using UnityEngine.Scripting;
#endif

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static void Play()
        {
#if !UNITY_EDITOR
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
#endif
            NSPlayback.playbackState = PlaybackState.Playing;
        }

        public static void WaitForInput()
        {
            NSPlayback.playbackState = PlaybackState.WaitingForInput;
#if !UNITY_EDITOR
            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
#endif
        }

        public static void Pause()
        {
            NSPlayback.playbackState = PlaybackState.Pausing;
#if !UNITY_EDITOR
            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
#endif
        }

        public static void Stop()
        {
            NSPlayback.playbackState = PlaybackState.Stop;
#if !UNITY_EDITOR
            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
#endif
        }
    }
}
