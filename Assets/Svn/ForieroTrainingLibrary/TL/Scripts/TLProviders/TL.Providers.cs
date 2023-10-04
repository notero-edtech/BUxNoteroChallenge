/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.Training.Classes.Providers;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Providers
        {
            public static void Reset()
            {
                Microphone.Reset();
                Midi.Reset();
                Metronome.Reset();
            }
        }
    }
}
