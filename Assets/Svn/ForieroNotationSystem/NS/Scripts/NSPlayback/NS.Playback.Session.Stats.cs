/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static partial class Session
        {
            public static partial class Stats
            {
                
                
                internal static void Reset()
                {
                    
                }
                
                #region PRIVATE
                
                [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
                private static void InitInternal(){
                    NSPlayback.OnPlaybackStateChanged += OnPlaybackStateChanged;
                    NSObject.MidiMessageData.OnToneOnFeedbackThreaded += OnToneOnFeedbackThreaded;
                    NSObject.MidiMessageData.OnToneOffFeedbackThreaded += OnToneOffFeedbackThreaded;
                }

                private static void OnToneOnFeedbackThreaded(NSObject.MidiMessageData o)
                {
                    
                }

                private static void OnToneOffFeedbackThreaded(NSObject.MidiMessageData o)
                {
                    
                }

                private static void OnPlaybackStateChanged(PlaybackState state)
                {
                    if (state == PlaybackState.Finished){
                        // Calculate Stats //
                        
                        OnStatsReady?.Invoke();
                    }
                }
                
                #endregion
            }           
        }
    }
}
