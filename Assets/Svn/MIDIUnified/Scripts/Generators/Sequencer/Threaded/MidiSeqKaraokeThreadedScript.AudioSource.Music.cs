using UnityEngine;

public partial class MidiSeqKaraokeThreadedScript : MonoBehaviour
{
    bool CheckMusic()
    {
        if (audioInput == AudioInput.Unity)
        {
            return music && audioMusic;
        }
        else if (audioInput == AudioInput.BASS24)
        {
            return music && audioMusicBass24;
        }
        else
        {
            return false;
        }
    }

    StateAudioSource GetMusicState()
    {
        if (!CheckMusic()) return StateAudioSource.None;

        if (audioInput == AudioInput.Unity)
        {
            if (audioMusic.isPlaying)
            {
                return StateAudioSource.Playing;
            }
            else if (audioMusic.time > 0f && audioMusic.time < musicClip.length)
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
            if (audioMusicBass24.IsPlaying)
            {
                return StateAudioSource.Playing;
            }
            else if (audioMusicBass24.time > 0f && audioMusicBass24.time < musicClip.length)
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

    void PlayMusic()
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.Play();
                break;
            case AudioInput.BASS24:
                audioMusicBass24.Play();
                break;
        }
    }

    void StopMusic()
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.Stop();
                break;
            case AudioInput.BASS24:
                audioMusicBass24.Stop();
                break;
        }
    }

    void PauseMusic()
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.Pause();
                break;
            case AudioInput.BASS24:
                audioMusicBass24.Pause();
                break;
        }
    }

    double GetMusicTime()
    {
        if (!CheckMusic()) return 0;

        if (audioInput == AudioInput.Unity)
        {
            return audioMusic.time;
        }
        else if (audioInput == AudioInput.BASS24)
        {
            return audioMusicBass24.time;
        }
        else
        {
            return 0;
        }
    }


    void SetMusicTime(double time)
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.time = (float)time;
                break;
            case AudioInput.BASS24:
                audioMusicBass24.time = (float)time;
                break;
        }
    }

    void MuteMusic()
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.mute = true;
                break;
            case AudioInput.BASS24:
                audioMusicBass24.mute = true;
                break;
        }
    }

    void UnMuteMusic()
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.mute = false;
                break;
            case AudioInput.BASS24:
                audioMusicBass24.mute = false;
                break;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.volume = volume;
                break;
            case AudioInput.BASS24:
                audioMusicBass24.volume = volume;
                break;
        }
    }

    public float GetMusicVolume()
    {
        if (!CheckMusic()) return 0;

        if (audioInput == AudioInput.Unity)
        {
            return audioMusic.volume;
        }
        else if (audioInput == AudioInput.BASS24)
        {
            return audioMusicBass24.volume;
        }
        else
        {
            return 0;
        }
    }

    void SetMusicClip(AudioClip clip)
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.clip = clip;
                break;
            case AudioInput.BASS24:
                audioMusicBass24.Init(clip);
                break;
        }
    }

    void SetMusicSemitone(int semitone)
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                Debug.LogError("Not implemented SetMusicSemitone!");
                break;
            case AudioInput.BASS24:
                audioMusicBass24.semitone = semitone;
                break;
        }
    }

    void SetMusicSpeed(float speed)
    {
        if (!CheckMusic()) return;

        switch (audioInput)
        {
            case AudioInput.Unity:
                audioMusic.pitch = speed;
                break;
            case AudioInput.BASS24:
                audioMusicBass24.speed = speed;
                break;
        }
    }
}
