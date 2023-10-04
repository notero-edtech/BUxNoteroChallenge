using System;
using ForieroEngine.Extensions;
using ForieroEngine.Settings;
using UnityEngine;
using UnityEngine.Audio;

public partial class GameSettings : Settings<GameSettings>, ISettingsProvider
{
    [Serializable]
    public class AudioSettings : AbstractSettings<AudioSettings>
    {
        public override void Init()
        {
            MasterVolume = MasterVolume;
            UIVolume = UIVolume;
            MusicVolume = MusicVolume;
            VoiceOverVolume = VoiceOverVolume;
            SfxVolume = SfxVolume;
            AmbienceVolume = AmbienceVolume;
        }
     
        #region Static

        public static Lang.LanguageCode Language
        {
            get => instance.audio.language;
            set
            {
                var changed = value != instance.audio.language;
                instance.audio.language = value;
                if(changed) OnLanguageChanged?.Invoke(value);
                instance.audio.Save();
            }
        }
        public static Action<Lang.LanguageCode> OnLanguageChanged;
        
        public static AudioMixer AudioMixer => instance.audio.audioMixer;

        static void SetAudioMixerUnityParameter(string parameter, float linearValue)
        {
            if(instance.audio.audioMixer) instance.audio.audioMixer.SetFloat(parameter, linearValue.ToDecibel());
        }
        
        static void SetAudioMixer3rdParameter(string parameter, float linearValue)
        {
            #if WWISE
            #elif FMOD
            var vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + parameter);
            if (vca.isValid()) vca.setVolume(linearValue);
            else Debug.LogError("FMOD | Can not find vca : " + parameter);
            #endif
        }
        
        public static float MasterVolume
        {
            get => instance.audio.masterVolume;
            set
            {
                var changed = !Mathf.Approximately(value, instance.audio.masterVolume);
                instance.audio.masterVolume = value;
                SetAudioMixerUnityParameter(instance.audio.masterVolumeUnity, value);
                SetAudioMixer3rdParameter(instance.audio.masterVolume3rd, value);
                if (changed)
                {
                    OnMasterVolumeChanged?.Invoke(value);
                    instance.audio.Save();    
                }
            }
        }
        public static Action<float> OnMasterVolumeChanged;
        
        public static float UIVolume
        {
            get => instance.audio.uiVolume;
            set
            {
                var changed = !Mathf.Approximately(value, instance.audio.uiVolume);
                instance.audio.uiVolume = value;
                SetAudioMixerUnityParameter(instance.audio.uiVolumeUnity, value);
                SetAudioMixer3rdParameter(instance.audio.uiVolume3rd, value);
                if (changed)
                {
                    OnUIVolumeChanged?.Invoke(value);
                    instance.audio.Save();
                }
            }
        }
        public static Action<float> OnUIVolumeChanged;
        
        public static float MusicVolume
        {
            get => instance.audio.musicVolume;
            set
            {
                var changed = !Mathf.Approximately(value, instance.audio.musicVolume);
                instance.audio.musicVolume = value;
                SetAudioMixerUnityParameter(instance.audio.musicVolumeUnity, value);
                SetAudioMixer3rdParameter(instance.audio.musicVolume3rd, value);
                if (changed)
                {
                    OnMusicVolumeChanged?.Invoke(value);
                    instance.audio.Save();
                }
            }
        }
        public static Action<float> OnMusicVolumeChanged;
        
        public static float VoiceOverVolume
        {
            get => instance.audio.voiceOverVolume;
            set
            {
                var changed = !Mathf.Approximately(value, instance.audio.voiceOverVolume);
                instance.audio.voiceOverVolume = value;
                SetAudioMixerUnityParameter(instance.audio.voiceOverVolumeUnity, value);
                SetAudioMixer3rdParameter(instance.audio.voiceOverVolume3rd, value);
                if (changed)
                {
                    OnVoiceOverVolumeChanged?.Invoke(value);
                    instance.audio.Save();
                }
            }
        }
        public static Action<float> OnVoiceOverVolumeChanged;
        
        public static float SfxVolume
        {
            get => instance.audio.sfxVolume;
            set
            {
                var changed = !Mathf.Approximately(value, instance.audio.sfxVolume);
                instance.audio.sfxVolume = value;
                SetAudioMixerUnityParameter(instance.audio.sfxVolumeUnity, value);
                SetAudioMixer3rdParameter(instance.audio.sfxVolume3rd, value);
                if (changed)
                {
                    OnSfxVolumeChanged?.Invoke(value);
                    instance.audio.Save();
                }
            }
        }
        public static Action<float> OnSfxVolumeChanged;
        
        public static float AmbienceVolume
        {
            get => instance.audio.ambienceVolume;
            set
            {
                var changed = !Mathf.Approximately(value, instance.audio.ambienceVolume);
                instance.audio.ambienceVolume = value;
                SetAudioMixerUnityParameter(instance.audio.ambienceVolumeUnity, value);
                SetAudioMixer3rdParameter(instance.audio.ambienceVolume3rd, value);
                if (changed)
                {
                    OnAmbienceVolumeChanged?.Invoke(value);
                    instance.audio.Save();
                }
            }
        }
        public static Action<float> OnAmbienceVolumeChanged;
        
        #endregion

        public AudioMixer audioMixer => SoundSettings.instance.audioMixer;
        
        public Lang.LanguageCode language = Lang.LanguageCode.EN;
        public string masterVolumeUnity = "MASTER_VOLUME";
        public string masterVolume3rd = "";
        [Range(0f, 1f)] public float masterVolume = 1f;
        
        public string uiVolumeUnity = "UI_VOLUME";
        public string uiVolume3rd = "";
        [Range(0f, 1f)] public float uiVolume = 1f;
        
        public string musicVolumeUnity = "MUSIC_VOLUME";
        public string musicVolume3rd = "";
        [Range(0f, 1f)] public float musicVolume = 1f;
        
        public string voiceOverVolumeUnity = "VO_VOLUME";
        public string voiceOverVolume3rd = "";
        [Range(0f, 1f)] public float voiceOverVolume = 1f;
        
        public string sfxVolumeUnity = "SFX_VOLUME";
        public string sfxVolume3rd = "";
        [Range(0f, 1f)] public float sfxVolume = 1f;
        
        public string ambienceVolumeUnity = "AMBIENCE_VOLUME";
        public string ambienceVolume3rd = "";
        [Range(0f, 1f)] public float ambienceVolume = 1f;
    }
}
