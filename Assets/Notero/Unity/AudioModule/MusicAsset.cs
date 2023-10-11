using UnityEngine;

namespace Notero.Unity.AudioModule
{
    [CreateAssetMenu(fileName = "MusicAsset", menuName = "Notero/Audio Module/MusicAsset", order = 2)]
    public class MusicAsset : ScriptableObject, IAdjustableAudioClip
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
            get => true;
            set { }
        }
        public AudioChannel Channel
        {
            get => AudioChannel.Music;
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
