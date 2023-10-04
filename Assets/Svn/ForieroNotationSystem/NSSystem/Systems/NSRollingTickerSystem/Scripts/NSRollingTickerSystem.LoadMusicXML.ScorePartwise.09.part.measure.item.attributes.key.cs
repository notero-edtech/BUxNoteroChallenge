/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTickerSystem : NS
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
                            internal static class Key
                            {
                                internal static key[] spKeys = null;

                                internal static void Parse(key[] keys, PoolEnum poolEnum)
                                {
                                    spKeys = keys;

                                    if (spKeys == null) return;

                                    for (int keyIndexer = 0; keyIndexer < spKeys.Length; keyIndexer++)
                                    {
                                        var spAttributesKey = spKeys[keyIndexer];
                                        var staveNumber = spAttributesKey.GetStaveNumber();

                                        keySignatureEnum = spAttributesKey.ToNSKeySignatureEnum();
                                        keyModeEnum = spAttributesKey.ToNSKeyModeEnum();

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
                                            NSKeySignature._options.Reset();
                                            NSKeySignature._options.keySignatureEnum = keySignatureEnum;
                                            NSKeySignature._options.keyModeEnum = keyModeEnum;
                                            NSKeySignature._options.changing = measureIndexer > 0;

                                            nsPart.parsing.staves[i].parsing.keySignature.CopyValuesFrom(NSKeySignature._options);

                                            switch (poolEnum)
                                            {
                                                case PoolEnum.NS_MOVABLE:
                                                    {
                                                        var nsStave = nsPart.parsing.staves[i].AddObject<NSStave>( poolEnum);
                                                        nsStave.options.staveEnum = nsPart.parsing.staves[i].options.staveEnum;
                                                        nsStave.options.width = 120f;
                                                        nsStave.options.background = true;
                                                        nsStave.Commit();

                                                        var keySignatureMovable = nsStave.AddKeySignature(NSKeySignature._options, PivotEnum.MiddleCenter, poolEnum);

                                                        var nsClef = nsStave.AddObject<NSClef>( poolEnum);
                                                        nsClef.options.changing = true;
                                                        nsClef.options.clefEnum = nsPart.parsing.staves[i].parsing.clef.clefEnum;
                                                        nsClef.Commit();

                                                        nsClef.PixelShiftX(-nsStave.options.width / 2f + ns.LineSize, true, false);

                                                        nsStave.clef = nsClef;

                                                        keySignatureMovable.passable = true;
                                                        keySignatureMovable.pixelTime = Part.fbPixels;
                                                        keySignatureMovable.Update(nsClef.options.clefEnum);

                                                        keySignatureMovable.PixelShiftX(-ns.LineSize, true, false);

                                                        //nsPanel.SetPositionX(Barlines.barline.GetPositionX(false), true, false);
                                                        nsStave.PixelShiftX(Part.fbPixels, true);

                                                        int dir = i == 0 ? 1 : -1;
                                                        nsStave.PixelShiftY(ns.LineSize * keySignaturesDistance * dir, true);

                                                        if (measureIndexer == 0)
                                                        {
                                                            nsStave.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha, true);
                                                            nsStave.hidden = true;
                                                        }

                                                        keySignatureMovable.stave = nsPart.parsing.staves[i];
                                                    }
                                                    break;
                                                case PoolEnum.NS_FIXED:
                                                    {
                                                        NSKeySignature keySignatureFixed = nsPart.parsing.staves[i].AddKeySignature(NSKeySignature._options, PivotEnum.MiddleCenter, poolEnum);
                                                        keySignatureFixed.passable = false;
                                                        nsPart.parsing.staves[i].keySignature = keySignatureFixed;
                                                        nsPart.parsing.staves[i].Arrange(NSPlayback.NSTickerPlayback.rollingMode.ToHorizontalDirectionEnum());
                                                    }
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
