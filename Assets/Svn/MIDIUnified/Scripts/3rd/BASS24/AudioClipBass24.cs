using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ForieroEngine.Audio.Recording;

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
#endif
using UnityEngine;

[Serializable]
public class AudioClipBass24
{
    public static List<AudioClipBass24> clips = new List<AudioClipBass24>();

    public int stream { get; private set; }
    public int streamFX { get; private set; }
    public bool decoding { get; }

    private GCHandle memhandle;
    private int syncHandle = 0;

    public bool loaded { get; private set; }

    public AudioClipBass24(bool decoding)
    {
        clips.Add(this);
        this.decoding = decoding;
    }

    ~AudioClipBass24()
    {
        Dispose();
    }

    public static AudioClipBass24 Create(string fileName, bool decoding = false)
    {
        var bytes = File.ReadAllBytes(fileName);
        return Create(bytes, decoding);
    }

    public static AudioClipBass24 Create(byte[] bytes, bool decoding = false)
    {
        if (!MIDISoundSettings.initialized)
        {
            return null;
        }

        var clipBass24 = new AudioClipBass24(decoding);
               
        clipBass24.Load(bytes);

        return clipBass24;
    }

    public static AudioClipBass24 Create(AudioClip clip, bool decoding = false)
    {
        if (!MIDISoundSettings.initialized) { return null; }
        var bytes = SaveWav.GetWavBytes(clip);
        return Create(bytes, decoding);
    }

    public void Load(byte[] bytes)
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        Dispose();

        if (!MIDISoundSettings.initialized) { return; }
        _samples = null;
        memhandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

        bool sampleFloat = true;

        stream = Bass.BASS_StreamCreateFile(memhandle.AddrOfPinnedObject(), 0, bytes.Length, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);
        if(stream == 0)
        {
            stream = Bass.BASS_StreamCreateFile(memhandle.AddrOfPinnedObject(), 0, bytes.Length, BASSFlag.BASS_STREAM_DECODE);
            if (IsBassError("BASS_StreamCreateFile"))
            {
                stream = 0;
                return;
            }
            sampleFloat = false;
            Debug.LogWarning("Creating 16bit non-floating stream!");
        }

        BASSFlag flags = BASSFlag.BASS_FX_FREESOURCE;

        if (sampleFloat) flags |= BASSFlag.BASS_SAMPLE_FLOAT;
        if (decoding) flags |= BASSFlag.BASS_STREAM_DECODE;

        streamFX = BassFx.BASS_FX_TempoCreate(stream,  flags);
        if (IsBassError("BASS_FX_TempoCreate"))
        {
            streamFX = 0;
            return;
        }

        loaded = true;
#endif
    }

    public float length
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        get
        {
            if (loaded && stream != 0)
            {
                var totalTimeInBytes = Bass.BASS_ChannelGetLength(stream);
                IsBassError("BASS_ChannelGetLength");

                var totalTime = (float)Bass.BASS_ChannelBytes2Seconds(stream, totalTimeInBytes);
                IsBassError("BASS_ChannelBytes2Seconds");

                return totalTime;
            }
            else
            {
                return 0;
            }
        }
#else
        get { return 0; }
#endif
    }

    public int channels
    {
        get
        {
            if (loaded && stream != 0)
            {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
                var info = Bass.BASS_ChannelGetInfo(stream);
                return info.chans;
#else
                return 0;
#endif
            }
            else
            {
                return 0;
            }
        }
    }

    float[] _samples;

    public float[] GetSamples()
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        if (!loaded)
        {
            return null;
        }

        if (_samples != null)
        {
            return _samples;
        }

        int samplerate = AudioSettings.outputSampleRate;

        float[] data = null;
        //create streams for re-sampling

        if (stream == 0)
        {
            return null;
        }

        int mixerStream = BassMix.BASS_Mixer_StreamCreate(samplerate, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);

        if (mixerStream == 0)
        {
            throw new Exception(Bass.BASS_ErrorGetCode().ToString());
        }

        if (BassMix.BASS_Mixer_StreamAddChannel(mixerStream, stream, BASSFlag.BASS_SAMPLE_FLOAT))
        {
            int bufferSize = samplerate * 10 * 4; /*read 10 seconds at each iteration*/
            float[] buffer = new float[bufferSize];
            var chunks = new List<float[]>();
            int size = 0;

            while (true)
            {
                //get re-sampled/mono data
                int bytesRead = Bass.BASS_ChannelGetData(mixerStream, buffer, bufferSize);
                if (bytesRead == 0)
                    break;
                float[] chunk = new float[bytesRead / 4]; //each float contains 4 bytes
                Array.Copy(buffer, chunk, bytesRead / 4);
                chunks.Add(chunk);
                size += bytesRead / 4; //size of the data
            }

            data = new float[size];
            int index = 0;
            /*Concatenate*/
            foreach (float[] chunk in chunks)
            {
                Array.Copy(chunk, 0, data, index, chunk.Length);
                index += chunk.Length;
            }
        }
        else
        {
            throw new Exception(Bass.BASS_ErrorGetCode().ToString());
        }

        Bass.BASS_StreamFree(mixerStream);

        _samples = data;
        return data;
#else
        return null;
#endif
    }

    private static bool IsBassError(string location)
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        var error = Bass.BASS_ErrorGetCode();
        if (error == 0)
        {
            return false;
        }
        else
        {
            Debug.LogError("BASS24 Error in " + location + ", code = " + error.ToString());
            return true;
        }
#else
        return false;
#endif
    }


    public void Dispose()
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        if (MIDISoundSettings.initialized && streamFX != 0)
        {
            ReleaseSyncCallback();

            //Bass.BASS_ChannelRemoveFX(stream, streamFX);
            //IsBassError("BASS_ChannelRemoveFX");
            Bass.BASS_StreamFree(streamFX);
            IsBassError("BASS_StreamFree(streamFX)");
            //Bass.BASS_StreamFree(stream);
            //IsBassError("BASS_StreamFree(stream)");
            streamFX = 0;
        }

        if (memhandle.IsAllocated)
        {
            memhandle.Free();
        }

        loaded = false;
#endif
    }

    void ReleaseSyncCallback()
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        if (syncHandle != 0)
        {
            Bass.BASS_ChannelRemoveSync(streamFX, syncHandle);
            IsBassError("BASS_ChannelRemoveSync");
            syncHandle = 0;
        }
#endif
    }

}
