using System;
using ForieroEngine.MIDIUnified.Interfaces;
using UnityEngine;

#if FMOD
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

    public class AudioSourceFMOD : MonoBehaviour, IAudioSource
    {
        public EventReference eventReference;

        public bool IsPlaying { get => State == AudioSourceState.Playing; }

        private PLAYBACK_STATE _state = PLAYBACK_STATE.STOPPED;

        public AudioSourceState State
        {
            get
            {
                _ei.getPlaybackState(out _state);
                switch (_state)
                {
                    case PLAYBACK_STATE.PLAYING: return AudioSourceState.Playing;
                    case PLAYBACK_STATE.SUSTAINING: return AudioSourceState.Paused;
                    case PLAYBACK_STATE.STOPPED: return AudioSourceState.Stopped;
                    case PLAYBACK_STATE.STARTING: return AudioSourceState.Playing;
                    case PLAYBACK_STATE.STOPPING: return AudioSourceState.Playing;
                }

                return AudioSourceState.Stopped;
            }
        }

        public bool PlayOnAwake { get; set; }

        private int _time = 0;
        public float Time
        {
            get
            {
                _ei.getTimelinePosition(out _time);
                return _time / 1000f;
            }
            set
            {
                _time = (int) (value * 1000f);
                _ei.setTimelinePosition(_time);
            }
        }

        public float Speed { get; set; }
        public int Semitone { get; set; }

        private float _volume = 1;

        public float Volume
        {
            get
            {
                _ei.getVolume(out _volume);
                return _volume;
            }
            set
            {
                _volume = value;
                _ei.setVolume(_volume);
            }
        }

        public bool Mute { get; set; }

        public void Play()
        {
            if(!IsEventInstanceValid()) return;
            _ei.getPaused(out var paused);
            if(paused) Resume();
            else _ei.start();
        }

        public void Pause()
        {
            if(!IsEventInstanceValid()) return;
            _ei.setPaused(true);
        }

        public void Resume()
        {
            if(!IsEventInstanceValid()) return;
            _ei.setPaused(false);
        }

        public void Stop()
        {
            if(!IsEventInstanceValid()) return;
            _ei.stop(STOP_MODE.ALLOWFADEOUT);
        }

        public void Init(AudioClip clip) => throw new System.NotImplementedException();

        public void Init(string clip)
        {
            if (!clip.StartsWith("event:/")) clip = "event:/" + clip;
            if (_ei.handle != IntPtr.Zero) _ei.release();
            if (string.IsNullOrEmpty(clip)) return;
            //this.eventReference = er;
            _ei = FMODUnity.RuntimeManager.CreateInstance(clip);
            Initialized = IsEventInstanceValid();
        }

        public bool Initialized { get; set; }
        public void PlayOneShot(AudioClip clip)
        {
            throw new NotImplementedException();
        }

        public void PlayOneShot(AudioClip clip, float volume)
        {
            throw new NotImplementedException();
        }

        public void PlayOneShot(EventReference clip, float volume) => FMODUnity.RuntimeManager.PlayOneShot(clip);
        public void PlayOneShot(string clip, float volume) => FMODUnity.RuntimeManager.PlayOneShot(clip);

        public void Release()
        {
            if (_ei.handle != IntPtr.Zero) _ei.release();
        }

        public void Init(EventReference er)
        {
            if (_ei.handle != IntPtr.Zero) _ei.release();
            if (er.IsNull) return;
            this.eventReference = er;
            _ei = FMODUnity.RuntimeManager.CreateInstance(er);
            Initialized = IsEventInstanceValid();
        }

        bool IsEventInstanceValid()
        {
            if (!_ei.isValid() && !eventReference.IsNull)
            {
                #if UNITY_EDITOR
                Debug.LogWarning($"FMOD | EventInstance is invalid : {eventReference.Guid} {eventReference.Path}");
                #endif
            }
            return _ei.isValid();
        }

        private EventInstance _ei;

        void Awake()
        {
            Init(this.eventReference);
        }
    }
#else
    public class AudioSourceFMOD : MonoBehaviour, IAudioSource
    {
        public string clipId;
        
        public bool IsPlaying { get; }
        public AudioSourceState State { get; set; }
        public bool PlayOnAwake { get; set; }
        public float Time { get; set; }
        public float Speed { get; set; }
        public int Semitone { get; set; }
        public float Volume { get; set; }
        public bool Mute { get; set; }
        public void Play() { throw new NotImplementedException() ; }
        public void Pause() { throw new NotImplementedException(); }
        public void Resume() { throw new NotImplementedException(); }
        public void Stop() { throw new NotImplementedException(); }
        public void Init(AudioClip clip) { throw new NotImplementedException(); }
        public void Init(string clip) { throw new NotImplementedException(); }
        public bool Initialized { get; set; }
        public void PlayOneShot(AudioClip clip) { throw new NotImplementedException(); }
        public void PlayOneShot(AudioClip clip, float volume)
        {
            throw new NotImplementedException();
        }

        public void PlayOneShot(string clip, float volume) { throw new NotImplementedException(); }
        public void Release() { throw new NotImplementedException(); }
    }
#endif
