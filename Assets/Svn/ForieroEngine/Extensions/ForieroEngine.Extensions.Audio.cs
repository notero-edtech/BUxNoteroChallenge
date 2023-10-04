using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {

        public static IEnumerator PlayOneShotDelayed(this AudioSource anAudioSource, AudioClip anAudioClip, float aDelay)
        {
            while (aDelay > 0)
            {
                yield return null;
                aDelay -= Time.deltaTime;
            }
            anAudioSource.PlayOneShot(anAudioClip);
        }

        public static AudioType PlatformAudioType()
        {
#if UNITY_EDITOR && UNITY_IOS
			return AudioType.MPEG;
#elif UNITY_IOS
			return AudioType.AUDIOQUEUE;
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA
            return AudioType.OGGVORBIS;
#elif UNITY_ANDROID
			return AudioType.MPEG;
#else
			return AudioType.OGGVORBIS;
#endif
        }

        public static string PlatformAudioExtension()
        {
#if UNITY_EDITOR && UNITY_IOS
			return ".mp3";
#elif UNITY_IOS
			return ".mp3";
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA
            return ".ogg";
#elif UNITY_ANDROID
			return ".mp3";
#else
			return ".ogg";
#endif
        }

        public static string PlatformRuntimeAudioExtension()
        {
#if UNITY_IOS
			return ".mp3";
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WSA
            return ".ogg";
#elif UNITY_ANDROID
			return ".mp3";
#else
			return ".ogg";
#endif
        }

        public static string PlatformFileProtocol()
        {
#if UNITY_EDITOR_OSX
            return "file://";
#elif UNITY_EDITOR_WIN
			return "file:///";
#elif UNITY_STANDALONE_WIN || UNITY_WSA
			return "file:///";
#else
			return "file://";
#endif
        }

        public static float ToDecibel(this float linear)
        {
            float dB;

            if (linear != 0)
                dB = 20.0f * Mathf.Log10(linear);
            else
                dB = -144.0f;

            return dB;
        }

        public static float ToLinear(this float dB)
        {
            float linear = Mathf.Pow(10.0f, dB / 20.0f);

            return linear;
        }

        // now create a pinned handle, so that the Garbage Collector will not move this object
        //_hGCFile = GCHandle.Alloc( buffer, GCHandleType.Pinned );
        // create the stream (AddrOfPinnedObject delivers the necessary IntPtr)
        // stream = Bass.BASS_StreamCreateFile(_hGCFile.AddrOfPinnedObject(), 0L, length, BASSFlag.BASS_SAMPLE_FLOAT);
        // if (stream != 0) Bass.BASS_ChannelStop (stream);
        // _hGCFile.Free();

        public static byte[] ToBuffer(this AudioClip ac)
        {
            if (!ac)
                return new byte[0];

            byte[] buffer = new byte[0];

            using (MemoryStream fs = new MemoryStream())
            {

                float[] samples = new float[ac.samples];

                ac.GetData(samples, 0);

                Int16[] intData = new Int16[samples.Length];
                //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

                Byte[] bytesData = new Byte[samples.Length * 2];
                //bytesData array is twice the size of
                //dataSource array because a float converted in Int16 is 2 bytes.

                int rescaleFactor = 32767; //to convert float to Int16

                for (int i = 0; i < samples.Length; i++)
                {
                    intData[i] = (short)(samples[i] * rescaleFactor);
                    Byte[] byteArr = new Byte[2];
                    byteArr = BitConverter.GetBytes(intData[i]);
                    byteArr.CopyTo(bytesData, i * 2);
                }

                fs.Write(bytesData, 0, bytesData.Length);

                long length = fs.Length;
                buffer = new byte[length];

                fs.Read(buffer, 0, (int)length);

            }

            return buffer;
        }

    }
}
