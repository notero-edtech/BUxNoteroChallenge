/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.IO;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

public class NSMetronome
{
    public struct MetronomeEntry
    {
        public double time;
        public int beat;
        public TimeSignatureStruct timeSig;
    }

    public List<MetronomeEntry> entries = new List<MetronomeEntry>();

    scorepartwise scorepw;

    public void LoadMusicXML(byte[] bytes)
    {
        using (MemoryStream xmlStream = new MemoryStream(bytes))
        {

            scorepw = scorepw.Load(xmlStream);

            double pixelsPerSecond = 100f; //TODO!!! NSPlayback.NSRollingPlayback.

            double totalTime = 0;

            TimeSignatureStruct currentTimeSig = new TimeSignatureStruct(4, 4);

            for (int partIndexer = 0; partIndexer < scorepw.part.Length; partIndexer++)
            {
                var spPart = scorepw.part[partIndexer];

                float spAttributesDivisions = 1.0f;

                NSPart._options.Reset();
                NSPart._options.id = string.IsNullOrEmpty(spPart.id) ? partIndexer.ToString() : spPart.id;

                for (int measureIndexer = 0; measureIndexer < spPart.measure.Length; measureIndexer++)
                {
                    var spMeasure = spPart.measure[measureIndexer];

                    int spAttributesStaveCount = 1;

                    #region attributes

                    var spAttributes = spMeasure.Items.ObjectOfType<attributes>();
                    if (spAttributes != null)
                    {

                        spAttributesDivisions = spAttributes.GetDivisions(spAttributesDivisions);
                        spAttributesStaveCount = spAttributes.GetStaveCount();
                        spAttributesStaveCount = spAttributesStaveCount == 0 ? 1 : spAttributesStaveCount;

                        #region time

                        if (spAttributes.time != null)
                        {
                            for (int timeIndexer = 0; timeIndexer < spAttributes.time.Length; timeIndexer++)
                            {
                                var spAttributesTime = spAttributes.time[timeIndexer];
                                var staveNumber = spAttributesTime.GetStaveNumber();

                                currentTimeSig = spAttributesTime.GetTimeSignature();
                            }
                        }

                        #endregion
                    }
                    #endregion

                    direction spDirection = null;
                    metronome spMetronome = null;

                    // if tempo is not present in musicxml then sibelius defaults to 100 and all other softwares to 120 //
                    float tempo = 100.0f;

                    if (spMeasure.Items.ContainsType<direction>())
                    {
                        spDirection = spMeasure.Items.ObjectOfType<direction>();
                        spMetronome = spDirection.GetMetronome();
                    }

                    tempo = spMetronome.GetTempoPerQuarterNote(tempo);

                    int currentBeat = 0;

                    for (int measureItemsIndexer = 0; measureItemsIndexer < spMeasure.Items.Length; measureItemsIndexer++)
                    {
                        var spMeasureItem = spMeasure.Items[measureItemsIndexer];

                        if (spMeasureItem is direction)
                        {
                            spDirection = spMeasureItem as direction;
                            spMetronome = spDirection.GetMetronome();
                            tempo = spMetronome.GetTempoPerQuarterNote(tempo);

                        }
                        else if (spMeasureItem is note)
                        {
                            var spNote = spMeasureItem as note;
                            int staveNumber = spNote.GetStaveNumber();
                            int voiceNumber = spNote.GetVoiceNumber();

                            if (spNote.IsCue() || spNote.IsGrace())
                            {
                                continue;
                            }

                            if (!spNote.IsChord() && !spNote.IsCue() && !spNote.IsGrace())
                            {
                                var entry = new MetronomeEntry()
                                {
                                    time = totalTime,
                                    beat = currentBeat, //TODO !!!  
                                    timeSig = currentTimeSig

                                };
                                entries.Add(entry);

                                totalTime += spNote.GetTime(spAttributesDivisions, tempo);
                            }
                        }
                    }
                }
            }
        }
    }
}
