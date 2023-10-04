using System;
using UnityEngine;
using ForieroEngine.Settings;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class ScreenSettings : Settings<ScreenSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Screen", false, -1000)] public static void ScreenSettingsMenu() => Select();    
#endif

    public enum VSyncCountEnum
    {
        None = 0,
        Every = 1,
        EverySecond = 2
    }

    public enum AntialiasingEnum
    {
        None = 0,
        OneX = 1,
        TwoX = 2,
        FourX = 4,
        EightX = 8
    }

    [Serializable] public class ScreenItem
    {
        public enum SleepTimeout
        {
            NeverSleep = -1,
            SystemSetting = -2
        }

        public VSyncCountEnum vSyncCount = VSyncCountEnum.Every;
        [Range(-1, 120)] public int frameRate = 60;
        public AntialiasingEnum antialiasing = AntialiasingEnum.None;
        public SleepTimeout sleepTimeout = SleepTimeout.NeverSleep;
    }

    [Serializable]
    public class ScreenMobileItem : ScreenItem
    {
        [Range(1, 10)] public int renderFrameRateInterval = 1;
    }

    public ScreenItem editor;
    public ScreenItem desktop;
    public ScreenMobileItem mobile;
    public ScreenItem xbox;
    public ScreenItem ps5;
    public ScreenItem ps4;
    public ScreenItem ps3;
    public ScreenItem psm;
    public ScreenItem nintendoswitch;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;

        var current = instance.desktop;
        
#if UNITY_EDITOR
        current = instance.editor;
#elif UNITY_XBOXONE
        current = instance.xbox;
#elif UNITY_PS3
        current = instance.ps3;
#elif UNITY_PS4 
        current = instance.ps4;
#elif UNITY_PS5
        current = instance.ps5;
#elif UNITY_PSM
        current = instance.psm;  
#elif UNITY_SWITCH
        current = instance.nintendoswitch;
#else
        if (IsMobile() || Application.isMobilePlatform) current = instance.mobile;
        else current = instance.desktop;
        OnDemandRendering.renderFrameInterval = instance.mobile.renderFrameRateInterval;
#endif
        
        QualitySettings.vSyncCount = (int)current.vSyncCount;
        QualitySettings.antiAliasing = (int)current.antialiasing;
        Application.targetFrameRate = current.frameRate;
        Screen.sleepTimeout = (int)current.sleepTimeout;
        
        if(ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (ScreenSettings - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

    public static bool IsMobile()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS3 || UNITY_PS4 || UNITY_PS5
        return false;
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_PSM || UNITY_SWITCH)
        return true;
#elif UNITY_WSA
        return SystemInfo.deviceType == DeviceType.Handheld;
#else
        return true;
#endif
    }
}
