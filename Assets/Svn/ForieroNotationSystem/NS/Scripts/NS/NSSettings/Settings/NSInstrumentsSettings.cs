/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "NS/Settings/NS Instruments Settings")]
[SettingsManager] public partial class NSInstrumentsSettings : Settings<NSInstrumentsSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/NS/NS Instruments Settings")] public static void NSSettingsMenu() => Select();   
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Init() => Instance();
    
    [Serializable] public class Instrument
    {
        public string id;
        public GameObject prefab;
        public List<Controller> controllers;
    }
    
    [Serializable] public class Controller
    {
        public string id;
        public GameObject prefab;
    }
    
    public List<Instrument> instruments;
}
