using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAudioSettings : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;
    public Slider masterVolumeSlider;
    public Slider uiVolumeSlider;
    public Slider voiceOverVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider ambienceVolumeSlider;

    private void Awake()
    {
        languageDropdown.options.Clear();
        foreach (var l in LangSettings.instance.supportedLanguages)
        {
            if(l.included) languageDropdown.options.Add(new TMP_Dropdown.OptionData(Lang.GetLanguageString(l.langCode)));
        }

        languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
        masterVolumeSlider.onValueChanged.AddListener((v) => { GameSettings.AudioSettings.MasterVolume = v;});
        uiVolumeSlider.onValueChanged.AddListener((v) => { GameSettings.AudioSettings.UIVolume = v;});
        voiceOverVolumeSlider.onValueChanged.AddListener((v) => { GameSettings.AudioSettings.VoiceOverVolume = v;});
        musicVolumeSlider.onValueChanged.AddListener((v) => { GameSettings.AudioSettings.MusicVolume = v;});
        sfxVolumeSlider.onValueChanged.AddListener((v) => { GameSettings.AudioSettings.SfxVolume = v;});
        ambienceVolumeSlider.onValueChanged.AddListener((v) => { GameSettings.AudioSettings.AmbienceVolume = v;});
    }

    private void OnEnable()
    {
        for (int i = 0; i<languageDropdown.options.Count; i++)
        {
            if (languageDropdown.options[i].text.GetLangCodeFromLanguageString() == Lang.selectedLanguage)
            {
                languageDropdown.SetValueWithoutNotify(i);
                break;
            }
        }
        
        masterVolumeSlider.SetValueWithoutNotify(GameSettings.AudioSettings.MasterVolume);
        uiVolumeSlider.SetValueWithoutNotify(GameSettings.AudioSettings.UIVolume);
        voiceOverVolumeSlider.SetValueWithoutNotify(GameSettings.AudioSettings.VoiceOverVolume);
        musicVolumeSlider.SetValueWithoutNotify(GameSettings.AudioSettings.MusicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(GameSettings.AudioSettings.SfxVolume);
        ambienceVolumeSlider.SetValueWithoutNotify(GameSettings.AudioSettings.AmbienceVolume);
    }

    void OnLanguageDropdownChanged(int v)
    {
        Lang.selectedLanguage = languageDropdown.options[v].text.GetLangCodeFromLanguageString();
    }
}
