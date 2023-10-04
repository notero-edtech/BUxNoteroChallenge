using System.Collections.Generic;
using ForieroEngine.Pooling;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public partial class MIDIPercussion : MonoBehaviour
    {
        public static MIDIPercussion instance;
        private static MIDIPercussionSettings Settings => MIDIPercussionSettings.instance;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = MIDIMixerSettings.instance.metronome;
            _audioSource.playOnAwake = false;
            _audioSource.bypassEffects = true;
            _audioSource.bypassListenerEffects = true;
            _audioSource.bypassReverbZones = true;
            _audioSource.priority = 0;
            InitAudioSourcePool();
        }

        private void OnEnable()
        {
            if (instance)
            {
                Debug.LogError("Something is trying to add MIDIPercussion into scene but it already exists!");
                DestroyImmediate(this.gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void OnDestroy() { PercussionDestroy(); }
        private void Update() { UpdateAudioSourcePool(); }

        private AudioObject _audioObject;
        private ScheduledPercussion _scheduledPercussion;
        private AudioClip _percussionAudioClip;

        private struct ScheduledPercussion
        {
            public AudioClip clip;
            public float volume;
            public double dspTime;
        }

        private ObjectPool<AudioObject> _audioObjectPool = null;
        private readonly List<ScheduledPercussion> _scheduledPercussionItems = new ();
        private readonly List<AudioObject> _playingPercussionItems = new ();
        private readonly List<AudioObject> _percussionItems = new ();

        private void InitAudioSourcePool()
        {
            if (_audioObjectPool != null) return;
            
            _audioObjectPool = new ObjectPool<AudioObject>(() => new AudioObject(), MIDIPercussionSettings.instance.maxScheduledUnits);
            
            while (_audioObjectPool.AvailableItems > 0)
            {
                _audioObject = _audioObjectPool.GetItem();
                if (_audioObject.audioSource) continue;
                _audioObject.audioSource = gameObject.AddComponent<AudioSource>();
                _audioObject.audioSource.outputAudioMixerGroup = MIDIMixerSettings.instance.metronome;
                _audioObject.audioSource.playOnAwake = false;
                _audioObject.audioSource.bypassEffects = true;
                _audioObject.audioSource.bypassListenerEffects = true;
                _audioObject.audioSource.bypassReverbZones = true;
                _audioObject.audioSource.priority = 0;
                _percussionItems.Add(_audioObject);
            }

            _audioObjectPool.ReleaseAll();
        }

        private void UpdateAudioSourcePool()
        {
            for (var i = _playingPercussionItems.Count - 1; i >= 0; i--)
            {
                if (_playingPercussionItems[i].IsPlaying()) continue;
                _audioObjectPool.ReleaseItem(_playingPercussionItems[i]);
                _playingPercussionItems.RemoveAt(i);
            }

            for (var i = _scheduledPercussionItems.Count - 1; i >= 0; i--)
            {
                _scheduledPercussion = _scheduledPercussionItems[i];
                if (_audioObjectPool.AvailableItems > 0)
                {
                    _audioObject = _audioObjectPool.GetItem();
                    _audioObject.Schedule(_scheduledPercussion);
                    _playingPercussionItems.Add(_audioObject);
                    _playingPercussionItems.Sort((a, b) => -1 * a.dspTime.CompareTo(b.dspTime));
                    _scheduledPercussionItems.RemoveAt(i);
                }
                else { break; }
            }
        }

        private static int Percussion(PercussionEnum percussionEnum, int volume)
        {
            if (!instance) return 0;
            if (!System.Enum.IsDefined(typeof(PercussionEnum), percussionEnum)) return 0;
            instance._percussionAudioClip = Settings.GetAudioClip(percussionEnum);
            if (!instance._percussionAudioClip) return 0;
            instance._audioSource.PlayOneShot(instance._percussionAudioClip, volume.ToVolume());
            return 1;
        }

        public static void Mute() { 
            if (!instance) return; 
            instance._audioSource.mute = true;
            instance._percussionItems.ForEach(p => p.audioSource.mute = true);
        }

        public static void UnMute()
        {
            if (!instance) return; 
            instance._audioSource.mute = false;
            instance._percussionItems.ForEach(p => p.audioSource.mute = false);
        }

        public static double SchedulePercussion(PercussionEnum percussionEnum, int volume, double scheduleTime = 0, bool absoluteDspTime = false)
        {
            if (!instance) return -1;
            if (!System.Enum.IsDefined(typeof(PercussionEnum), percussionEnum)) return -1;
            
            if (scheduleTime <= 0) { Percussion(percussionEnum, volume); return 0; }
            instance._percussionAudioClip = Settings.GetAudioClip(percussionEnum);
            if (!instance._percussionAudioClip) return -1;
            
            var item = new ScheduledPercussion();
            item.clip = instance._percussionAudioClip;
            item.volume = volume.ToVolume();
            if (absoluteDspTime) { item.dspTime = scheduleTime; }
            else { item.dspTime = AudioSettings.dspTime + scheduleTime; }
            instance._scheduledPercussionItems.Add(item);
            instance._scheduledPercussionItems.Sort((a, b) => -1 * a.dspTime.CompareTo(b.dspTime));
            return item.dspTime;
        }

        public static void CancelScheduledPercussion()
        {
            if (!instance) return;
            instance._scheduledPercussionItems?.Clear();
            for (var i = 0; i < instance._playingPercussionItems.Count; i++) { instance._playingPercussionItems[i].audioSource.Stop(); }
            instance._playingPercussionItems?.Clear();
            instance._audioObjectPool?.ReleaseAll();
        }

        private void PercussionDestroy()
        {
            CancelScheduledPercussion();
            _audioObjectPool = null;
        }

        private class AudioObject
        {
            public AudioSource audioSource;
            public double dspTime;
            public bool IsPlaying() => audioSource != null && audioSource.isPlaying;
            public void Schedule(ScheduledPercussion scheduledPercussion)
            {
                this.dspTime = scheduledPercussion.dspTime;

                if (!audioSource) return;
                
                audioSource.clip = scheduledPercussion.clip;
                audioSource.volume = scheduledPercussion.volume;
                audioSource.PlayScheduled(scheduledPercussion.dspTime);
            }
        }
    }
}