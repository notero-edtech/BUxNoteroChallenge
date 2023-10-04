using System;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;
using UnityEngine.Audio;

#if FMOD
using FMODUnity;
#endif

[RequireComponent(typeof(AudioSource))]
public class AudioSourceUnity : MonoBehaviour, IAudioSource
{
    public bool IsPlaying => audioSource.isPlaying;
    public AudioSourceState State => GetState();
    public bool PlayOnAwake { get => audioSource.playOnAwake; set => audioSource.playOnAwake = value; }
    public float Time { get => audioSource.time; set => audioSource.time = value; }
    public float Speed { get => audioSource.pitch; set => audioSource.pitch = value; }
    public int Semitone { get; set; }
    public float Volume { get => audioSource.volume; set => audioSource.volume = value; }
    public void Play() => audioSource.Play();
    public void Pause() => audioSource.Pause();
    public void Resume() => audioSource.Play();
    public void Stop() => audioSource.Stop();
    public void Init(AudioClip clip)
    {
        audioSource.clip = clip;
        Initialized = clip;
    }

    public AudioMixerGroup OutputAudioMixerGroup
    {
        get => audioSource.outputAudioMixerGroup;
        set => audioSource.outputAudioMixerGroup = value;
    }

#if FMOD
    public void Init(EventReference clip)=> throw new NotImplementedException();
#endif

    public void Init(string clip) => throw new NotImplementedException();
    public bool Initialized { get; set; }
    public void PlayOneShot(AudioClip clip) => audioSource.PlayOneShot(clip);
    public void PlayOneShot(AudioClip clip, float volume) => audioSource.PlayOneShot(clip, volume);
    
#if FMOD
    public void PlayOneShot(EventReference clip, float volume) { throw new NotImplementedException(); }
#endif

    public void PlayOneShot(string clip, float volume) { throw new NotImplementedException(); }

    public void Release()
    {
        if (audioSource.clip)
        {
            var clip = audioSource.clip;
            audioSource.clip = null;
            clip.UnloadAudioData();
        }
    }

    public bool Mute { get => audioSource.mute; set => audioSource.mute = value; }
    public AudioSource audioSource;
    void Awake() { if (!audioSource) audioSource = GetComponent<AudioSource>(); }
    AudioSourceState GetState()
    {
        if (audioSource == null) return AudioSourceState.Stopped;
        if (IsPlaying) return AudioSourceState.Playing;
        else if (audioSource.time > 0f && audioSource.time < audioSource.clip.length) return AudioSourceState.Paused;
        else return AudioSourceState.Finished;
    }
}
