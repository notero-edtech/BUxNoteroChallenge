/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.Training.Classes.Providers;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Providers
        {
            public static MetronomeProvider metronome = new ();

            public static partial class Metronome
            {
                public static Action<int> OnBeatsChanged; 
                private static int _beats = 4;
                public static int beats
                {
                    get => _beats;
                    set { _beats = value; OnBeatsChanged?.Invoke(value);}
                }

                public static Action<int> OnBPMChanged;
                private static int _bpm = 60;
                public static int bpm
                {
                    get => _bpm;
                    set { _bpm = value; OnBPMChanged?.Invoke(value);}
                }

                public static Action<int> OnSubdivisionsChanged;
                private static int _subdivisions = 0;
                public static int subdivisions
                {
                    get => _subdivisions;
                    set { _subdivisions = value; OnSubdivisionsChanged?.Invoke(value);}
                }
                
                public static bool pickupBar = false;

                #region Updated

                public static int beat = 0;
                public static double totalTime = 0f;
                public static double measureTime = 0f;

                #endregion

                public static void Reset() => metronome.Reset();
                
                // returning start offset time //
                public static double Start() => metronome.Start();
                public static void Stop() => metronome.Stop();
                public static void Mute() => metronome.Mute();
                public static void UnMute() => metronome.UnMute();
                public static void ScheduleEvent(double time, Action onScheduledEvent) => metronome.ScheduleEvent(time, onScheduledEvent);
                public static void CancelScheduledEvents() => metronome.CancelScheduledEvents();
            }
        }
    }
}
