/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        static partial class ScorePartwise
        {
            static partial class PartList
            {
                internal static void Parse(partlist partlist)
                {
                    if (partlist?.Items == null) return;
                    
                    var groupIndex = 0;
                    var groupBracket = false;
                    var groupBarlines = true;

                    for (int partListIndexer = 0; partListIndexer < partlist.Items.Length; partListIndexer++)
                    {
                        var plItem = partlist.Items[partListIndexer];
                        if (plItem is partgroup)
                        {
                            var partgroup = plItem as partgroup;
                        }
                        else if (plItem is scorepart)
                        {
                            var spScorePart = plItem as scorepart;

                            var nsPart = ns.AddObject<NSPart>( PoolEnum.NS_FIXED);
                            nsPart.options.id = spScorePart.id;
                            nsPart.options.index = parts.Count;
                            nsPart.options.groupIndex = 0;
                            nsPart.options.bracket = true;
                            //nsPart.options.groupBarlines = groupBarlines;
                            nsPart.Commit();

                            parts.Add(nsPart.options.id, nsPart);
                        }
                    }
                }
            }
        }
    }
}
