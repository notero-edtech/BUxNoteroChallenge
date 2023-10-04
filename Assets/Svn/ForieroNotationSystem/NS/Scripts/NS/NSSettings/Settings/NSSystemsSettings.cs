/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using ForieroEngine.Settings;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Systems Settings")]
[SettingsManager] public partial class NSSystemsSettings : Settings<NSSystemsSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Systems Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();

    [Header("System Settings")] 
    public NSDefaultSystemSettings defaultSystemSettings;
    public NSPageLayoutTickerSystemSettings pageLayoutTickerSystemSettings;
    public NSRollingTickerSystemSettings tickerSystemSettings;
    public NSRollingLeftRightSystemSettings rollingLeftRightSystemSettings;
    public NSRollingTopBottomSystemSettings rollingTopBottomSystemSettings;

    [Header("Fonts")] 
    public SystemFontEnum systemFont = SystemFontEnum.Clean;
    
    [Serializable] public class SystemFontDefinition
    {
        [Header("Fonts UI")]
        [Tooltip("SMuFL compliant font.")]
        public Font smuflFont;
        [Tooltip("SMuFL compliant text font.")]
        public Font textFont;

        [Header("Fonts TextMeshPRO")]
        [Tooltip("SMuFL compliant font.")]
        public TMP_FontAsset smuflFontTMP;
        [Tooltip("SMuFL compliant text font.")]
        public TMP_FontAsset textFontTMP;
    }
    
    [Tooltip("SMuFL Bravura Font")]
    public SystemFontDefinition cleanFont;
    [Tooltip("SMuFL Petaluma Font")]
    public SystemFontDefinition jazzFont;

    public static SystemFontEnum SystemFont
    {
        get => instance.systemFont;
        set => instance.systemFont = value;
    }

    public static SystemFontDefinition CurrentFont
    {
        get
        {
            switch (instance.systemFont)
            {
                default:
                case SystemFontEnum.Clean: return instance.cleanFont;
                case SystemFontEnum.Jazz: return instance.jazzFont;
            }
        }
    }

    public static Font SMuFLFont => CurrentFont.textFont;
    public static Font TextFont => CurrentFont.textFont;
    public static TMP_FontAsset SMuFLFontTMP => CurrentFont.smuflFontTMP;
    public static TMP_FontAsset TextFontTMP => CurrentFont.textFontTMP;
}
