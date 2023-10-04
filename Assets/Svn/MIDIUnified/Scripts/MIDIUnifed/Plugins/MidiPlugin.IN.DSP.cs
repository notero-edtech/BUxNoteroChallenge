using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Plugins
{
    public static partial class MidiINPlugin
    {
        public static class DSP
        {
            public class Data
            {
                public volatile bool On = false;
                public float HoldOnTime => ReleasedTime - PressedTime;
                public volatile float PressedTime = 0f;
                public volatile float ReleasedTime = 0f;
            }
            
            private static bool _initialized = false;
            public static readonly Data[] Tone = new Data[128];
            public static readonly Data[] ControlChange = new Data[128];

            public static void ToneOn(int index)
            {
                if (index < 0 || index >= Tone.Length) return;
                Tone[index].PressedTime = (float)AudioSettings.dspTime; 
                Tone[index].On = true;
            }
            
            public static void ToneOff(int index)
            {
                if (index < 0 || index >= Tone.Length) return;
                Tone[index].ReleasedTime = (float)AudioSettings.dspTime; 
                Tone[index].On = false;
            }
            
            internal static void Init()
            {
                if (_initialized) return;
                
                for (var i = 0; i < 128; i++)
                {
                    Tone[i] = new();
                    ControlChange[i] = new Data();
                }
                
                _initialized = true;
            }
        }
    }
}