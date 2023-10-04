/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

using System.Collections.Generic;
using ForieroEngine.Music.MusicXML;
using ForieroEngine.Music.MusicXML.Xsd;
using UnityEngine;
using UnityEngine.Assertions;
using static System.Convert;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static partial class Analyzes
        {
            public static partial class ScorePartwise
            {
                public class Part
                {
                    public readonly string id = "";
                    private float spAttributesDivisions = 1.0f;

                    public Dictionary<int, Measure> measures = new ();

                    public KeySignatureEnum keySignatureEnum = KeySignatureEnum.CMaj_AMin;
                    public KeyModeEnum keyModeEnum = KeyModeEnum.Major;
                    public TimeSignatureStruct timeSignatureStruct = new TimeSignatureStruct(4, 4);
                    public TimeSignatureEnum timeSignatureEnum = TimeSignatureEnum.Normal;

                    public float totalTime = 0;
                    public float totalDivisions = 0;

                    public class Stave { public ClefEnum clefEnum = ClefEnum.Treble; }

                    public List<Stave> staves = new (){ new Stave() };
                    public Part(string id) { this.id = id; }
                    
                    public void ParseMeasures(scorepartwisePartMeasure[] measures)
                    {
                        if (measures == null) return;
                        
                        for (var measureIndexer = 0; measureIndexer < measures.Length; measureIndexer++)
                        {
                            var measureTime = new MeasureTime();

                            var measureIndex = measureIndexer + 1;
                            var spMeasure = measures[measureIndexer];
                                                                
                            for (var measureItemsIndexer = 0; measureItemsIndexer < spMeasure.Items.Length; measureItemsIndexer++)
                            {
                                var spMeasureItem = spMeasure.Items[measureItemsIndexer];

                                switch (spMeasureItem)
                                {
                                    case attributes spAttributes:
                                    {
                                        spAttributesDivisions = spAttributes.GetDivisions(spAttributesDivisions);
                                        var spAttributesStaveCount = spAttributes.GetStaveCount(staves.Count);
                                        spAttributesStaveCount = spAttributesStaveCount == 0 ? 1 : spAttributesStaveCount;

                                        if (staves.Count < spAttributesStaveCount) { while (staves.Count < spAttributesStaveCount) { staves.Add(new Stave()); } }
                                        if (NS.debug) Assert.AreEqual(spAttributesStaveCount, staves.Count, string.Format("spAttributesStaveCount = {0} staves.Count = {1}", spAttributesStaveCount, staves.Count));

                                        ParseClefs(spAttributes.clef);
                                        ParseKeySignatures(spAttributes.key);
                                        ParseTimeSignatures(spAttributes.time);
                                        break;
                                    }
                                    case direction item:
                                        var spDirection = item;
                                        var spMetronome = spDirection.GetMetronome();
                                        TPQN = spMetronome.GetTempoPerQuarterNote(TPQN);
                                        var spSound = spDirection.sound;
                                        TPQN = spSound.GetTempoPerQuarterNote(TPQN);
                                        break;
                                    case forward forward:
                                        measureTime.divisions += forward.GetDuration();
                                        measureTime.time += forward.GetTime(spAttributesDivisions, TPQN);
                                        break;
                                    case backup backup:
                                        measureTime.divisions -= backup.GetDuration();
                                        measureTime.time -= backup.GetTime(spAttributesDivisions, TPQN);
                                        break;
                                    case note spNote:
                                    {
                                        var staveNumber = spNote.GetStaveNumber();
                                        if (NS.debug) Assert.IsTrue(staveNumber >= 0 && staveNumber < staves.Count, string.Format("staveNumber = {0} staves.Count = {1}", staveNumber, staves.Count));
                                        var voiceNumber = spNote.GetVoiceNumber();

                                        var timeSign = this.timeSignatureStruct;

                                        if (spNote.IsCue() || spNote.IsGrace()) { continue; }

                                        var time = spNote.GetTime(spAttributesDivisions, TPQN);
                                        var timePixels = time * NSRollingPlayback.pixelsPerSecond;

                                        shortestTime = Mathf.Min(time, shortestTime);
                                        longestTime = Mathf.Max(time, longestTime);

                                        if (!spNote.IsChord() && !spNote.IsCue() && !spNote.IsGrace())
                                        {
                                            measureTime.divisions += spNote.GetDuration();
                                            measureTime.time += spNote.GetTime(spAttributesDivisions, TPQN);
                                        }

                                        break;
                                    }
                                }
                            }

                            var measure = new Measure(measureIndex, totalTime, this.timeSignatureStruct);
                            var durationPerBeat = spAttributesDivisions * (4.0 / (double)this.timeSignatureStruct.denominator);

                            var numberOfBeatsRest = measureTime.totalDivisions % durationPerBeat;
                            var numberOfBeats = (int)(measureTime.totalDivisions / durationPerBeat) + (numberOfBeatsRest.Approximately(0) ? 0 : 1);

                            if (numberOfBeats == 0) numberOfBeats = 1;

                            var fromBeat = 0;
                            var toBeat = this.timeSignatureStruct.numerator;
                            switch (measureIndexer)
                            {
                                // pickubar //
                                case 0 when measures.Length == 1:
                                    toBeat = numberOfBeats;
                                    break;
                                case 0 when numberOfBeats < this.timeSignatureStruct.numerator:
                                    fromBeat = this.timeSignatureStruct.numerator - numberOfBeats;
                                    break;
                                // ending //
                                case > 0:
                                    toBeat = numberOfBeats;
                                    break;
                            }

                            //Debug.LogFormat(
                            //    "TotalTime : {8}" + " TotalDivisions : {9}" +
                            //" From : {0}" +
                            //" To : {1}" +
                            //" NumberOfBeats : {2}" +
                            //" Measure.Divisions : {3}" +
                            //" Divisions : {4}" +
                            //" DurationPerBeat : {5} " +
                            //" TimeSignature {6}/{7}",
                            //fromBeat,
                            //toBeat,
                            //numberOfBeats,
                            //measureTime.divisions,
                            //spAttributesDivisions,
                            //durationPerBeat,
                            //this.timeSignatureStruct.numerator,
                            //this.timeSignatureStruct.denominator, totalTime, totalDivisions);

                            for (var b = fromBeat; b < toBeat; b++)
                            {
                                var beatIndex = b + 1;
                                var beatNote = new note();
                                beatNote.ItemsElementName = beatNote.ItemsElementName.Add(ItemsChoiceType1.duration);
                                beatNote.Items = new object[1] { ToDecimal(spAttributesDivisions) };

                                var beatTotalTime = beatNote.GetTime(spAttributesDivisions * (measure.timeSignatureStruct.denominator / 4.0f), TPQN);
                                var beatNoteTime = beatTotalTime * (b - fromBeat);
                                var beat = new Beat(beatIndex, measure.time + beatNoteTime, measure);
                                measure.totalTime += beatNoteTime;
                                beat.totalTime = beatTotalTime;

                                if (measure.beats.Count > 0) { measure.beats[measure.beats.Keys.MaxBy(key => key)].nextBeat = beat; }

                                measure.beats.Add(beatIndex, beat);
                            }

                            if (this.measures.Count > 0)
                            {
                                var lastMeasure = this.measures[this.measures.Keys.MaxBy(key => key)];
                                lastMeasure.nextMeasure = measure;

                                lastMeasure.beats[lastMeasure.beats.Keys.MaxBy(key => key)].nextBeat = measure.beats[measure.beats.Keys.MinBy(key => key)];
                            }

                            this.measures.Add(measureIndex, measure);

                            totalTime += measureTime.totalTime;
                            totalDivisions += measureTime.totalDivisions;
                        }
                    }

                    private void ParseClefs(IReadOnlyList<clef> clefs)
                    {
                        if (clefs == null) return;
                        
                        for (var clefIndexer = 0; clefIndexer < clefs.Count; clefIndexer++)
                        {
                            var spAttributesClef = clefs[clefIndexer];
                            var staveNumber = spAttributesClef.GetStaveNumber();
                            var lineNumber = spAttributesClef.GetLineNumber();
                            if (staveNumber != -1) { staves[staveNumber].clefEnum = spAttributesClef.sign.ToNS(lineNumber); }
                            else { for (var i = 0; i < staves.Count; i++) { staves[i].clefEnum = spAttributesClef.sign.ToNS(lineNumber); } }
                        }
                    }

                    private void ParseKeySignatures(IReadOnlyList<key> keys)
                    {
                        if (keys == null) return;
                        
                        for (var keyIndexer = 0; keyIndexer < keys.Count; keyIndexer++)
                        {
                            var spAttributesKey = keys[keyIndexer];
                            var staveNumber = spAttributesKey.GetStaveNumber();
                            this.keySignatureEnum = spAttributesKey.ToNSKeySignatureEnum();
                            this.keyModeEnum = spAttributesKey.ToNSKeyModeEnum();
                        }
                    }

                    private void ParseTimeSignatures(IReadOnlyList<time> times)
                    {
                        if (times == null) return;
                        
                        for (var timeIndexer = 0; timeIndexer < times.Count; timeIndexer++)
                        {
                            var spAttributesTime = times[timeIndexer];
                            var staveNumber = spAttributesTime.GetStaveNumber();
                            this.timeSignatureStruct = spAttributesTime.GetTimeSignature();
                        }
                    }
                }
            }
        }
    }
}
