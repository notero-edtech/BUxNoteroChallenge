/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.Settings;
using ForieroEngine.Music.NotationSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Settings")]
[SettingsManager] public partial class NSSettings : Settings<NSSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();
    
    private static TextAsset _releases;
    public static TextAsset Releases => _releases ? _releases : _releases = Resources.Load<TextAsset>("NS/RELEASES");
    
    private static TextAsset _readMe;
    public static TextAsset ReadMe => _readMe ? _readMe : _readMe = Resources.Load<TextAsset>("NS/README");
}
