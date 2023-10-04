/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Debug Settings")]
[SettingsManager] public partial class NSDebugSettings : Settings<NSDebugSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Debug Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();
    
    [Header("Debug")]
    public bool debug = false;
  
    [Header("Debug - Display")]
    public bool hiddenObjects = false;

    public bool copyrightEditorLabel = true;
}
