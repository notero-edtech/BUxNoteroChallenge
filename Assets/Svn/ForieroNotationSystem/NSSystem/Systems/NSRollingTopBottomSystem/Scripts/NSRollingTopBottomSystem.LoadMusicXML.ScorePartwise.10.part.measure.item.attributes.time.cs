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
                            internal static class Time
                            {
                                internal static time[] spTimes = null;

                                internal static void Parse(time[] times, PoolEnum poolEnum)
                                {
                                    if (poolEnum == PoolEnum.NS_FIXED) return;

                                    spTimes = times;

                                    if (spTimes == null) return;

                                    for (int timeIndexer = 0; timeIndexer < spTimes.Length; timeIndexer++)
                                    {
                                        var spAttributesTime = spTimes[timeIndexer];
                                        var staveNumber = spAttributesTime.GetStaveNumber();

                                        Part.timeSignatureEnum = spAttributesTime.GetTimeSignatureEnum(); ;
                                        Part.timeSignatureStruct = spAttributesTime.GetTimeSignature();

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
                                            NSTimeSignature._options.Reset();
                                            NSTimeSignature._options.timeSignatureStruct = timeSignatureStruct;
                                            NSTimeSignature._options.timeSignatureEnum = spAttributesTime.GetTimeSignatureEnum();
                                            NSTimeSignature._options.changing = false;
                                            NSTimeSignature timeSignature = Part.staves[i].AddTimeSignature(NSTimeSignature._options, PivotEnum.MiddleCenter, poolEnum);

                                            Part.staves[i].parsing.timeSignature.CopyValuesFrom(NSTimeSignature._options);

                                            timeSignature.passable = true;
                                            timeSignature.pixelTime = Part.fbPixels;
                                            timeSignature.PixelShiftY(timeSignature.pixelTime * directionSign, true);
                                            timeSignature.SetAlpha(NSSettingsStatic.hiddenObjectsAlpha, true);
                                            timeSignature.hidden = true;

                                            if (!Part.staves[i].timeSignature) Part.staves[i].timeSignature = timeSignature;
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
