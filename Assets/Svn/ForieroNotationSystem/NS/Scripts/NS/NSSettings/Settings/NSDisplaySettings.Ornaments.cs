/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using ForieroEngine.Settings;
#if UNITY_EDITOR
#endif

public partial class NSDisplaySettings : Settings<NSDisplaySettings>, ISettingsProvider
{
    [Serializable] public class OrnamentsItems
    {
        public bool render = true;
    }
    
    public static class Ornaments
    {
        private static OrnamentsItems I => instance.ornaments;
        
        public static bool Render { get => I.render; set => I.render = value; }
    }
}
