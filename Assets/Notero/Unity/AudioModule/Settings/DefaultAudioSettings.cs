using UnityEngine;

namespace Notero.Unity.AudioModule.Settings
{
    public class DefaultAudioSettings : AudioSettings
    {
        private string PlayPrefPrefix => $"{typeof(DefaultAudioSettings).FullName}";
        private string MasterVolumeKey => $"{PlayPrefPrefix}.MasterVolume";
        private string BGMVolumeKey => $"{PlayPrefPrefix}.BGMVolume";
        private string SFXVolumeKey => $"{PlayPrefPrefix}.SFXVolume";
        private string MidiPlaybackVolumeKey => $"{PlayPrefPrefix}.MidiPlaybackVolume";

        public override float MasterVolume
        {
            get => m_MasterVolume;
            set => m_MasterVolume = value;
        }
        public override float BGMVolume
        {
            get => m_BGMVolume;
            set => m_BGMVolume = value;
        }
        public override float SFXVolume
        {
            get => m_SFXVolume;
            set => m_SFXVolume = value;
        }
        public override float MidiPlaybackVolume 
        { 
            get => m_MidiPlaybackVolume;
            set => m_MidiPlaybackVolume = value; 
        }

        private float m_MasterVolume;
        private float m_BGMVolume;
        private float m_SFXVolume;
        private float m_MidiPlaybackVolume;

        public DefaultAudioSettings()
        {
            m_MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1F);
            m_BGMVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 1F);
            m_SFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1F);
            m_MidiPlaybackVolume = PlayerPrefs.GetFloat(MidiPlaybackVolumeKey, 0F);
        }

        public override void Save()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, m_MasterVolume);
            PlayerPrefs.SetFloat(BGMVolumeKey, m_BGMVolume);
            PlayerPrefs.SetFloat(SFXVolumeKey, m_SFXVolume);
            PlayerPrefs.SetFloat(MidiPlaybackVolumeKey, m_MidiPlaybackVolume);
        }

        public override void Reset()
        {
            PlayerPrefs.DeleteKey(MasterVolumeKey);
            PlayerPrefs.DeleteKey(BGMVolumeKey);
            PlayerPrefs.DeleteKey(SFXVolumeKey);
            PlayerPrefs.DeleteKey(MidiPlaybackVolumeKey);
        }
    }
}
