/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.MusicXML;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSPageLayoutTickerSystem : NS
    {
        static partial class ScorePartwise
        {
            static partial class Part
            {
                internal static float spAttributesDivisions = 1.0f;
                internal static int spAttributesStaveCount = 0;
                internal static scorepartwisePart spPart = null;
                internal static NSPart nsPart = null;

                internal static NSMetronomeMark.Options metronomeMarkOptions = new NSMetronomeMark.Options();
                internal static List<NSBarLineVertical> barlinesvertical = new List<NSBarLineVertical>();
                internal static int barNumber = 0;

                internal static KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined;
                internal static KeyModeEnum keyModeEnum = KeyModeEnum.Undefined;
                internal static TimeSignatureEnum timeSignatureEnum = TimeSignatureEnum.Undefined;
                internal static TimeSignatureStruct timeSignatureStruct = new TimeSignatureStruct(4, 4);

                internal static MeasureTime measureTime = new MeasureTime();

                internal static float totalDivisions = 0;
                internal static float totalTime = 0;
                // total forward backward pixels for positionin //
                internal static float fbPixels {
                    get {
                        return totalTime
                            * NSPlayback.NSRollingPlayback.pixelsPerSecond
                            * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f)
                            + measureTime.time
                            * NSPlayback.NSRollingPlayback.pixelsPerSecond
                            * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f);
                    }
                }

                internal static float fbPreviousPixels
                {
                    get {
                        return totalTime
                            * NSPlayback.NSRollingPlayback.pixelsPerSecond
                            * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f)
                            + measureTime.previousTime
                            * NSPlayback.NSRollingPlayback.pixelsPerSecond
                            * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f);
                    }
                }

                internal static void Reset()
                {
                    spAttributesDivisions = 1.0f;
                    spAttributesStaveCount = 0;
                    spPart = null;
                    nsPart = null;
                    measureTime = new MeasureTime();
                    totalTime = 0;
                    totalDivisions = 0;

                    barlinesvertical = new List<NSBarLineVertical>();
                    barNumber = 0;

                    keySignatureEnum = KeySignatureEnum.Undefined;
                    keyModeEnum = KeyModeEnum.Undefined;
                    timeSignatureEnum = TimeSignatureEnum.Undefined;
                    timeSignatureStruct = new TimeSignatureStruct(4, 4);

                    voices = new List<Voice>(new Voice[8] {
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice ()
                        });
                }

                internal static void Parse(scorepartwisePart part)
                {
                    Reset();

                    spPart = part;

                    NSPart._options.Reset();
                    NSPart._options.id = string.IsNullOrEmpty(spPart.id) ? partIndexer.ToString() : spPart.id;
                    NSPart._options.bracket = true;

                    if (ns.parsing.parts.ContainsKey(NSPart._options.id))
                    {
                        nsPart = ns.parsing.parts[spPart.id];
                    }
                    else
                    {
                        nsPart = ns.AddPart(NSPart._options, PivotEnum.MiddleCenter, PoolEnum.NS_FIXED);
                        ns.parsing.parts.Add(NSPart._options.id, nsPart);
                    }

                    nsPart.options.lineDistance = ns.PartLineDistance;
                }

                public class Voice
                {
                    public List<NSNote> tiedNotes = new List<NSNote>();
                    public NSDurationBarHorizontal durationBar;
                }

                internal static List<Voice> voices = new List<Voice>(new Voice[8] {
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice (),
                        new Voice ()
                    });
            }
        }
    }
}
