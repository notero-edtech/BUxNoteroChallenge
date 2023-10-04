using System;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;

public partial class AudioSourceBass24 : MonoBehaviour
{
    #region Interface

    public bool IsPlaying => this.state == AudioSourceState.Playing;
    public AudioSourceState State => GetState();
    public float Time { get => time; set => time = value; }
    public float Speed { get => speed; set => speed = value; }
    public int Semitone { get => semitone; set => semitone = value; }
    public float Volume { get => volume; set => volume = value; }
    public void Resume() => Play();

    public bool Mute { get => mute; set => mute = value; }

    public void Init(AudioClip clip)
    {
        _clipBass24?.Dispose();
        this.clip = this._clip = clip;
        if(clip) _clipBass24 = AudioClipBass24.Create(clip);
    }

    public void Init(string clip)
    {
        throw new NotImplementedException();
    }
    
    AudioSourceState GetState()
    {
        if (IsPlaying) return AudioSourceState.Playing;
        else if (time > 0f && time < clip.length) return AudioSourceState.Paused;
        else return AudioSourceState.Finished;
    }

    #endregion
}