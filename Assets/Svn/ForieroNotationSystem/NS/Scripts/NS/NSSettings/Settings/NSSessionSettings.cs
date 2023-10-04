/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Session Settings")]
[SettingsManager] public partial class NSSessionSettings : Settings<NSSessionSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Session Settings")] public static void NSSettingsMenu() => Select();   
#endif

    public NSSessionBank[] banks;
}
