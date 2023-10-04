/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using UnityEngine;
using UnityEngine.Scripting;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static class Metronome
        {
            private static bool _mute = false;
            public static bool Mute
            {
                get => _mute;
                set
                {
                    _mute = value;
                    if (value) MIDIPercussion.Mute(); else MIDIPercussion.UnMute();
                }
            }

            private static int _subdivisions = 0;
            public static int Subdivisions {
                get => _subdivisions;
                set
                {
                    _subdivisions = Mathf.Clamp(value, 0, int.MaxValue);
                    Beats.Cancel();
                    Beats.Schedule();
                }
            }
            
            public static float PendulumAngle => _pendulumAngle = (60f * NS.Easing.Sinusoidal.InOut((float)NormalizedBeatTime) - 30f) * (_pendulumFlip ? 1f : -1f);
            public static bool PendulumFlip { get => _pendulumFlip; set => _pendulumFlip = value; }
        }
    }
}
