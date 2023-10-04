/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingLeftRightSystem : NS
    {
        private static partial class ScorePartwise
        {
            internal static partial class PartList
            {
                internal static void Parse(partlist partlist)
                {
                    if (partlist != null && partlist.Items != null)
                    {
                        var groupIndex = 0;
                        var groupBracket = false;
                        var groupBarlines = true;

                        for (var partListIndexer = 0; partListIndexer < partlist.Items.Length; partListIndexer++)
                        {
                            var plItem = partlist.Items[partListIndexer];
                            switch (plItem)
                            {
                                case partgroup spPartGroup: break;
                                case scorepart spScorePart:
                                {
                                    var nsPart = ns.AddObject<NSPart>(PoolEnum.NS_FIXED);
                                    nsPart.options.id = spScorePart.id;
                                    nsPart.options.index = ns.parsing.parts.Count;
                                    nsPart.options.groupIndex = 0;
                                    nsPart.options.bracket = true;
                                    //nsPart.options.groupBarlines = groupBarlines;
                                    nsPart.Commit();

                                    ns.parsing.parts.Add(nsPart.options.id, nsPart);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
