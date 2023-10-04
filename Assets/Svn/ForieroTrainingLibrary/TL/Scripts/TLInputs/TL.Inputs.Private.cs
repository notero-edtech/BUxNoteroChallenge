/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Inputs
        {
            static class Private
            {
                // volume level events //
                public static float volumeLevel;
                public static Action<float> OnVolumeLevel;

                // mouse and touch events //
                public static Action OnTapDown;
                public static Action OnTapUp;

                // midi keyboard events //
                public static Action<int> OnMidiOn;
                public static Action<int> OnMidiOff;

                // microphone recognized clap events //
                public static Action OnClap;

                // sung recognized midi events //
                public static Action<int, int> OnPitch;

                // sung recognized midi events //
                public static Action<float> OnTuner;

                // keyboard spacebar events//
                public static Action OnSpacebarDown;
                public static Action OnSpacebarUp;

                // periodical update for exercises that operate in time //
                public static Action OnUpdate;
            }
        }
    }
}
