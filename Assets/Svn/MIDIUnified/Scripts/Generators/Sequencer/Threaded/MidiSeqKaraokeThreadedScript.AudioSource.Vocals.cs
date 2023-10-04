using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour
{
    bool CheckVocals()
    {
        if (audioInput == AudioInput.Unity)
        {
            return vocals && audioVocals;
        }
        else if (audioInput == AudioInput.BASS24)
        {
            return vocals && audioVocalsBass24;
        }
        else
        {
            return false;
        }
    }

    StateAudioSource GetVocalsState()
    {
        if (!CheckVocals()) return StateAudioSource.None;

        if (audioInput == AudioInput.Unity)
        {
            if (audioVocals.isPlaying)
            {
                return StateAudioSource.Playing;
            }
            else if (audioVocals.time > 0f && audioVocals.time < vocalsClip.length)
            {
                return StateAudioSource.Paused;
            }
            else
            {
                return StateAudioSource.Finished;
            }
        }
        else if (audioInput == AudioInput.BASS24)
        {
            if (audioVocalsBass24.IsPlaying)
            {
                return StateAudioSource.Playing;
            }
            else if (audioVocalsBass24.time > 0f && audioVocalsBass24.time < vocalsClip.length)
            {
                return StateAudioSource.Paused;
            }
            else
            {
                return StateAudioSource.Finished;
            }
        }
        else
        {
            return StateAudioSource.None;
        }
    }

    void PlayVocals()
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.Play();
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.Play();
                break;
        }
    }

    void StopVocals()
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.Stop();
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.Stop();
                break;
        }
    }

    void PauseVocals()
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.Pause();
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.Pause();
                break;
        }
    }

    double GetVocalsTime()
    {
        if (!CheckVocals()) return 0;

        if (audioInput == AudioInput.Unity)
        {
            return audioVocals.time;
        }
        else if (audioInput == AudioInput.BASS24)
        {
            return audioVocalsBass24.time;
        }
        else
        {
            return 0;
        }
    }

    void SetVocalsTime(double time)
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.time = (float)time;
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.time = (float)time;
                break;
        }
    }

    void MuteVocals()
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.mute = true;
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.mute = true;
                break;
        }
    }

    void UnMuteVocals()
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.mute = false;
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.mute = false;
                break;
        }
    }

    public void SetVocalsVolume(float volume)
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.volume = volume;
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.volume = volume;
                break;
        }
    }

    public float GetVocalVolume()
    {
        if (!CheckVocals()) return 0;

        if (audioInput == AudioInput.Unity)
        {
            return audioVocals.volume;
        }
        else if (audioInput == AudioInput.BASS24)
        {
            return audioVocalsBass24.volume;
        }
        else
        {
            return 0;
        }
    }

    void SetVocalsClip(AudioClip clip)
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.clip = clip;
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.Init(clip);
                break;
        }
    }

    void SetVocalsSemitone(int semitone)
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                Debug.LogError("Not implemented SetVocalsSemitone!");
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.semitone = semitone;
                break;
        }
    }

    void SetVocalsSpeed(float speed)
    {
        if (!CheckVocals()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioVocals.pitch = speed;
                break;
            case AudioInput.BASS24:
                audioVocalsBass24.speed = speed;
                break;
        }
    }
}
