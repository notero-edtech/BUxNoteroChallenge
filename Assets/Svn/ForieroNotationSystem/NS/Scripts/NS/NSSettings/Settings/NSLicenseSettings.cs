/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.Settings;
using ForieroEngine.Music.NotationSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS License Settings")]
[SettingsManager] public partial class NSLicenseSettings : Settings<NSLicenseSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS License Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();

    [Tooltip("")]
    public string company;
    [Tooltip("")]
    public string representative;
    [Tooltip("")]
    public string email;
    [Tooltip("")]
    public string licenseNumber;
    
}
