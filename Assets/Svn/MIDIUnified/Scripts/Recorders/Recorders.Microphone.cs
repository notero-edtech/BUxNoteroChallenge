using System.Linq;

namespace ForieroEngine.MIDIUnified.Recording
{
    public static partial class Recorders
    {
        public static string MicrophoneExistsOrDefault(this string microphoneName)
        {
            #if UNITY_WEBGL
            return null;
            #else
            microphoneName = UnityEngine.Microphone.devices.FirstOrDefault(m => m == microphoneName);
            microphoneName = UnityEngine.Microphone.devices.FirstOrDefault();
            return microphoneName;
            #endif
        }

        public static class Microphone
        {
            public static readonly string fileName = "";
            public static void Start(string microphoneName = null) { }
            public static void Stop(string microphoneName = null) { }
        }
    }
}
