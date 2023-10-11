using UnityEngine;

namespace Notero.Unity.AudioModule
{
    [CreateAssetMenu(fileName = "SoundEffect", menuName = "Notero/Audio Module/SoundEffectAsset", order = 1)]
    public class SoundEffectAsset : ScriptableObject, IAdjustableAudioClip
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
            get => false;
            set { }
        }
        public AudioChannel Channel
        {
            get => AudioChannel.SoundEffect;
            set { }
        }
        public AudioDataLoadState LoadState => m_AudioClip == null ? AudioDataLoadState.Unloaded : m_AudioClip.loadState;

        [SerializeField]
        [Tooltip("A container for audio data.")]
        private AudioClip m_AudioClip;
        [SerializeField]
        [Tooltip("Volume value between 0 and 1.")]
        private float m_Volume = 1F;
    }
}
