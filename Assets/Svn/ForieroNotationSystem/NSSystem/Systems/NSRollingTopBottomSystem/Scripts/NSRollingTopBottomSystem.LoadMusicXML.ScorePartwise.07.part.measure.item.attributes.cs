/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        static partial class ScorePartwise
        {
            internal static partial class Part
            {
                internal static partial class Measure
                {
                    internal static partial class Item
                    {
                        internal static partial class Attributes
                        {
                            internal static attributes spAttributes = null;

                            internal static void Parse(attributes attributes)
                            {
                                spAttributes = attributes;

                                spAttributesDivisions = spAttributes.GetDivisions(spAttributesDivisions);
                                spAttributesStaveCount = spAttributes.GetStaveCount(Part.staves.Count);
                                spAttributesStaveCount = spAttributesStaveCount == 0 ? 1 : spAttributesStaveCount;

                                if (Part.staves.Count < spAttributesStaveCount)
                                {
                                    while (Part.staves.Count < spAttributesStaveCount)
                                    {
                                        NSStave._options.Reset();
                                        NSStave._options.id = (Part.staves.Count + 1).ToString();
                                        NSStave._options.index = Part.staves.Count;
                                        NSStave._options.staveEnum = StaveEnum.Undefined;
                                        NSStave._options.background = false;
                                        NSStave._options.systemBracket = false;
                                        NSStave._options.lineDistance = 0;
                                        NSStave._options.width = 0;
                                        NSStave nsStave = nsPart.AddStave(NSStave._options, PivotEnum.MiddleCenter, PoolEnum.NS_FIXED);
                                        Part.staves.Add(nsStave);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
