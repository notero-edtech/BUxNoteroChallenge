using System;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;

#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
using Un4seen.Bass;
#endif

public partial class AudioSourceBass24 : MonoBehaviour
{
	//private SYNCPROC syncProc = null;

	public void Play()
	{
		if (!MIDISoundSettings.initialized)
		{
			Debug.LogError("BASS24 not initialized!!!");
			return;
		}

        Init(clip);

        if (this.clipBass24 == null)
		{
			Debug.LogError("AudioClip is null");
			return;
		}

		if (clipBass24 == null || this.clipBass24 == null || this.clipBass24.streamFX == 0)
		{
			return;
		}

		switch (state)
		{
			case AudioSourceState.Finished:
			case AudioSourceState.Stopped:
                if (!this.clipBass24.decoding)
                {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
					Bass.BASS_ChannelPlay(this.clipBass24.streamFX, true);
#endif
                    IsBassError("BASS_ChannelPlay");
                }
				this.state = AudioSourceState.Playing;
				break;
			case AudioSourceState.Paused:
                if (!this.clipBass24.decoding)
                {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
					Bass.BASS_ChannelPlay(this.clipBass24.streamFX, false);
#endif
                    IsBassError("BASS_ChannelPlay");
                }
				this.state = AudioSourceState.Playing;
				break;
			case AudioSourceState.Playing:
				break;
		}
	}

	public void Pause()
	{
		if (state == AudioSourceState.Playing)
		{
			if (MIDISoundSettings.initialized && this.clipBass24 != null && this.clipBass24.streamFX != 0)
			{
                if (!this.clipBass24.decoding)
                {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
					Bass.BASS_ChannelPause(this.clipBass24.streamFX);
#endif
                    IsBassError("BASS_ChannelPause");
                }
			}
			this.state = AudioSourceState.Paused;
		}
	}

	public void Unpause()
	{
		if (this.state == AudioSourceState.Paused)
		{
			this.Play();
		}
	}

	public void Stop()
	{
		if (IsPlaying || state == AudioSourceState.Paused)
		{
			if (MIDISoundSettings.initialized && this.clipBass24 != null && this.clipBass24.streamFX != 0)
			{
                if (!this.clipBass24.decoding)
                {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
					Bass.BASS_ChannelStop(this.clipBass24.streamFX);
#endif
                    IsBassError("BASS_ChannelStop");
                }
			}
			this.state = AudioSourceState.Stopped;
		}
	}

	private float GetPercentChange(float prevValue, float curValue)
	{
		if (Math.Abs(prevValue - curValue) < 0.01f)
		{
			return 0;
		}

		return 100 * (curValue > prevValue ? (curValue / prevValue) - 1 : (1.0f - (curValue / prevValue)) * -1.0f);
	}

	private bool IsBassError(string location)
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

	public void SetTime(float value)
	{
		if (MIDISoundSettings.initialized && this.clipBass24 != null && this.clipBass24.streamFX != 0)
		{
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
			Bass.BASS_ChannelSetPosition(this.clipBass24.streamFX, value);
#endif
			IsBassError("BASS_ChannelSetPosition");			
		}
	}
}