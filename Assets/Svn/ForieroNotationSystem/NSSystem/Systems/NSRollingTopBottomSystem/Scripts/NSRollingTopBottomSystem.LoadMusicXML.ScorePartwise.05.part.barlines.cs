/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Extensions;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        static partial class ScorePartwise
        {
            internal static partial class Part
            {
                internal static partial class Barlines
                {
                    internal static NSBarLineHorizontal barline = null;
                    internal static NSBarLineNumber barlinenumber = null;

                    internal static void Reset()
                    {
                        barline = null;
                        barlinenumber = null;
                    }

                    internal static void Parse(bool lastBarLine = false)
                    {
                        Reset();
                        NSBarLineHorizontal._options.Reset();

                        var numberShiftY = 0f;
                        var numberAllignment = TextAnchor.MiddleCenter;

                        if (lastBarLine)
                        {
                            switch (NSPlayback.NSRollingPlayback.rollingMode)
                            {
                                case NSPlayback.NSRollingPlayback.RollingMode.Top:
                                    NSBarLineHorizontal._options.barLineEnum = BarLineEnum.HeavyLight;
                                    numberShiftY = -6f * ns.LineWidth;
                                    numberAllignment = TextAnchor.UpperLeft;
                                    break;
                                case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                                    NSBarLineHorizontal._options.barLineEnum = BarLineEnum.LightHeavy;
                                    numberShiftY = 6f * ns.LineWidth;
                                    numberAllignment = TextAnchor.LowerLeft;
                                    break;
                            }
                        }
                        else if (measureIndexer == 0)
                        {
                            switch (NSPlayback.NSRollingPlayback.rollingMode)
                            {
                                case NSPlayback.NSRollingPlayback.RollingMode.Top:
                                    NSBarLineHorizontal._options.barLineEnum = BarLineEnum.LightHeavy;
                                    numberShiftY = -4f * ns.LineWidth;
                                    numberAllignment = TextAnchor.UpperLeft;
                                    break;
                                case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                                    NSBarLineHorizontal._options.barLineEnum = BarLineEnum.HeavyLight;
                                    numberShiftY = 4f * ns.LineWidth;
                                    numberAllignment = TextAnchor.LowerLeft;
                                    break;
                            }
                        }
                        else
                        {
                            NSBarLineHorizontal._options.barLineEnum = BarLineEnum.Regular;
                            switch (NSPlayback.NSRollingPlayback.rollingMode)
                            {
                                case NSPlayback.NSRollingPlayback.RollingMode.Top:
                                    numberAllignment = TextAnchor.UpperLeft;
                                    break;
                                case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                                    numberAllignment = TextAnchor.LowerLeft;
                                    break;
                            }
                        }

                        NSBarLineHorizontal._options.length = 0;
                        NSBarLineHorizontal._options.screenWidth = true;

                        barline = nsPart.AddBarLineHorizontal(NSBarLineHorizontal._options, PivotEnum.MiddleCenter, PoolEnum.NS_MOVABLE);
                        barline.PixelShiftY((lastBarLine ? Part.fbTotalPixels : Part.fbPixels) * directionSign, true);
                        Part.barlineshorizontal.Add(barline);
                        barline.parsing.number = Part.barlineshorizontal.Count();
                        barline.parsing.time = Part.barNumber;
                        barline.SendVisuallyBack(null, true);

                        barlinenumber = nsPart.AddObject<NSBarLineNumber>( PoolEnum.NS_MOVABLE);
                        barlinenumber.options.number = Part.barlineshorizontal.Count();
                        barlinenumber.Commit();

                        barlinenumber.text.SetAlignment(numberAllignment);
                        barlinenumber.text.SetFontSize(NSSettingsStatic.smuflFontSize / 4);
                        barlinenumber.AlignTo(barline, true, true);
                        barlinenumber.rectTransform.SetLeft(5);
                        barlinenumber.PixelShiftY(numberShiftY, true);
                    }
                }
            }
        }
    }
}
