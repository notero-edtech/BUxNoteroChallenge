using Notero.Unity.AudioModule.Core;
using Notero.Unity.AudioModule.Settings;
using System;
using UnityEngine;
using UnityEngine.Audio;
using AudioSettings = Notero.Unity.AudioModule.Settings.AudioSettings;
using Math = Notero.Unity.AudioModule.Utilities.Math;

namespace Notero.Unity.AudioModule
{
    public class AudioPlayer : MonoBehaviour
    {
        public static AudioPlayer Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    Cleanup();
                    var audioPlayerPrefab = Resources.Load<AudioPlayer>("AudioPlayer");
                    var audioPlayerObject = Instantiate(audioPlayerPrefab);
                    m_Instance = audioPlayerObject.GetComponent<AudioPlayer>();
                    m_Instance.Init();
                    m_Instance.name = "AudioPlayer";
                    DontDestroyOnLoad(m_Instance);
                }

                return m_Instance;
            }
        }
        private static AudioPlayer m_Instance;

        private static void Cleanup()
        {
            var audioPlayers = FindObjectsOfType<AudioPlayer>();
            foreach(var audioPlayer in audioPlayers)
            {
                if(audioPlayer != null)
                {
                    Destroy(audioPlayer.gameObject);
                }
            }
        }

        public float MasterVolume
        {
            get => m_Settings.MasterVolume;
            set
            {
                m_Settings.MasterVolume = value;
                m_MasterMixer.audioMixer.SetFloat("masterVolume", Math.NormalToDecibel(value));
            }
        }
        public float BGMVolume
        {
            get => m_Settings.BGMVolume;
            set
            {
                m_Settings.BGMVolume = value;
                m_MusicMixer.audioMixer.SetFloat("bgmVolume", Math.NormalToDecibel(value));
            }
        }
        public float SFXVolume
        {
            get => m_Settings.SFXVolume;
            set
            {
                m_Settings.SFXVolume = value;
                m_SoundEffectMixer.audioMixer.SetFloat("sfxVolume", Math.NormalToDecibel(value));
            }
        }

        public float MidiPlaybackVolume
        {
            get => m_Settings.MidiPlaybackVolume;
            set
            {
                m_Settings.MidiPlaybackVolume = value;
                m_MidiPlaybackMixer.audioMixer.SetFloat("midiPlaybackVolume", Math.NormalToDecibel(value));
            }
        }

        [SerializeField]
        private AudioMixerGroup m_MasterMixer;
        [SerializeField]
        private AudioMixerGroup m_MusicMixer;
        [SerializeField]
        private AudioMixerGroup m_SoundEffectMixer;
        [SerializeField]
        private AudioMixerGroup m_MetronomeMixer;
        [SerializeField]
        private AudioMixerGroup m_MidiPlaybackMixer;

        private AudioSpeaker m_MusicAudioSpeaker;
        private AudioSpeaker m_MetronomeAudioSpeaker;
        private AudioSpeakerCollection m_SoundEffectAudioSpeakers;
        private AudioSpeakerCollection m_CustomAudioSpeakers;

        private AudioSettings m_Settings;

        public void SetMasterVolume(float value)
        {
            MasterVolume = value;
        }

        public void SetBGMVolume(float value)
        {
            BGMVolume = value;
        }

        public void SetSFXVolume(float value)
        {
            SFXVolume = value;
        }

        public void SetMidiPlaybackVolume(float value)
        {
            MidiPlaybackVolume = value;
        }

        public void ResetSettings()
        {
            m_Settings.Reset();
        }

        protected virtual AudioSettings GetSettings()
        {
            return new DefaultAudioSettings();
        }

        protected virtual void Init()
        {
            LoadSettings();
            InitAudioSpeakers();
        }

        protected virtual void Start()
        {
            ApplySettings();
        }

        protected virtual void OnDestroy()
        {
            SaveSettings();
        }

        public AudioSpeaker Play(AudioClip audioClip, AudioMixerGroup audioMixerGroup, float volume = 1F, bool loop = false, Action onFinish = null)
        {
            var audioSpeaker = m_SoundEffectAudioSpeakers.GetAudioSpeaker();
            audioSpeaker.Volume = volume;
            audioSpeaker.Loop = loop;
            audioSpeaker.AudioClip = audioClip;
            audioSpeaker.Play(() =>
            {
                m_SoundEffectAudioSpeakers.ReturnAudioSpeaker(audioSpeaker);
                onFinish?.Invoke();
            });

            return audioSpeaker;
        }

        public AudioSpeaker Play(AudioClip audioClip, Action onFinish = null)
        {
            return Play(audioClip, m_MasterMixer, onFinish: onFinish);
        }

        public AudioSpeaker PlayMetronome(AudioClip audioClip, Action onFinish = null)
        {
            var speaker = m_MetronomeAudioSpeaker;
            speaker.AudioClip = audioClip;
            speaker.OutputAudioMixerGroup = GetAudioMixer(AudioChannel.Metronome);
            speaker.Stop();
            speaker.Play(() =>
            {
                onFinish?.Invoke();
            });

            return speaker;
        }

        public AudioSpeaker Play(IAdjustableAudioClip audio, Action onFinish = null)
        {
            var speaker = m_MusicAudioSpeaker;
            if(audio.Channel == AudioChannel.SoundEffect)
            {
                speaker = m_SoundEffectAudioSpeakers.GetAudioSpeaker();
            }

            speaker.Volume = audio.Volume;
            speaker.Loop = audio.IsLoop;
            speaker.AudioClip = audio.AudioClip;
            speaker.OutputAudioMixerGroup = GetAudioMixer(audio.Channel);
            speaker.Stop();
            speaker.Play(() =>
            {
                if(audio.Channel == AudioChannel.SoundEffect)
                {
                    m_SoundEffectAudioSpeakers.ReturnAudioSpeaker(speaker);
                }
                onFinish?.Invoke();
            });

            return speaker;
        }

        public AudioSpeaker GetAudioSpeaker()
        {
            return m_CustomAudioSpeakers.GetAudioSpeaker();
        }

        public void ReturnAudioSpeaker(AudioSpeaker speaker)
        {
            m_CustomAudioSpeakers.ReturnAudioSpeaker(speaker);
        }

        private void InitAudioSpeakers()
        {
            m_CustomAudioSpeakers = new GameObject("CustomAudioSpeakers", typeof(AudioSpeakerCollection)).GetComponent<AudioSpeakerCollection>();
            m_CustomAudioSpeakers.transform.SetParent(transform);
            m_CustomAudioSpeakers.Init();

            m_SoundEffectAudioSpeakers = new GameObject("SoundEffectAudioSpeakers", typeof(AudioSpeakerCollection)).GetComponent<AudioSpeakerCollection>();
            m_SoundEffectAudioSpeakers.transform.SetParent(transform);
            m_SoundEffectAudioSpeakers.Init();

            var parent = new GameObject("MusicAudioSpeakers").transform;
            parent.SetParent(transform);
            m_MusicAudioSpeaker = new GameObject("MusicAudioSpeaker", typeof(AudioSpeaker)).GetComponent<AudioSpeaker>();
            m_MusicAudioSpeaker.transform.SetParent(parent);
            m_MusicAudioSpeaker.Init();

            var metronomeParent = new GameObject("MetronomeAudioSpeakers").transform;
            metronomeParent.SetParent(transform);
            m_MetronomeAudioSpeaker = new GameObject("MetronomeAudioSpeakers", typeof(AudioSpeaker)).GetComponent<AudioSpeaker>();
            m_MetronomeAudioSpeaker.transform.SetParent(metronomeParent);
            m_MetronomeAudioSpeaker.Init();
        }

        private void LoadSettings()
        {
            m_Settings = GetSettings();
        }

        private void ApplySettings()
        {
            MasterVolume = m_Settings.MasterVolume;
            BGMVolume = m_Settings.BGMVolume;
            SFXVolume = m_Settings.SFXVolume;
            MidiPlaybackVolume = m_Settings.MidiPlaybackVolume;
        }

        private void SaveSettings()
        {
            if(m_Settings == null)
                return;

            m_Settings.Save();
        }

        public AudioMixerGroup GetAudioMixer(AudioChannel channel)
        {
            switch(channel)
            {
                case AudioChannel.Music:
                    return m_MusicMixer;
                case AudioChannel.SoundEffect:
                    return m_SoundEffectMixer;
                case AudioChannel.Metronome:
                    return m_MetronomeMixer;
                case AudioChannel.MidiPlayback:
                    return m_MidiPlaybackMixer;
                default:
                    return m_MasterMixer;
            }
        }
    }
}
