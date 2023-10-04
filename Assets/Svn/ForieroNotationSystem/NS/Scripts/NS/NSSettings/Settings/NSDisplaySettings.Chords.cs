/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using ForieroEngine.Settings;
#if UNITY_EDITOR
#endif

public partial class NSDisplaySettings : Settings<NSDisplaySettings>, ISettingsProvider
{
    [Serializable] public class ChordsItems
    {
        public bool render = true;
    }
    
    public static class Chords
    {
        private static ChordsItems I => instance.chords;
        
        public static bool Render { get => I.render; set => I.render = value; }
    }
}
