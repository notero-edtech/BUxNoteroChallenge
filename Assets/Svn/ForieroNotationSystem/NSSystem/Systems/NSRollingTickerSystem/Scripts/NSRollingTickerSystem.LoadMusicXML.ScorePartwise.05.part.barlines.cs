/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTickerSystem : NS
    {
        static partial class ScorePartwise
        {
            internal static partial class Part
            {
                internal static partial class Barlines
                {
                    internal static NSBarLineVertical barline = null;

                    internal static void Parse(bool lastBarLine = false)
                    {
                        NSBarLineVertical._options.Reset();
                        if (lastBarLine)
                        {
                            switch (NSPlayback.NSTickerPlayback.rollingMode)
                            {
                                case NSPlayback.NSTickerPlayback.RollingMode.Left: NSBarLineVertical._options.barLineEnum = BarLineEnum.LightHeavy; break;
                                case NSPlayback.NSTickerPlayback.RollingMode.Right: NSBarLineVertical._options.barLineEnum = BarLineEnum.HeavyLight; break;
                            }

                        }
                        else if (measureIndexer == 0)
                        {
                            switch (NSPlayback.NSTickerPlayback.rollingMode)
                            {
                                case NSPlayback.NSTickerPlayback.RollingMode.Left: NSBarLineVertical._options.barLineEnum = BarLineEnum.HeavyLight; break;
                                case NSPlayback.NSTickerPlayback.RollingMode.Right: NSBarLineVertical._options.barLineEnum = BarLineEnum.LightHeavy; break;
                            }
                        }
                        else
                        {
                            NSBarLineVertical._options.barLineEnum = BarLineEnum.Regular;
                        }

                        NSBarLineVertical._options.length = Mathf.Abs(nsPart.parsing.staves.First().GetPositionY(true) - nsPart.parsing.staves.Last().GetPositionY(true)) + nsPart.parsing.staves.First().topEdge - nsPart.parsing.staves.Last().bottomEdge;
                        NSBarLineVertical._options.screenHeight = false;

                        barline = nsPart.AddBarLineVertical(NSBarLineVertical._options, PivotEnum.TopCenter, PoolEnum.NS_FIXED);
                        barline.PixelShiftX(Part.fbPixels * directionSign, true);
                        Part.barlinesvertical.Add(barline);
                        barline.parsing.number = Part.barlinesvertical.Count();
                        barline.parsing.time = Part.barNumber;
                        barline.SetPositionY(nsPart.parsing.staves.First().GetPositionY(false) + nsPart.parsing.staves.First().topEdge, true, true);
                        barline.SendVisuallyFront(null, true);

                        if(barline.options.barLineEnum != BarLineEnum.LightHeavy) barline.PixelShiftX(-NSSettingsStatic.barlinesPixelShiftX, true);
                    }
                }
            }
        }
    }
}
