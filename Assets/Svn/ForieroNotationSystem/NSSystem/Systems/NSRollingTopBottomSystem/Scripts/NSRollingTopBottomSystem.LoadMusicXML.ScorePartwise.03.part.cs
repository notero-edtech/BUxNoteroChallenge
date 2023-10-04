/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.MusicXML;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        static partial class ScorePartwise
        {
            static partial class Part
            {
                internal static float spAttributesDivisions = 1.0f;
                internal static int spAttributesStaveCount = 0;
                internal static scorepartwisePart spPart = null;
                internal static NSPart nsPart = null;

                internal static List<NSStave> staves = new List<NSStave>();
                internal static List<NSBarLineHorizontal> barlineshorizontal = new List<NSBarLineHorizontal>();
                internal static int barNumber = 0;

                internal static KeySignatureEnum keySignatureEnum = KeySignatureEnum.Undefined;
                internal static KeyModeEnum keyModeEnum = KeyModeEnum.Undefined;
                internal static TimeSignatureEnum timeSignatureEnum = TimeSignatureEnum.Undefined;
                internal static TimeSignatureStruct timeSignatureStruct = new TimeSignatureStruct(4, 4);

                internal static MeasureTime measureTime = new MeasureTime();

                internal static float totalDivisions = 0;
                internal static float totalTime = 0;
                
                internal static float fbTotalPixels => NSPlayback.Time.TotalTime * NSPlayback.NSRollingPlayback.pixelsPerSecond;
                internal static float fbTime => totalTime + measureTime.time;
                internal static float fbPixels => fbTime * NSPlayback.NSRollingPlayback.pixelsPerSecond;
                internal static float fbPreviousTime => totalTime + measureTime.previousTime;
                internal static float fbPreviousPixels => fbPreviousTime * NSPlayback.NSRollingPlayback.pixelsPerSecond;

                internal static void Reset()
                {
                    spAttributesDivisions = 1.0f;
                    spPart = null;
                    nsPart = null;
                    measureTime = new MeasureTime();
                    totalTime = 0;
                    totalDivisions = 0;

                    staves = new List<NSStave>();
                    barlineshorizontal = new List<NSBarLineHorizontal>();
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

                    if (parts.ContainsKey(NSPart._options.id))
                    {
                        nsPart = parts[spPart.id];
                    }
                    else
                    {
                        nsPart = ns.AddPart(NSPart._options, PivotEnum.MiddleCenter, PoolEnum.NS_FIXED);
                        parts.Add(NSPart._options.id, nsPart);
                    }

                    nsPart.options.lineDistance = ns.PartLineDistance;
                }

                public class Voice
                {

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
