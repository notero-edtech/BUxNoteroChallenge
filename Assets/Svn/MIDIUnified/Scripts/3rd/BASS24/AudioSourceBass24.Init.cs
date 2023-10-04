using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
#endif

public partial class AudioSourceBass24 : MonoBehaviour
{
    public void Init(AudioClip c, bool decoding = false)
    {
        if (c == _clip) return;

        _clip = clip = c;

        _clipBass24?.Dispose();
        _clipBass24 = null;

        if(c) _clipBass24 = AudioClipBass24.Create(c, decoding);
    }

    void SetSyncCallback()
    {
        //if (syncProc == null)
        //{
        //    syncProc = new SYNCPROC(SyncCallback);
        //}

        //syncHandle = Bass.BASS_ChannelSetSync(streamFX, BASSSync.BASS_SYNC_END, 0, syncProc, IntPtr.Zero);
        //if (IsBassError("BASS_ChannelSetSync"))
        //{
        //    syncHandle = 0;
        //    return;
        //}

        //if (loop)
        //{
        //    Bass.BASS_ChannelFlags(stream, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
        //    if (IsBassError("BASS_ChannelFlags"))
        //    {
        //        return;
        //    }
        //}
    }
}