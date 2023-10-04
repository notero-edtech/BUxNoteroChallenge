/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingLeftRightSystem : NS
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
                                            maxStave = nsPart.parsing.staves.Count - 1;
                                        }

                                        for (int i = minStave; i <= maxStave; i++)
                                        {
                                            NSClef._options.Reset();
                                            NSClef._options.clefEnum = spAttributesClef.sign.ToNS(lineNumber);
                                            NSClef._options.staveLine = lineNumber;
                                            NSClef._options.octaveChange = octaveChange;
                                            NSClef._options.changing = Part.fbPixels > 0;
                                            NSClef clef = nsPart.parsing.staves[i].AddClef(NSClef._options, PivotEnum.MiddleCenter, poolEnum);

                                            nsPart.parsing.staves[i].parsing.clef.CopyValuesFrom(NSClef._options);

                                            switch (poolEnum)
                                            {
                                                case PoolEnum.NS_MOVABLE:
                                                    {
                                                        clef.passable = true;
                                                        clef.pixelTime = Part.fbPixels - ns.LineSize * 1.6f;
                                                        clef.PixelShiftX(clef.pixelTime, true);
                                                        if (!NSClef._options.changing)
                                                        {
                                                            clef.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha, true);
                                                            clef.hidden = true;
                                                        }
                                                    }
                                                    break;
                                                case PoolEnum.NS_FIXED:
                                                    clef.passable = false;
                                                    nsPart.parsing.staves[i].clef = clef;
                                                    nsPart.parsing.staves[i].Arrange(NSPlayback.NSRollingPlayback.rollingMode.ToHorizontalDirectionEnum());
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
        }
    }
}
