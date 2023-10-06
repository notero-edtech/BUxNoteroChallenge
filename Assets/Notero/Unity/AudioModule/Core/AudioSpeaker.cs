using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Notero.Unity.AudioModule.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSpeaker : MonoBehaviour
    {
        public AudioClip AudioClip
        {
            get => m_AudioSource.clip;
            set => m_AudioSource.clip = value;
        }
        public AudioMixerGroup OutputAudioMixerGroup
        {
            get => m_AudioSource.outputAudioMixerGroup;
            set => m_AudioSource.outputAudioMixerGroup = value;
        }
        public float Volume
        {
            get => m_AudioSource.volume;
            set => m_AudioSource.volume = value;
        }
        public bool Loop
        {
            get => m_AudioSource.loop;
            set => m_AudioSource.loop = value;
        }

        public AudioSource AudioSource => m_AudioSource;

        private AudioSource m_AudioSource;
        private Action m_OnPlayingFinish;

        public void Play(Action onFinish)
        {
            m_OnPlayingFinish = onFinish;
            m_AudioSource.Play();
            name = $"AudioSpeaker:Playing {AudioClip.name}";
            StartCoroutine(WatchingRoutine());
        }

        public void Stop()
        {
            m_AudioSource.Stop();
            OnPlayingFinish();
        }

        public void Init()
        {
            name = "AudioSpeaker";
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
        }

        IEnumerator WatchingRoutine()
        {
            yield return new WaitUntil(() => !m_AudioSource.isPlaying);
            OnPlayingFinish();
        }

        void OnPlayingFinish()
        {
            if(m_OnPlayingFinish == null)
            {
                return;
            }

            name = "AudioSpeaker";
            m_OnPlayingFinish?.Invoke();
            m_OnPlayingFinish = null;
        }
    }
}
