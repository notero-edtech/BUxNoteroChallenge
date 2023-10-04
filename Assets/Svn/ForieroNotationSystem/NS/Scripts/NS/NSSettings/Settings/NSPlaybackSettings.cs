/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using ForieroEngine.Settings;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Playback Settings")]
[SettingsManager] public partial class NSPlaybackSettings : Settings<NSPlaybackSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Playback Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();

    [Tooltip("")]
    public bool pickupBar = false;
    [Tooltip("")]
    public DivisionEnum staccatoDuration = DivisionEnum.Three;
    [Tooltip("")]
    public DivisionEnum staccatissimoDuration = DivisionEnum.Four;
    [Tooltip("")]
    [Range(0f, 0.5f)] public float shortenNoteDurationBy = 0.1f;
    
    [Header("Midi")] 
    [Range(0, 127)] public int defaultNoteAttack = 80;
    
    public static bool PickupBar {
        get => instance.pickupBar;
        set => instance.pickupBar = value;
    }

    public static float StaccatoDurationMultiplier => 1f / (float)StaccatoDuration;
    public static DivisionEnum StaccatoDuration
    {
        get => instance.staccatoDuration;
        set => instance.staccatoDuration = value;
    }

    public static float StaccatissimoDurationMultiplier => 1f / (float)StaccatissimoDuration;
    public static DivisionEnum StaccatissimoDuration
    {
        get => instance.staccatissimoDuration;
        set => instance.staccatissimoDuration = value;
    }
    
    public static float ShortenNoteDurationBy
    {
        get => instance.shortenNoteDurationBy;
        set => instance.shortenNoteDurationBy = value;
    }
    
    public static int DefaultNoteAttack
    {
        get => instance.defaultNoteAttack;
        set => instance.defaultNoteAttack = value;
    }
}
