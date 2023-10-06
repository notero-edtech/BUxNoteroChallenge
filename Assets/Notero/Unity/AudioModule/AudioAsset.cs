using UnityEngine;

namespace Notero.Unity.AudioModule
{
    public class AudioAsset : ScriptableObject, IAdjustableAudioClip
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
        public AudioDataLoadState LoadState => m_AudioClip == null ? AudioDataLoadState.Unloaded : m_AudioClip.loadState;

        [SerializeField]
        [Tooltip("A container for audio data.")]
        private AudioClip m_AudioClip;
        [SerializeField]
        [Tooltip("Volume value between 0 and 1.")]
        private float m_Volume = 1F;
        [SerializeField]
        [Tooltip("Is the audio clip looping?")]
        private bool m_IsLoop;
        [SerializeField]
        [Tooltip("Target channel for audio output.")]
        private AudioChannel m_Channel = AudioChannel.SoundEffect;
    }
}
