/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.Music.MusicXML.Xsd;
using UnityEngine;
using UnityEngine.Assertions;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static partial class Analyzes
        {
            public static partial class ScorePartwise
            {
                public static float shortestTime = float.MaxValue;
                public static float longestTime = float.MinValue;
                public static float totalTime = 0;
                public static float totalDivisions = 0;
                // default sibelius value other apps have 120 //
                public static float defaultTPQN = 100;
                public static float TPQN = 100;
                public static Dictionary<int, Measure> Measures
                {
                    get
                    {
                        Dictionary<int, Measure> r = null;
                        foreach (var part in parts)
                        {
                            if (r != null) { if (part.Value.measures.Count > r.Count) { r = part.Value.measures; } }
                            else { r = part.Value.measures; }
                        }
                        return r;
                    }
                }

                public static Dictionary<string, Part> parts = new ();

                public static void Reset()
                {
                    parts = new ();
                    shortestTime = float.MaxValue;
                    longestTime = float.MinValue;
                    totalTime = 0;
                    totalDivisions = 0;
                    TPQN = 100;
                }

                public static void Analyze(scorepartwise score)
                {
                    Reset();
                    defaultTPQN = score.GetDefaultTempo();
                    TPQN = score.GetTempoPerQuarterNote(defaultTPQN);
                    ParsePartList(score.partlist);
                    ParseParts(score.part);
                }

                public static void ParsePartList(partlist partlist)
                {
                    if (partlist?.Items == null) return;
                    //var groupIndex = 0;
                    //var groupBracket = false;
                    //var groupBarlines = true;

                    for (int partListIndexer = 0; partListIndexer < partlist.Items.Length; partListIndexer++)
                    {
                        var partList = partlist.Items[partListIndexer];
                        if (partList is partgroup partgroup) { }
                        else if (partList is scorepart spScorePart)
                        {
                            var part = new Part(spScorePart.id);
                            parts.Add(part.id, part);
                        }
                    }
                }

                public static void ParseParts(scorepartwisePart[] parts)
                {
                    for (var partIndexer = 0; partIndexer < parts.Length; partIndexer++)
                    {
                        var spPart = parts[partIndexer];
                        Part part = null;
                        var id = string.IsNullOrEmpty(spPart.id) ? partIndexer.ToString() : spPart.id;
                        if (ScorePartwise.parts.ContainsKey(id)) { part = ScorePartwise.parts[spPart.id]; }
                        else { part = new Part(id); ScorePartwise.parts.Add(id, part); }
                        part.ParseMeasures(spPart.measure);
                        totalTime = Mathf.Max(part.totalTime, totalTime);
                        totalDivisions = Mathf.Max(part.totalDivisions, totalDivisions);
                    }
                }
            }
        }
    }
}
