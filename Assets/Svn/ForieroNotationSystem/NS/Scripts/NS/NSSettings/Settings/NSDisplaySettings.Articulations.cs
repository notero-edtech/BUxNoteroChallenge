/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System;
using ForieroEngine.Settings;
#if UNITY_EDITOR
#endif

public partial class NSDisplaySettings : Settings<NSDisplaySettings>, ISettingsProvider
{
    [Serializable] public class ArticulationsItems
    {
        public bool render = true;
        public bool accent = false;
        public bool breathMark = false;
        public bool caesura = false;
        public bool detachedLegato = false;
        public bool doit = false;
        public bool fallOff = false;
        public bool otherArticulation = false;
        public bool plop = false;
        public bool softAccent = false;
        public bool spiccato = false;
        public bool staccatissimo = false;
        public bool staccato = false;
        public bool stress = false;
        public bool strongAccent = false;
        public bool tenuto = false;
        public bool unstress = false;
    }
    
    public static class Articulations
    {
        private static ArticulationsItems I => instance.articulations;
        
        public static bool Render { get => I.render; set => I.render = value; }
        public static bool Accent { get => I.render && I.accent; set => I.accent = value; }
        public static bool BreathMark { get => I.render && I.breathMark; set => I.breathMark = value; }
        public static bool Caesura { get => I.render && I.caesura; set => I.caesura = value; }
        public static bool DetachedLegato { get => I.render && I.detachedLegato; set => I.detachedLegato = value; }
        public static bool Doit { get => I.render && I.doit; set => I.doit = value; }
        public static bool FallOff { get => I.render && I.fallOff; set => I.fallOff = value; }
        public static bool OtherArticulation { get => I.render && I.otherArticulation; set => I.otherArticulation = value; }
        public static bool Plop { get => I.render && I.plop; set => I.plop = value; }
        public static bool SoftAccent { get => I.render && I.softAccent; set => I.softAccent = value; }
        public static bool Spiccato { get => I.render && I.spiccato; set => I.spiccato = value; }
        public static bool Staccatissimo { get => I.render && I.staccatissimo; set => I.staccatissimo = value; }
        public static bool Staccato { get => I.render && I.staccato; set => I.staccato = value; }
        public static bool Stress { get => I.render && I.stress; set => I.stress = value; }
        public static bool StrongAccent { get => I.render && I.strongAccent; set => I.strongAccent = value; }
        public static bool Tenuto { get => I.render && I.tenuto; set => I.tenuto = value; }
        public static bool Unstress { get => I.render && I.unstress; set => I.unstress = value; }
    }
}
