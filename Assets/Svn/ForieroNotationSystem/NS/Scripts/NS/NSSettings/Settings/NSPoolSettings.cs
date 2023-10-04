/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Settings;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Pool Settings")]
[SettingsManager] public partial class NSPoolSettings : Settings<NSPoolSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Pool Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();
    
    public enum Pool
    {
        Fixed = 0,
        FixedEffects = 10,
        FixedOverlay = 20,
        Movable = 30,
        MovableEffects = 40,
        MovableOverlay = 50,
        Undefined = int.MaxValue
    }
    
    public NSPoolBank fixedPoolBank;
    [FormerlySerializedAs("fixedEffectsBoolBank")] public NSPoolBank fixedEffectsPoolBank;
    public NSPoolBank fixedOverlayPoolBank;
    public NSPoolBank movablePoolBank;
    public NSPoolBank movableEffectsPoolBank;
    [FormerlySerializedAs("overlayPoolBank")] public NSPoolBank movableOverlayPoolBank;

    public static NSPoolBank GetPoolBank(Pool poolBank)
    {
        return poolBank switch
        {
            Pool.Fixed => instance.fixedPoolBank,
            Pool.FixedEffects => instance.fixedEffectsPoolBank,
            Pool.FixedOverlay => instance.fixedOverlayPoolBank,
            Pool.Movable => instance.movablePoolBank,
            Pool.MovableEffects => instance.movableEffectsPoolBank,
            Pool.MovableOverlay => instance.movableOverlayPoolBank,
            Pool.Undefined => null,
            _ => null
        };
    }
}
