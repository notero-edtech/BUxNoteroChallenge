/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

[RequireComponent(typeof(AudioSource))]
public class NSAudioSourceTimeProvider : MonoBehaviour, ITimeProvider, IAudioProvider
{     
    public string id = "";
    public string Id => id;
   
    public AudioMixer mixer;
    
    private AudioSource _audioSource;
    private AudioClip _clip;

    public float Samples => _clip ? _clip.length * _clip.frequency * _clip.channels : 0;
    public float GetTime() => _clip ? (float)_audioSource.timeSamples / (float)_clip.frequency : 0;
    public void SetTime(float value) { _audioSource.time = value; }

    public void EnableAudioProvider() { this.gameObject.SetActive(true); }
    public void DisableAudioProvider() { this.gameObject.SetActive(false); }
    public void InitAudioClip(AudioClip clip)
    {
        var muted = _audioSource.mute;
        _audioSource.mute = true;
        _audioSource.Stop(); 
        _audioSource.clip = _clip = clip;
        _finished = false;
        _audioSource.Stop(); 
        _audioSource.Play();
        _audioSource.Pause();
        _audioSource.time = 0;
        _audioSource.mute = muted;        
    }
    public void EnableTimeProvider() { }
    public void DisableTimeProvider() { }

    private void SubscribeEvents()
    {
        NSPlayback.OnPlaybackStateChanged += OnPlaybackStateChanged;
        NSPlayback.OnSpeedChanged += OnSpeedChanged;
        NSPlayback.OnSemitoneChanged += OnSemitoneChanged;
    }
        
    private void UnsubscribeEvents()
    {
        NSPlayback.OnPlaybackStateChanged -= OnPlaybackStateChanged;
        NSPlayback.OnSpeedChanged -= OnSpeedChanged;
        NSPlayback.OnSemitoneChanged -= OnSemitoneChanged;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        TimeProviders.Register(this);
        AudioProviders.Register(this);
        this.gameObject.SetActive(false);

        // @TODO: I am expecting Marek to fix this himself.
        _audioSource.outputAudioMixerGroup = MIDIMixerSettings.instance.accompaniment;
        mixer = MIDIMixerSettings.instance.accompaniment.audioMixer;
    }

    private void OnEnable() { SubscribeEvents(); }
    private void OnDisable() { UnsubscribeEvents(); }

    private void Start() { OnSpeedChanged(NSPlayback.speed); }

    private void OnDestroy()
    {
        UnsubscribeEvents();
        TimeProviders.Unregister(this);
        AudioProviders.Unregister(this);
    }

    private bool _finished = false;
    private void Update()
    {
        if (_audioSource.time == 0 && _audioSource.timeSamples > 0 && !_audioSource.isPlaying) { _finished = true; }
    }
    
    private void OnSpeedChanged(double speed)
    {
        if (!_audioSource) return;
        _audioSource.pitch = (float)speed;
        _audioSource.mute = speed is < 0.5 or > 2;
        mixer.SetFloat("AccompanimentPitch", 1f / (float)speed);
    }
    private void OnSemitoneChanged(int semitone) { /*audioSource.semitone = semitone;*/}
    private void OnPlaybackStateChanged(NSPlayback.PlaybackState playbackState)
    {      
        switch (NSPlayback.playbackState)
        {
            case NSPlayback.PlaybackState.Stop: { StopAudio(); break; }
            case NSPlayback.PlaybackState.Playing: { PlayAudio(); break; }
            case NSPlayback.PlaybackState.WaitingForInput:
            case NSPlayback.PlaybackState.Pausing: { PauseAudio(); break; }
            case NSPlayback.PlaybackState.Finished: { break; }
        }        
    }

    private TweenerCore<float, float, FloatOptions> _t = null;
    private void PlayAudio() {
        _t?.Kill(false);
        _t =_audioSource.DOFade(1f, NSPlayback.Interaction.waitForInputFadeOutTime);
        if (_audioSource.isPlaying) return;
        if(_finished) _audioSource.Play(); else _audioSource.UnPause();
    }

    private void StopAudio()
    {
        _t?.Kill(false);
        _t =_audioSource.DOFade(0, NSPlayback.Interaction.waitForInputFadeOutTime).OnComplete(() =>
        {
            var muted = _audioSource.mute;
            _audioSource.mute = true;
            _audioSource.Stop();
            _audioSource.Play();
            _audioSource.Pause();
            _audioSource.time = 0;
            _audioSource.mute = muted;
        });
    }

    private void PauseAudio()
    {
        _t?.Kill(false);
        _t =_audioSource.DOFade(0, NSPlayback.Interaction.waitForInputFadeOutTime).OnComplete(() =>
        {
            _audioSource.Pause();
            NSPlayback.Time.iTimeProvider.SetTime(NSPlayback.Time.time);
        });
    }
}
