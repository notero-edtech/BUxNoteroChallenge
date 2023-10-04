/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Threading;
using System.Threading.Tasks;
using ForieroEngine.MIDIUnified.Plugins;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static partial class Time
        {
            private static readonly DSP DSPTimeProvider = new DSP();

            public class DSP : ITimeProvider
            {
                public string id = "NSDSP";
                public string Id => id;
                public DSP() { this.Register(); }
                ~DSP() { this.Unregister(); }

                private static double deltaTime = 0;
                private static double lastTime = -1;
                private static volatile bool terminate = false;
                private static volatile bool running = false;
                private static volatile int sleep = 2;

                public static volatile float time = 0;
                public static float startTime = 0f;

                public static float DSPTimeToTimeOffset => (time - Time.time);

                public static void Terminate() => terminate = true;

                public void EnableTimeProvider() { }
                public void DisableTimeProvider() { }

                public static void Init()
                {
                    if (running) return;

                    running = true;

                    Task.Run(() =>
                    {
                        while (!terminate)
                        {
                            if (lastTime < 0) lastTime = AudioSettings.dspTime;

                            deltaTime = AudioSettings.dspTime - lastTime;
                            lastTime = AudioSettings.dspTime;

                            if (_playbackState is PlaybackState.Playing) time = ((float)AudioSettings.dspTime - startTime) * _speed;
                            else startTime = (float)AudioSettings.dspTime - time / _speed;
                            
                            Thread.Sleep(sleep);
                        }

                        running = false;
                    });
                }

                public float GetTime() => time;
                public void SetTime(float value) { }

                public static void InitDspTimes()
                {                    
                    Metronome.StartTime = startTime = (float)AudioSettings.dspTime - Time.time / _speed;
                    time = ((float)AudioSettings.dspTime - startTime) * _speed;
                }
            }
        }
    }
}
