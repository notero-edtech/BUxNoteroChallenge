/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        public static partial class Enums
        {
            public static class Microphone
            {
                [Flags]
                public enum DetectionFlags
                {
                    Clap = 1 << 0,
                    Pitch = 1 << 1,
                    Tuner = 1 << 2,
                }
            }
        }
    }
}
