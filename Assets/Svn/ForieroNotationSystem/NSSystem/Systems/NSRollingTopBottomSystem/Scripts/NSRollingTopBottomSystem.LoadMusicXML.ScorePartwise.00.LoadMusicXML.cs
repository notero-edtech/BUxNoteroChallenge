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
    public partial class NSRollingTopBottomSystem : NS
    {
        static partial class ScorePartwise
        {

            static NSRollingTopBottomSystem ns = null;
            static scorepartwise score = null;
            private static MIDIFile midi = null;
            static bool parseStaffDistance = false;
            static int panelDistance = 6;
            private static Dictionary<int, NSObject> midiOnBuffer = new ();


            static int partIndexer = 0;
            static int measureIndexer = 0;
            static int measureItemsIndexer = 0;
            // if tempo is not present in musicxml then sibelius defaults to 100 and all other softwares to 120 //
            static float tempo = 100;

            static Dictionary<string, NSPart> parts = new Dictionary<string, NSPart>();

            static float directionSign = 1f;

            public static void LoadMusicXML(NSRollingTopBottomSystem ns, byte[] bytes)
            {
                Reset();

                ScorePartwise.ns = ns;

                float totalTime = 0;
                var watch = Stopwatch.StartNew();

                switch (NSPlayback.NSRollingPlayback.rollingMode)
                {
                    case NSPlayback.NSRollingPlayback.RollingMode.Top:
                        directionSign = -1f;
                        break;
                    case NSPlayback.NSRollingPlayback.RollingMode.Bottom:
                        directionSign = 1f;
                        break;
                }

                // Destroy all children in case previous load created some NSObjectes //
                ns.DestroyChildren();
                if (NS.debug) Debug.LogFormat("{0} ns.DestroyChildren(true)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

                var hitZone = ns.AddObject<NSLineHorizontal>( PoolEnum.NS_FIXED);
                ns.nsBehaviour.hitZoneRT = hitZone.rectTransform;
                hitZone.rectTransform.SetPivotAndAnchors(directionSign > 0 ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 1f));
                hitZone.SetPositionX(-5000, true, false);
                hitZone.SetPositionY(directionSign * ns.specificSettings.hitZoneOffset, true, false);
                
                hitZone.options.length = 10000;
                hitZone.Commit();
                
                hitZone.vector.lineHorizontal.options.thickness = ns.specificSettings.hitZoneWidth;
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
                    Part.Measure.Item.Direction.options = new NSMetronomeMark.Options();
                    Part.Measure.Item.Direction.options.beatsPerMinute = tempo;
                    score.SetMetronomeMarkOptions(Part.Measure.Item.Direction.options);

                    // Analyzing measures, beats, total time //
                    NSPlayback.Analyzes.Analyze(score, ns.MinimumDurationWidth);
                    if (NS.debug) Debug.LogFormat("{0}  NSPlayback.Analyzes.Analyze(scorepw, ns.minimumDurationWidth)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

                    // Getting information about the parts and creating NSPart[] //
                    PartList.Parse(score.partlist);
                    if (NS.debug) Debug.LogFormat("{0}   PartList.Parse(score.partlist)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));

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
                            }

                            if (partIndexer == 0) Part.Barlines.Parse(false);

                            for (measureItemsIndexer = 0; measureItemsIndexer < spMeasure.Items.Length; measureItemsIndexer++)
                            {
                                var spMeasureItem = spMeasure.Items[measureItemsIndexer];
                                if (spMeasureItem is attributes)
                                {
                                    var spAttributes = spMeasureItem as attributes;

                                    Part.Measure.Item.Attributes.Parse(spAttributes);
                                    Part.Measure.Item.Attributes.Clef.Parse(spAttributes.clef, PoolEnum.NS_MOVABLE);
                                    Part.Measure.Item.Attributes.Time.Parse(spAttributes.time, PoolEnum.NS_MOVABLE);
                                    Part.Measure.Item.Attributes.Key.Parse(spAttributes.key, PoolEnum.NS_MOVABLE);
                                }
                                if (spMeasureItem is backup)
                                {
                                    Part.Measure.Item.BackupAndForward.Parse(spMeasureItem as backup);
                                }
                                if (spMeasureItem is barline) { }
                                if (spMeasureItem is bookmark) { }
                                if (spMeasureItem is direction)
                                {
                                    Part.Measure.Item.Direction.Parse(spMeasureItem as direction);
                                }
                                if (spMeasureItem is figuredbass) { }
                                if (spMeasureItem is forward)
                                {
                                    Part.Measure.Item.BackupAndForward.Parse(spMeasureItem as forward);
                                }
                                if (spMeasureItem is grouping) { }
                                if (spMeasureItem is harmony) { }
                                if (spMeasureItem is link) { }
                                if (spMeasureItem is note)
                                {
                                    Part.Measure.Item.NoteRest.Parse(spMeasureItem as note);
                                }
                                if (spMeasureItem is print) { }
                                if (spMeasureItem is sound) { }
                            }

                            if (partIndexer == 0 && measureIndexer == spPart.measure.Length - 1) Part.Barlines.Parse(true);

                            Part.totalDivisions += Part.measureTime.totalDivisions;
                            Part.totalTime += Part.measureTime.totalTime;
                        }

                        totalTime = Mathf.Max(totalTime, Part.totalTime);
                    }
                }
                
                hitZone.SendVisuallyBack();
                
                Debug.LogFormat("{0} Total musicxml parsing time.)", watch.Elapsed.TotalSeconds.ToTimeString(@"s\.ff"));
                watch.Stop();

                if (NS.debug)
                {
                    Assert.IsTrue(totalTime.Approximately(NSPlayback.Time.TotalTime), string.Format("LoadMusicXML totalTime = {0} NSPlayback.totalTime = {1}", totalTime, NSPlayback.Time.TotalTime));
                }
            }

            public static void Reset()
            {
                ns = null;
                score = null;
                parseStaffDistance = false;
                panelDistance = 6;
                midiOnBuffer = new Dictionary<int, NSObject>();
                for(var i = 0; i<128; i++ ) midiOnBuffer.Add(i, null);

                partIndexer = 0;
                measureIndexer = 0;
                measureItemsIndexer = 0;
                tempo = 100;

                parts = new Dictionary<string, NSPart>();
            }
        }
    }
}
