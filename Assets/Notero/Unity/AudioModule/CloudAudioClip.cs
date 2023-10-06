using Notero.Unity.AudioModule.Utilities;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Notero.Unity.AudioModule
{
    public class CloudAudioClip : IAdjustableAudioClip
    {
        public AudioClip AudioClip
        {
            get => m_AudioClip;
            set => m_AudioClip = value;
        }
        public float Volume
        {
            get => Mathf.Clamp01(m_Volume);
            set => m_Volume = Mathf.Clamp01(value);
        }
        public bool IsLoop
        {
            get => m_IsLoop;
            set => m_IsLoop = value;
        }
        public AudioChannel Channel
        {
            get => m_Channel;
            set => m_Channel = value;
        }

        public bool IsDownloaded => m_AudioClip != null;
        public AudioDataLoadState LoadState => m_AudioClip == null ? AudioDataLoadState.Unloaded : m_AudioClip.loadState;

        private AudioClip m_AudioClip;
        private AudioType m_AudioType;
        private float m_Volume = 1F;
        private bool m_IsLoop;
        private AudioChannel m_Channel = AudioChannel.SoundEffect;
        protected string m_URL;

        public CloudAudioClip(string url, AudioType audioType)
        {
            m_URL = url;
            m_AudioType = audioType;
        }

        public async void Download(Action onFinish = null)
        {
            if(m_AudioClip == null)
            {
                using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(m_URL, m_AudioType);
                var async = request.SendWebRequest();
                await async;

                if(request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    m_AudioClip = DownloadHandlerAudioClip.GetContent(request);
                    m_AudioClip.name = m_URL;
                }
            }

            onFinish?.Invoke();
        }
    }
}
