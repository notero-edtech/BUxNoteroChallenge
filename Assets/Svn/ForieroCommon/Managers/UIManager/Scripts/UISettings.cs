using UnityEngine;
using DG.Tweening;
using System;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class UISettings : Settings<UISettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/UI", false, -1000)] public static void UISettingsMenu() => Select();
#endif

    public enum Provider
    {
        SoundManager,
        MasterAudio
    }

    public TTUIAnimStyle defaultAnimStyle;
    public TTUISoundStyle defaultSoundStyle;
    public TTUITooltipStyle defaultTooltipStyle;

    public enum UIAnimEnum
    {
        None,
        Position,
        LocalPosition,
        AnchoredPosition,
        AnchoredPosition3D,
        Rotation,
        LocalRotation,
        Scale,
        LocalScale,
        Color
    }

    [Serializable]
    public class UIBaseDOTweenStyle
    {
        public bool enabled = true;
        public Ease ease = Ease.InOutSine;
        public float time = 1f;
        public float delay = 0f;
        public string onStartSound = "";
        public string onFinishedSound = "";
        public UIAnimEnum animEnum = UIAnimEnum.None;
        public Vector2 vector2;
        public Vector3 vector3;
        public Color color;
    }

    [Serializable]
    public class UIDOTweenStyle
    {
        public bool enabled = true;
        public string id;
        public UIBaseDOTweenStyle[] onEnable;
        public UIBaseDOTweenStyle[] onAwake;
        public UIBaseDOTweenStyle[] onStart;
        public UIBaseDOTweenStyle[] onMouseEnter;
        public UIBaseDOTweenStyle[] onMouseExit;
        public UIBaseDOTweenStyle[] onMouseDown;
        public UIBaseDOTweenStyle[] onMouseUp;
    }

    public UIDOTweenStyle[] uiDOTweenStyles;

    [Serializable]
    public class UISoundStyle
    {
        public bool enabled = true;
        public string onEnable;
        public string onAwake;
        public string onStart;
        public string onMouseEnter;
        public string onMouseExit;
        public string onMouseDown;
        public string onMouseUp;
    }

    public UISoundStyle[] uiSoundStyles;
}
