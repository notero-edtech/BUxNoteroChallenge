/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Music.MusicXML;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using Sanford.Multimedia.Timers;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingLeftRightSystem : NS
    {
        private static partial class ScorePartwise
        {
            internal static partial class Part
            {
                private static float spAttributesDivisions = 1.0f;
                private static int spAttributesStaveCount = 0;
                private static scorepartwisePart spPart = null;
                internal static NSPart nsPart = null;

                internal static NSMetronomeMark.Options metronomeMarkOptions = new NSMetronomeMark.Options();
                private static List<NSBarLineVertical> barlinesvertical = new List<NSBarLineVertical>();
                private static int barNumber = 0;

                private static KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined;
                private static KeyModeEnum keyModeEnum = KeyModeEnum.Undefined;
                private static TimeSignatureEnum timeSignatureEnum = TimeSignatureEnum.Undefined;
                private static TimeSignatureStruct timeSignatureStruct = new TimeSignatureStruct(4, 4);

                internal static MeasureTime measureTime = new MeasureTime();

                internal static float totalDivisions = 0;
                internal static float totalTime = 0;
                private static float fbTotalPixels => NSPlayback.Time.TotalTime
                                                      * NSPlayback.NSRollingPlayback.pixelsPerSecond
                                                      * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f);
                private static float fbTime => totalTime + measureTime.time;
                // total forward backward pixels for positionin //
                private static float fbPixels => fbTime 
                                                 * NSPlayback.NSRollingPlayback.pixelsPerSecond
                                                 * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f);

                private static float fbPreviousTime => totalTime + measureTime.previousTime;
                private static float fbPreviousPixels => fbPreviousTime
                                                         * NSPlayback.NSRollingPlayback.pixelsPerSecond
                                                         * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? 1f / NSPlayback.Zoom : 1f);
                
                private static void Reset()
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
