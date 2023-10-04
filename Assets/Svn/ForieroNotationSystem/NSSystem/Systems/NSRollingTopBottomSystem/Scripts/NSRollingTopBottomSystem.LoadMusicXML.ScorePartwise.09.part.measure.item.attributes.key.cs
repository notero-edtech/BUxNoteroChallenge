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
                            internal static class Key
                            {
                                internal static key[] spKeys = null;

                                internal static void Parse(key[] keys, PoolEnum poolEnum)
                                {
                                    if (poolEnum == PoolEnum.NS_FIXED) return;

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
                                            maxStave = Part.staves.Count - 1;
                                        }

                                        for (int i = minStave; i <= maxStave; i++)
                                        {
                                            NSKeySignature._options.Reset();
                                            NSKeySignature._options.keySignatureEnum = keySignatureEnum;
                                            NSKeySignature._options.keyModeEnum = keyModeEnum;
                                            NSKeySignature._options.changing = false;

                                            Part.staves[i].parsing.keySignature.CopyValuesFrom(NSKeySignature._options);

                                            var nsStave = Part.staves[i].AddObject<NSStave>( poolEnum);
                                            nsStave.options.staveEnum = Part.staves[i].options.staveEnum;
                                            nsStave.options.width = 120f;
                                            nsStave.options.background = true;
                                            nsStave.Commit();

                                            var keySignatureMovable = nsStave.AddKeySignature(NSKeySignature._options, PivotEnum.MiddleCenter, poolEnum);

                                            var nsClef = nsStave.AddObject<NSClef>( poolEnum);
                                            nsClef.options.changing = true;
                                            nsClef.options.clefEnum = Part.staves[i].parsing.clef.clefEnum;
                                            nsClef.Commit();

                                            nsClef.PixelShiftX(-nsStave.options.width / 2f + ns.LineSize, true, false);

                                            nsStave.clef = nsClef;

                                            keySignatureMovable.passable = true;
                                            keySignatureMovable.pixelTime = Part.fbPixels;
                                            keySignatureMovable.Update(nsClef.options.clefEnum);

                                            keySignatureMovable.PixelShiftX(-ns.LineSize, true, false);

                                            nsStave.PixelShiftY(Part.fbPixels * directionSign, true);
                                            nsStave.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha, true);
                                            nsStave.hidden = true;

                                            keySignatureMovable.stave = Part.staves[i];

                                            if (!Part.staves[i].keySignature) staves[i].keySignature = keySignatureMovable;
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

