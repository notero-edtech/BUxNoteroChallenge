/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
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
                            internal static class Clef
                            {
                                internal static clef[] spClefs = null;

                                internal static void Parse(clef[] clefs, PoolEnum poolEnum)
                                {
                                    if (poolEnum == PoolEnum.NS_FIXED) return;

                                    spClefs = clefs;

                                    if (spClefs == null) return;

                                    for (int clefIndexer = 0; clefIndexer < spClefs.Length; clefIndexer++)
                                    {
                                        var spAttributesClef = spClefs[clefIndexer];

                                        var staveNumber = spAttributesClef.GetStaveNumber(measureIndexer == 0 ? clefIndexer - 1 : -1);
                                        var lineNumber = spAttributesClef.GetLineNumber(1);
                                        var octaveChange = spAttributesClef.GetOctaveChange(0);

                                        int minStave, maxStave;

                                        if (staveNumber != -1)
                                        {
                                            minStave = staveNumber;
                                            maxStave = staveNumber;
                                        }
                                        else
                                        {
                                            minStave = 0;
                                            maxStave = Part.staves.Count - 1;
                                        }

                                        for (int i = minStave; i <= maxStave; i++)
                                        {
                                            NSClef._options.Reset();
                                            NSClef._options.clefEnum = spAttributesClef.sign.ToNS(lineNumber);
                                            NSClef._options.staveLine = lineNumber;
                                            NSClef._options.octaveChange = octaveChange;
                                            NSClef._options.changing = false;
                                            NSClef clef = Part.staves[i].AddClef(NSClef._options, PivotEnum.MiddleCenter, poolEnum);

                                            Part.staves[i].parsing.clef.CopyValuesFrom(NSClef._options);

                                            clef.passable = true;
                                            clef.pixelTime = Part.fbPixels;
                                            clef.PixelShiftY(clef.pixelTime * directionSign, true);
                                            clef.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha, true);
                                            clef.hidden = true;

                                            if (!Part.staves[i].clef) Part.staves[i].clef = clef;
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
}

