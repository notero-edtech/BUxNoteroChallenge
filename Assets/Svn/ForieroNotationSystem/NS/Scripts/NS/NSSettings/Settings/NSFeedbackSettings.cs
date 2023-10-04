/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Feedback Settings")]
[SettingsManager] public partial class NSFeedbackSettings : Settings<NSFeedbackSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Feedback Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();
    
    [Header("Reaction Ranges")]
    [Tooltip("")]
    public float tooEarly = -0.5f;
    [Tooltip("")]
    public float early = -0.25f;
    [Tooltip("")]
    public float perfectMin = -0.1f;
    [Tooltip("")]
    public float perfectMax = 0.1f;
    [Tooltip("")]
    public float late = 0.25f;
    [Tooltip("")]
    public float tooLate = 0.5f;
}
