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
            public static MicrophoneProvider microphone = new MicrophoneProvider();

            public static partial class Microphone
            {
                public static void Start(string deviceName = null)
                {
                    microphone.Start(deviceName);
                    microphone.recording = true;
                }

                public static void Stop(string deviceName = null)
                {
                    microphone.Stop(deviceName);
                    microphone.recording = false;
                }

                public static void Reset()
                {
                    microphone.Reset();
                }
            }
        }
    }
}
