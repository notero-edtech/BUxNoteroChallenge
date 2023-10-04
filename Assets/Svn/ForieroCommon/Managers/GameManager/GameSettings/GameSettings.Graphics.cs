using System;
using ForieroEngine.Settings;
using UnityEngine;

public partial class GameSettings : Settings<GameSettings>, ISettingsProvider
{
    [Serializable]
    public class GraphicsSettings : AbstractSettings<GraphicsSettings>
    {
        public override void Init()
        {
            
        }
        
        public static QualityEnum Quality
        {
            get => instance.graphics.quality;
            set
            {
                var changed = value != instance.graphics.quality;
                instance.graphics.quality = value;
                if (changed)
                {
                    OnQualityChanged?.Invoke(value);
                    instance.graphics.Save();
                }
            }
        }

        public static Action<QualityEnum> OnQualityChanged;

        public static float Brightness
        {
            get => instance.graphics.brightness;
            set
            {
                var changed = !Mathf.Approximately(value, instance.graphics.brightness);
                instance.graphics.brightness = value;
                if (changed)
                {
                    OnBrightnessChanged?.Invoke(value);
                    instance.graphics.Save();
                }
            }
        }

        public static Action<float> OnBrightnessChanged;

        public static float Contrast
        {
            get => instance.graphics.contrast;
            set
            {
                var changed = !Mathf.Approximately(value, instance.graphics.contrast);
                instance.graphics.contrast = value;
                if (changed)
                {
                    OnContrastChanged?.Invoke(value);
                    instance.graphics.Save();
                }
            }
        }

        public static Action<float> OnContrastChanged;

        public static bool MotionBlur
        {
            get => instance.graphics.motionBlur;
            set
            {
                var changed = value != instance.graphics.motionBlur;
                instance.graphics.motionBlur = value;
                if (changed)
                {
                    OnMotionBlurChanged?.Invoke(value);
                    instance.graphics.Save();
                }
            }
        }

        public static Action<bool> OnMotionBlurChanged;


        public enum QualityEnum
        {
            Low,
            Medium,
            High
        }

        public QualityEnum quality = QualityEnum.Medium;
        public bool motionBlur = true;
        [Range(0f, 1f)] public float brightness = 0.5f;
        [Range(0f, 1f)] public float contrast = 0.5f;
    }
}
