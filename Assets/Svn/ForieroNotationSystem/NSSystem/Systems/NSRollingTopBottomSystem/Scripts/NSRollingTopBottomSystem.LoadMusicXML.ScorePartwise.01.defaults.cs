/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        static partial class ScorePartwise
        {
            static partial class Defaults
            {
                public static void Parse(defaults defaults)
                {
                    //if (defaults != null)
                    //{
                    //    if (!parseStaffDistance || defaults == null || defaults.stafflayout == null)
                    //    {
                    //        for (int i = 0; i < nsPart.parsing.staves.Count; i++) { nsPart.parsing.staves[i].options.lineDistance = ns.staveLineDistance; }
                    //    }
                    //    else if (defaults.stafflayout.Length == 1 && defaults.stafflayout[0].GetStaveNumber(-1) == -1)
                    //    {
                    //        for (int i = 0; i < nsPart.parsing.staves.Count; i++) { nsPart.parsing.staves[i].options.lineDistance = (float)defaults.stafflayout[0].staffdistance / 10f; }
                    //    }
                    //    else
                    //    {
                    //        for (int i = 0; i < defaults.stafflayout.Length; i++)
                    //        {
                    //            var staffNumber = defaults.stafflayout[i].GetStaveNumber(0);
                    //            nsPart.parsing.staves[staffNumber].options.lineDistance = (float)defaults.stafflayout[i].staffdistance / 10f;
                    //        }
                    //    }
                    //}
                }
            }
        }
    }
}
