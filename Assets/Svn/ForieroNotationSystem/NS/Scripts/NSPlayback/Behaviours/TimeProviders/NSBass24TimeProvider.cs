/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified.Plugins;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

[RequireComponent(typeof(AudioSourceBass24))]
public class NSBass24TimeProvider : MonoBehaviour, ITimeProvider, IAudioProvider
{
    public string id = "";
    public string Id => id;
    
    public float GetTime() => _audioSource.time;
    public void SetTime(float value) => _audioSource.time = value;

    public void EnableAudioProvider() { this.gameObject.SetActive(true);}
    public void DisableAudioProvider() { this.gameObject.SetActive(false); }
    public void InitAudioClip(AudioClip clip) { _audioSource.Stop(); _audioSource.clip = clip; }
    public void EnableTimeProvider() { }
    public void DisableTimeProvider() { }

    private AudioSourceBass24 _audioSource;
    
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

    private void OnEnable() { SubscribeEvents(); }
    private void OnDisable() { UnsubscribeEvents(); }

    private void Awake()
    {
        TimeProviders.Register(this);
        AudioProviders.Register(this);
        this.gameObject.SetActive(false);
    }
    private void Start() { _audioSource = GetComponent<AudioSourceBass24>(); }

    private void OnDestroy()
    {
        UnsubscribeEvents();
        TimeProviders.Unregister(this);
        AudioProviders.Unregister(this);
    }
    private void OnSpeedChanged(double speed) {  if(_audioSource) _audioSource.speed = (float)speed; }
    private void OnSemitoneChanged(int semitone) { if(_audioSource) _audioSource.semitone = semitone; }
    private void OnPlaybackStateChanged(NSPlayback.PlaybackState playbackState)
    {        
        switch (NSPlayback.playbackState)
        {
            case NSPlayback.PlaybackState.Stop: { if(_audioSource) _audioSource.Stop(); break; }
            case NSPlayback.PlaybackState.Playing: { if(_audioSource) _audioSource.Play(); break; }
            case NSPlayback.PlaybackState.WaitingForInput:
            case NSPlayback.PlaybackState.Pausing: { if(_audioSource) _audioSource.Pause(); break; }
        }        
    }    
}
