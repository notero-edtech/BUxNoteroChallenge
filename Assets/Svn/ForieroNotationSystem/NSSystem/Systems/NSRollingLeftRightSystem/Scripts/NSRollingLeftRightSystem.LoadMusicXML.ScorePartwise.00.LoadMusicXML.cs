/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingLeftRightSystem : NS
    {
        private static partial class ScorePartwise
        {
            private static NSRollingLeftRightSystem ns = null;
            private static scorepartwise score = null;
            private static MIDIFile midi = null;
            private static bool parseStaffDistance = false;
            private static int keySignaturesDistance = 8;
            private static int pedalDistance = 5;
            private static Dictionary<int, NSObject> midiOnBuffer = new ();

            private static int partIndexer = 0;
            private static int measureIndexer = 0;
            private static int measureItemsIndexer = 0;
            // if tempo is not present in musicxml then sibelius defaults to 100 and all other softwares to 120 //
            private static float tempo = 100;

            private static float directionSign = 1f;

            public static void LoadMusicXML(NSRollingLeftRightSystem ns, byte[] bytes)
            {
                Reset();

                ScorePartwise.ns = ns;

                ns.parsing.parts = new Dictionary<string, NSPart>();

                float totalTime = 0;
                var watch = Stopwatch.StartNew();

                switch (NSPlayback.NSRollingPlayback.rollingMode)
                {
                    case NSPlayback.NSRollingPlayback.RollingMode.Right: directionSign = -1f; break;
                    case NSPlayback.NSRollingPlayback.RollingMode.Left: directionSign = 1f; break;
                }

                // Destroy all children in case previous load created some NSObjectes //
                ns.DestroyChildren();
                if (NS.debug) Debug.LogFormat("{0} ns.DestroyChildren(true)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

                // Hit Zone //
                var hitZone = ns.AddObject<NSLineVertical>( PoolEnum.NS_FIXED);
                ns.nsBehaviour.hitZoneRT = hitZone.rectTransform;
                hitZone.rectTransform.SetPivotAndAnchors(directionSign > 0 ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f));

                var hitZoneX = 0f;
                if (ns.specificSettings.stretch != Stretch.Auto) { hitZoneX = ns.fixedCanvasRT.rect.size.x / 2f - ns.specificSettings.width / 2f; }
                else { hitZoneX = ns.specificSettings.autoMargin; }
                hitZone.SetPositionX(directionSign * (hitZoneX + ns.specificSettings.hitZoneOffset), true, false);
                hitZone.SetPositionY(-5000, true, false);
                hitZone.options.length = 10000;
                hitZone.Commit();
                
                hitZone.vector.lineVertical.options.thickness = ns.specificSettings.hitZoneWidth;
                hitZone.SetColor(ns.specificSettings.hitZoneColor);
                
                // Creating memorystream from bytes //
                using (var xmlStream = new MemoryStream(bytes))
                {
                    if (NS.debug) Debug.LogFormat("{0} using (MemoryStream xmlStream = new MemoryStream(bytes))", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));
                    //xmlStream.ValidateMusicXml();
                    //if (NS.debug) Debug.LogFormat("{0} xmlStream.ValidateMusicXml()", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));
                    NSPlayback.ScorePartWise = score = score.Load(xmlStream);
                    if (NS.debug) Debug.LogFormat("{0} score.Load(xmlStream)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));
                    //NSPlayback.MidiFile = midi.Load(xmlStream);
                    //if (NS.debug) Debug.LogFormat("{0} score.ToMIDIFile()", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

                    tempo = score.GetDefaultTempo();
                    tempo = score.GetTempoPerQuarterNote(tempo);
                    Part.metronomeMarkOptions = new NSMetronomeMark.Options();
                    Part.metronomeMarkOptions.beatsPerMinute = tempo;
                    score.SetMetronomeMarkOptions(Part.metronomeMarkOptions);

                    Debug.Log("DEFAULT TEMPO PER QUARTER NOTE : " + tempo);

                    // Analyzing measures, beats, total time //
                    NSPlayback.Analyzes.Analyze(score, ns.MinimumDurationWidth);
                    if (NS.debug) Debug.LogFormat("{0}  NSPlayback.Analyzes.Analyze(scorepw, ns.minimumDurationWidth)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

                    // Getting information about the parts and creating NSPart[] //
                    PartList.Parse(score.partlist);
                    if (NS.debug) Debug.LogFormat("{0} PartList.Parse(score.partlist)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

                    for (partIndexer = 0; partIndexer < score.part.Length; partIndexer++)
                    {
                        var spPart = score.part[partIndexer];
                        // Part base information and assigning Part.nsPart variable //
                        Part.Parse(spPart);
                        if (NS.debug) Debug.LogFormat("{0} Part.Parse(nsPart)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

                        for (measureIndexer = 0; measureIndexer < spPart.measure.Length; measureIndexer++)
                        {
                            Part.Measure.Item.BackupAndForward.Reset();

                            var spMeasure = spPart.measure[measureIndexer];
                            // Measure base information and assigning Part.Measure.nsMeasure //
                            Part.Measure.Parse(spMeasure);

                            // We need to set up fixed staves from first measure //
                            if (measureIndexer == 0)
                            {
                                var spAttributes = Part.Measure.spMeasure.Items.ObjectOfType<attributes>();
                                Assert.IsNotNull(spAttributes, "attributes MUST exists in first measure otherwise we don't know how many staves we have for Pool.NSFixed");

                                Part.Measure.Item.Attributes.Parse(spAttributes);
                                Defaults.Parse(score.defaults);

                                spAttributes.clef = spAttributes.clef.SelfOrDefault();
                                Part.Measure.Item.Attributes.Clef.Parse(spAttributes.clef, PoolEnum.NS_FIXED);

                                spAttributes.time = spAttributes.time.SelfOrDefault();
                                Part.Measure.Item.Attributes.Time.Parse(spAttributes.time, PoolEnum.NS_FIXED);

                                spAttributes.key = spAttributes.key.SelfOrDefault();
                                Part.Measure.Item.Attributes.Key.Parse(spAttributes.key.SelfOrDefault(), PoolEnum.NS_FIXED);

                                direction spDirection = Part.Measure.spMeasure.Items.ObjectOfType<direction>();
                                Part.Measure.Item.Direction.Parse(spDirection);

                                Part.nsPart.ArrangeStavesVerticalDistances();
                            }

                            Part.Barlines.Parse(false);

                            for (measureItemsIndexer = 0; measureItemsIndexer < spMeasure.Items.Length; measureItemsIndexer++)
                            {
                                var spMeasureItem = spMeasure.Items[measureItemsIndexer];
                                switch (spMeasureItem)
                                {
                                    case attributes spAttributes:
                                        Part.Measure.Item.Attributes.Parse(spAttributes);
                                        Part.Measure.Item.Attributes.Clef.Parse(spAttributes.clef, PoolEnum.NS_MOVABLE);
                                        Part.Measure.Item.Attributes.Time.Parse(spAttributes.time, PoolEnum.NS_MOVABLE);
                                        Part.Measure.Item.Attributes.Key.Parse(spAttributes.key, PoolEnum.NS_MOVABLE);
                                        break;
                                    case backup spBackup: Part.Measure.Item.BackupAndForward.Parse(spBackup); break;
                                    case barline: break;
                                    case bookmark: break;
                                    case direction item: Part.Measure.Item.Direction.Parse(item); break;
                                    case figuredbass: break;
                                    case forward spForward: Part.Measure.Item.BackupAndForward.Parse(spForward); break;
                                    case grouping: break;
                                    case harmony: break;
                                    case link: break;
                                    case note spNote: Part.Measure.Item.NoteRest.Parse(spNote); break;
                                    case print: break;
                                    case sound: break;
                                }
                            }

                            if (measureIndexer == spPart.measure.Length - 1) Part.Barlines.Parse(true);

                            Part.totalDivisions += Part.measureTime.totalDivisions;
                            Part.totalTime += Part.measureTime.totalTime;
                        }

                        totalTime = Mathf.Max(totalTime, Part.totalTime);
                    }

                    ns.ArrangePartsVerticalDistances(true);
                }

                hitZone.SendVisuallyBack();
                
                Debug.LogFormat("{0} Total musicxml parsing time.)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));
                watch.Stop();

                Assert.IsTrue(totalTime.Approximately(NSPlayback.Time.TotalTime), string.Format("LoadMusicXML totalTime = {0} NSPlayback.totalTime = {1}", totalTime, NSPlayback.Time.TotalTime));
            }

            public static void Reset()
            {
                ns = null;
                score = null;
                parseStaffDistance = false;
                keySignaturesDistance = 8;
                pedalDistance = 5;
                midiOnBuffer = new Dictionary<int, NSObject>();
                for(var i = 0; i<128; i++ ) midiOnBuffer.Add(i, null);

                partIndexer = 0;
                measureIndexer = 0;
                measureItemsIndexer = 0;
                tempo = 100;
            }
        }
    }
}
