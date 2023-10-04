using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGraphicsSettings : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    public Slider brightnessSlider;
    public Slider contrastSlider;
    public Toggle motionBlurToggle;

    void Awake()
    {
        qualityDropdown.options.Clear();
        foreach (var q in System.Enum.GetNames(typeof(GameSettings.GraphicsSettings.QualityEnum)))
        {
            qualityDropdown.options.Add(new TMP_Dropdown.OptionData(q));
        }
        
        qualityDropdown.onValueChanged.AddListener(OnQualityDropdownChanged);
        brightnessSlider.onValueChanged.AddListener((v) => { GameSettings.GraphicsSettings.Brightness = v; });
        contrastSlider.onValueChanged.AddListener((v) => { GameSettings.GraphicsSettings.Contrast = v;});
        motionBlurToggle.onValueChanged.AddListener((v) => { GameSettings.GraphicsSettings.MotionBlur = v;});
    }

    private void OnQualityDropdownChanged(int v)
    {
        GameSettings.GraphicsSettings.Quality = (GameSettings.GraphicsSettings.QualityEnum) v;
    }

    private void OnEnable()
    {
        qualityDropdown.SetValueWithoutNotify((int)GameSettings.GraphicsSettings.Quality);
        brightnessSlider.SetValueWithoutNotify(GameSettings.GraphicsSettings.Brightness);
        contrastSlider.SetValueWithoutNotify(GameSettings.GraphicsSettings.Contrast);
        motionBlurToggle.SetIsOnWithoutNotify(GameSettings.GraphicsSettings.MotionBlur);
    }
}
