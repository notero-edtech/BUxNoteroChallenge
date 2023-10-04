/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.MIDIUnified.Plugins;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {                               
        public static partial class Time
        {
            public static void Init(TimeProvider timeProvider)
            {
                switch (timeProvider)
                {
                    case TimeProvider.DSPTime: TimeProviders.Init("NSDSP");break;
                    case TimeProvider.Time: TimeProviders.Init("NSBEHAVIOUR"); break;
                    case TimeProvider.AudioSource: TimeProviders.Init(NSPlayback.Audio.iAudioProvider.Id); break;
                    case TimeProvider.Midi: TimeProviders.Init("");break;
                    case TimeProvider.Unknown: TimeProviders.Init(""); break;
                }
            }
            
            private static float TimeWithEvent {
                get => time;
                set { time = value; OnTimeChanged?.Invoke(time); }
            }

            public static float TimeNormalized
            {
                get => _totalTime > 0 ? time / _totalTime : 0;
                set => UpdateTime(_totalTime * value);
            }
            
            public static float time { get; private set; } = 0;

            public static void UpdateTime(float t)
            {
                if (time >= _totalTime && playbackState == PlaybackState.Playing) playbackState = PlaybackState.Finished;
                else if(time < _totalTime && playbackState == PlaybackState.Finished) playbackState = PlaybackState.Pausing;
                t = t.Clamp(0, TotalTime);
                if (playbackState is not (PlaybackState.Playing or PlaybackState.WaitingForInput)) iTimeProvider.SetTime(t);
                TimeWithEvent = t;                                                     
                UpdateMeasuresAndBeats();                    
            }
                       
            private static float _totalTime = 0;
            public static float TotalTime
            {
                get => _totalTime;
                set { _totalTime = value; OnTotalTimeChanged?.Invoke(_totalTime); }
            }

            public static void Reset()
            {
                time = 0;
                _totalTime = 0;
            }                 
        }              
    }
}
