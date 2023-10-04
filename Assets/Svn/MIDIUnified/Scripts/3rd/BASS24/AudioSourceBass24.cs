using System;
using System.Collections;
using ForieroEngine.MIDIUnified;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

using ForieroEngine.MIDIUnified.Interfaces;
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
using Un4seen.Bass;
#endif

#if FMOD
using FMODUnity;
#endif

public partial class AudioSourceBass24 : MonoBehaviour, IAudioSource
{
    private AudioClipBass24 _clipBass24;

    public AudioClipBass24 clipBass24 => _clipBass24;

    private AudioClip _clip;
    public AudioClip clip;
    public bool loop;
    public bool playOnAwake;

    [Range(0.1f, 10)] public volatile float speed = 1;
    [Range(-12, 12)] public volatile int semitone = 0;
    [Range(-1, 1)] public volatile float panStereo = 0;
    [Range(0, 1)] public float volume = 1;

    volatile float _time = 0;
    public float time { get => _time; set => SetTime(value); }

    private volatile bool _terminate = false;
    private volatile bool _running = false;
    private Task _timeTask = null;

    public volatile AudioSourceState state = AudioSourceState.Stopped;

    public bool mute;

    private void Awake()
    {
        state = AudioSourceState.Stopped;
    }

    private void InitThreads()
    {
        if (_running) return;
        _timeTask = Task.Run(() =>
        {
            _running = true;
            _terminate = false;

            while (!_terminate)
            {
                if (!MIDISoundSettings.initialized || clipBass24 == null || clipBass24.streamFX == 0)
                {
                    time = 0;
                    Thread.Sleep(1);
                    continue;
                }
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
                try
                {
                    var pos = Bass.BASS_ChannelGetPosition(clipBass24.streamFX, BASSMode.BASS_POS_BYTES);
                    IsBassError("BASS_ChannelGetPosition");
                    _time = (float)Bass.BASS_ChannelBytes2Seconds(clipBass24.streamFX, pos);
                }
                catch (Exception e) { Debug.LogError(e.Message); }
                Thread.Sleep(1);
#endif
            }

            _running = false;
        });
    }

    private void TerminateThreads(bool waitToJoin = true)
    {
        _terminate = true;

        if (waitToJoin && _timeTask != null) _timeTask.Wait();

        _timeTask = null;
    }

    private void OnEnable() => InitThreads();
    private void OnDisable() => TerminateThreads();
    private void OnDestroy() => TerminateThreads();
    
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MIDI.initialized);
        Init(clip);

        InitThreads();

        if (playOnAwake) this.Play();
    }

    private int _lastSemiton = 0;
    private float _lastSpeedVal = 100f;
    private float _lastPanStereo = 0f;
    private float _lastVolume = 0;

    // Update is called once per frame
    private void Update()
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        if (!MIDISoundSettings.initialized || clipBass24 == null || clipBass24.streamFX == 0)
        {
            return;
        }

        if (state == AudioSourceState.Playing)
        {
            var targetVal = this.mute ? 0 : this.volume;
            if (System.Math.Abs(targetVal - _lastVolume) > 0.01f)
            {
                Bass.BASS_ChannelSetAttribute(clipBass24.streamFX, BASSAttribute.BASS_ATTRIB_VOL, targetVal);
                IsBassError("BASS_ChannelSetAttribute");
                _lastVolume = targetVal;
            }

            float speedVal = GetPercentChange(1, this.speed);
            if (speedVal < -90) // bass min limit
            {
                speedVal = -90;
            }

            if ((int)_lastSpeedVal != (int)speedVal)
            {
                Bass.BASS_ChannelSetAttribute(clipBass24.streamFX, BASSAttribute.BASS_ATTRIB_TEMPO, speedVal);
                IsBassError("BASS_ChannelSetAttribute");
                _lastSpeedVal = speedVal;
            }

            if (_lastSemiton != this.semitone)
            {
                Bass.BASS_ChannelSetAttribute(clipBass24.streamFX, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, this.semitone);
                IsBassError("BASS_ChannelSetAttribute");
                _lastSemiton = this.semitone;
            }

            if (System.Math.Abs(_lastPanStereo - this.panStereo) > 0.001f)
            {
                Bass.BASS_ChannelSetAttribute(clipBass24.streamFX, BASSAttribute.BASS_ATTRIB_PAN, this.panStereo);
                IsBassError("BASS_ChannelSetAttribute");
                _lastPanStereo = this.panStereo;
            }
            
            if (time >= clipBass24.length)
            {
                state = AudioSourceState.Finished;
                if (loop)
                {
                    Play();
                }
            }
        }
#endif
    }

    private void OnApplicationQuit()
    {
        if (this.state != AudioSourceState.Stopped) { this.Stop(); }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
#if ((UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE) && !BASS24_DISABLED) || UNITY_EDITOR 
        if (state == AudioSourceState.Playing)
        {
            if (pauseStatus)
            {
                if (MIDISoundSettings.initialized && this.clipBass24 != null && clipBass24.streamFX != 0) Bass.BASS_ChannelPause(clipBass24.streamFX);
            }
            else
            {
                if (MIDISoundSettings.initialized && this.clipBass24 != null && clipBass24.streamFX != 0) Bass.BASS_ChannelPlay(clipBass24.streamFX, false);
            }
        }
#endif
    }

   private void SyncCallback(int handle, int channel, int data, IntPtr user)
    {
        Debug.Log("SyncCallback : FINISSHED");
        if (!loop) { state = AudioSourceState.Finished; }
    }

    
    public bool PlayOnAwake { get; set; }
#if FMOD
    public void Init(EventReference clip)=> throw new NotImplementedException();
#endif
    public bool Initialized { get; set; }
    public void PlayOneShot(AudioClip clip) { throw new NotImplementedException(); }

    public void PlayOneShot(AudioClip clip, float volume) { throw new NotImplementedException(); }
#if FMOD
    public void PlayOneShot(EventReference clip, float volume){ throw new NotImplementedException(); }
#endif

    public void PlayOneShot(string clip, float volume) { throw new NotImplementedException(); }
    public void Release() { throw new NotImplementedException(); }
}