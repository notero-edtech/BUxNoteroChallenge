/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

public partial class MusicXML2Score : MonoBehaviour
{

    /// DIVISIONS
    /// The duration element is an integer that represents a note’s duration in terms of divisions per quarter note. 
    /// Since our example has 24 divisions per quarter note, a quarter note has a duration of 24. 
    /// The eighth-note triplets have a duration of 8, while the eighth notes have a duration of 12.
    /// 
    /// POSITIONING
    /// The default-x and default-y attributes provide more precise positioning, and use units of tenths of interline space.
    /// The default-x attribute is measured from the start of the current measure (for other elements, it is measured from the left-hand side of the note or the musical position within the bar.)
    /// The default-y attribute is measured from the top barline of the staff. 
    /// 
    /// STEMS
    /// Stem direction is represented with the stem element, whose value can be up, down, none, or double.
    /// For up and down stems, the default-y attribute represents where the stem ends, measured in tenths of interline space from the top line of the staff.
    /// 
    /// BEAMS
    /// Beams are represented by beam elements. Their value can be begin, continue, end, forward hook, and backward hook.
    /// Each element has a beam-level attribute which ranges from 1 to 6 for eighth-note to 256th-note beams.

    public NSBehaviour nsBehaviour;

    public SpacingMode spacingMode = SpacingMode.MusicXML;
    public PlaybackMode playbackMode = PlaybackMode.ScrollingNoRepeats;

    public bool parseSystemDistance = false;
    [Tooltip("Staff line size units.")]
    public float defaultSystemDistance = 5f;

    public bool parseStaffDistance = false;
    [Tooltip("Staff line size units.")]
    public float defaultStaffDistance = 5f;

    /// <summary>
    /// sp = Score Partwise
    /// </summary>
   // scorepartwise sp = null;

    /// <summary>
    /// ns = Notation System
    /// </summary>
    NS ns;

    public void Parse(string xml)
    {
        //sp = null;
        ////Parsing MusicXML//
        //System.Diagnostics.Stopwatch watchParser = System.Diagnostics.Stopwatch.StartNew();
        //sp = MusicXmlParser.GetScore(xml);
        //Debug.Log("MusicXML Parsed in : " + watchParser.ElapsedMilliseconds.ToString() + "ms");
        //watchParser.Stop();

        ////Scoring MusicXML//
        //System.Diagnostics.Stopwatch watchScore = System.Diagnostics.Stopwatch.StartNew();
        //Score();
        //Debug.Log("MusicXML Scored in : " + watchScore.ElapsedMilliseconds.ToString() + "ms");
        //watchScore.Stop();
    }

    void Score()
    {
        //ns = nsBehaviour.ns;

        //if (ns == null)
        //{
        //    Debug.LogError("Notation System object is null!!!");
        //    return;
        //}

        //ns.DestroyChildren();

        //if (sp == null)
        //{
        //    Debug.LogError("Scorepartwise object is null!!!");
        //    return;
        //}

        //if (sp.partlist != null && sp.partlist.Items != null)
        //{
        //    for (int partListIndexer = 0; partListIndexer < sp.partlist.Items.Length; partListIndexer++)
        //    {

        //    }
        //}

        //for (int partIndexer = 0; partIndexer < sp.part.Length; partIndexer++)
        //{
        //var spPart = sp.part[partIndexer];
        //var nsPart = ns.SetPart(string.IsNullOrEmpty(spPart.id) ? partIndexer.ToString() : spPart.id);
        //if (partIndexer == 0)
        //{
        //    nsPart.lnDistanceY = 0;
        //}
        //else
        //{
        //    nsPart.lnDistanceY = defaultSystemDistance;
        //}

        //nsPart.arrangeStaves = false;

        //var spMeasureWidthTotal = 0f;

        //for (int measureIndexer = 0; measureIndexer < spPart.measure.Length; measureIndexer++)
        //{
        //    var spMeasure = spPart.measure[measureIndexer];
        //    var spMeasureWidth = 0f;
        //    if (spMeasure.widthSpecified)
        //    {
        //        spMeasureWidth = (float)spMeasure.width;
        //    }
        //    //				[System.Xml.Serialization.XmlElementAttribute ("attributes", typeof(attributes))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("backup", typeof(backup))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("barline", typeof(barline))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("bookmark", typeof(bookmark))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("direction", typeof(direction))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("figured-bass", typeof(figuredbass))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("forward", typeof(forward))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("grouping", typeof(grouping))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("harmony", typeof(harmony))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("link", typeof(link))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("note", typeof(note))]
        //    //				[System.Xml.Serialization.XmlElementAttribute ("sound", typeof(sound))]
        //    int spAttributesDivisions = -1;
        //    int spAttributesStaves = 1;

        //    #region attributes

        //    var spAttributes = spMeasure.Items.ObjectOfType<attributes>();
        //    if (spAttributes != null)
        //    {

        //        #region divisions

        //        if (spAttributes.divisionsSpecified)
        //        {
        //            spAttributesDivisions = (int)spAttributes.divisions;
        //        }

        //        #endregion

        //        #region staves

        //        if (!string.IsNullOrEmpty(spAttributes.staves))
        //        {
        //            spAttributesStaves = int.Parse(spAttributes.staves);
        //        }

        //        if (nsPart.staves.Count < spAttributesStaves)
        //        {
        //            while (nsPart.staves.Count < spAttributesStaves)
        //            {
        //                nsPart.SetStave(StaveEnum.Five);
        //            }

        //            if (sp.defaults != null)
        //            {
        //                if (sp.defaults.stafflayout != null && parseStaffDistance)
        //                {
        //                    if (sp.defaults.stafflayout.Length == 1 && sp.defaults.stafflayout[0].number == "-1")
        //                    {
        //                        for (int i = 0; i < nsPart.staves.Count; i++)
        //                        {
        //                            if (i > 0)
        //                            {
        //                                nsPart.staves[i].lnDistanceY = (float)sp.defaults.stafflayout[0].staffdistance / 10f;
        //                            }
        //                            nsPart.arrangeStaves = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        for (int i = 0; i < sp.defaults.stafflayout.Length; i++)
        //                        {
        //                            if (i > 0)
        //                            {
        //                                var staffNumber = int.Parse(sp.defaults.stafflayout[i].number) - 1;
        //                                nsPart.staves[staffNumber].lnDistanceY = (float)sp.defaults.stafflayout[i].staffdistance / 10f;
        //                            }
        //                            nsPart.arrangeStaves = true;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < nsPart.staves.Count; i++)
        //                    {
        //                        if (i > 0)
        //                        {
        //                            nsPart.staves[i].lnDistanceY = defaultStaffDistance;
        //                        }
        //                        nsPart.arrangeStaves = true;
        //                    }
        //                }
        //            }
        //        }

        //        #endregion

        //        #region clef

        //        if (spAttributes.clef != null)
        //        {
        //            for (int clefIndexer = 0; clefIndexer < spAttributes.clef.Length; clefIndexer++)
        //            {
        //                var spAttributesClef = spAttributes.clef[clefIndexer];
        //                var staveNumber = 0;
        //                if (!string.IsNullOrEmpty(spAttributesClef.number))
        //                {
        //                    staveNumber = int.Parse(spAttributesClef.number) - 1;
        //                }
        //                nsPart.staves[staveNumber].SetClef(spAttributesClef.sign.ToNS(int.Parse(spAttributesClef.line)));
        //            }
        //        }

        //        #endregion

        //        #region key

        //        if (spAttributes.key != null)
        //        {
        //            for (int keyIndexer = 0; keyIndexer < spAttributes.key.Length; keyIndexer++)
        //            {
        //                var spAttributesKey = spAttributes.key[keyIndexer];

        //                var staveNumber = -1;

        //                if (!string.IsNullOrEmpty(spAttributesKey.number))
        //                {
        //                    staveNumber = int.Parse(spAttributesKey.number) - 1;
        //                }

        //                var fifths = int.Parse(spAttributesKey.ItemsElementName.ValueOf<string>(ItemsChoiceType8.fifths, spAttributesKey.Items));

        //                if (staveNumber != -1)
        //                {
        //                    nsPart.staves[staveNumber].SetKeySignature((KeySignatureEnum)fifths);
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < nsPart.staves.Count; i++)
        //                    {
        //                        nsPart.staves[i].SetKeySignature((KeySignatureEnum)fifths);
        //                    }
        //                }
        //            }
        //        }

        //        #endregion

        //        #region time

        //        if (spAttributes.time != null)
        //        {
        //            for (int timeIndexer = 0; timeIndexer < spAttributes.time.Length; timeIndexer++)
        //            {
        //                var spAttributesTime = spAttributes.time[timeIndexer];

        //                var staveNumber = -1;

        //                if (!string.IsNullOrEmpty(spAttributesTime.number))
        //                {
        //                    staveNumber = int.Parse(spAttributesTime.number) - 1;
        //                }

        //                var beats = int.Parse(spAttributesTime.ItemsElementName.ValueOf<string>(ItemsChoiceType9.beats, spAttributesTime.Items));
        //                var beatsType = int.Parse(spAttributesTime.ItemsElementName.ValueOf<string>(ItemsChoiceType9.beattype, spAttributesTime.Items));

        //                if (staveNumber != -1)
        //                {
        //                    nsPart.staves[staveNumber].SetTimeSignature(beats, beatsType);
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < nsPart.staves.Count; i++)
        //                    {
        //                        nsPart.staves[i].SetTimeSignature(beats, beatsType);
        //                    }
        //                }
        //            }
        //        }

        //        #endregion
        //    }

        //    #endregion

        //    #region print

        //    var spPrint = spMeasure.Items.ObjectOfType<print>();
        //    if (spPrint != null)
        //    {
        //        if (spPrint.stafflayout != null)
        //        {
        //            for (int staffLayoutIndexer = 0; staffLayoutIndexer < spPrint.stafflayout.Length; staffLayoutIndexer++)
        //            {

        //                var spStaffLayout = spPrint.stafflayout[staffLayoutIndexer];

        //                var spStaffLayoutDistance = defaultStaffDistance * 10f;

        //                if (spStaffLayout.number == "-1")
        //                {
        //                    // apply default staff spacing //
        //                }
        //                else
        //                {
        //                    var staveNumber = int.Parse(spStaffLayout.number) - 1;
        //                    if (spStaffLayout.staffdistanceSpecified)
        //                    {
        //                        spStaffLayoutDistance = parseStaffDistance ? (float)spStaffLayout.staffdistance : defaultStaffDistance * 10f;
        //                        if (staveNumber > 0)
        //                        {
        //                            nsPart.staves[staveNumber].lnDistanceY = spStaffLayoutDistance / 10f;
        //                            nsPart.arrangeStaves = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    #endregion

        //    if (nsPart.arrangeStaves)
        //    {
        //        nsPart.ArrangeStaves();
        //    }

        //    for (int measureItemsIndexer = 0; measureItemsIndexer < spMeasure.Items.Length; measureItemsIndexer++)
        //    {
        //        var spMeasureItem = spMeasure.Items[measureItemsIndexer];

        //        if (measureItemsIndexer == 0)
        //        {
        //            for (int i = 0; i < nsPart.staves.Count; i++)
        //            {
        //                nsPart.staves[i].AddBarLine(BarLineEnum.HeavyLight);
        //            }
        //        }

        //        if (spMeasureItem is note)
        //        {
        //            var spNote = spMeasureItem as note;
        //            var staveNumber = int.Parse(spNote.staff) - 1;
        //            if (spNote.IsRest())
        //            {
        //                rest spRest = spNote.ItemsElementName.ValueOf<rest>(ItemsChoiceType1.rest, spNote.Items);

        //                NSRest nsRest = null;

        //                if (spRest.measureSpecified && spRest.measure == yesno.yes)
        //                {
        //                    // difficulties ... rest can be specified as whole measure
        //                    // actually without and type value 
        //                    // so what is needed is to convert time signature => rest 
        //                    // can be dotted like in 3/4 time signature 
        //                    nsRest = nsPart.staves[staveNumber].AddRest(RestEnum.Quarter);
        //                    nsRest.Commit();
        //                }
        //                else
        //                {
        //                    nsRest = nsPart.staves[staveNumber].AddRest(spNote.ToRestEnum());
        //                    nsRest.Commit();
        //                }

        //                if (spNote.defaultxSpecified)
        //                {
        //                    nsRest.PixelShiftX(ns.lineSize * (spMeasureWidthTotal + (float)spNote.defaultx) / 10f, true);
        //                }
        //            }
        //            else
        //            {
        //                NSNote nsNote = nsPart.staves[staveNumber].AddNote(spNote.ToNoteEnum());

        //                bool chord = spNote.ItemsElementName.Contains(ItemsChoiceType1.chord);

        //                nsNote.stemEnum = StemEnum.Undefined;

        //                if (spNote.stem != null && !chord)
        //                {
        //                    nsNote.stemEnum = spNote.stem.Value.ToNS();
        //                }

        //                if (spNote.beam != null)
        //                {

        //                }

        //                pitch spPitch = spNote.ItemsElementName.ValueOf<pitch>(ItemsChoiceType1.pitch, spNote.Items);
        //                if (spPitch != null)
        //                {
        //                    if (spPitch.alterSpecified)
        //                    {

        //                    }

        //                    float y = nsPart.staves[staveNumber].GetObjectPosition((int)spPitch.step.ToNS(), int.Parse(spPitch.octave));
        //                    nsNote.SetPositionY(y, true, true);
        //                }

        //                nsNote.Commit();

        //                if (spNote.defaultxSpecified)
        //                {
        //                    nsNote.PixelShiftX(ns.lineSize * (spMeasureWidthTotal + (float)spNote.defaultx) / 10f, true);
        //                }
        //            }
        //        }
        //        else if (spMeasureItem is sound)
        //        {

        //        }
        //    }

        //    spMeasureWidthTotal += spMeasureWidth;

        //    for (int i = 0; i < nsPart.staves.Count; i++)
        //    {
        //        var stave = nsPart.staves[i];
        //        var barline = stave.AddBarLine(BarLineEnum.Regular);
        //        barline.Commit();
        //        barline.PixelShiftX(ns.lineSize * spMeasureWidthTotal / 10f, true);
        //    }
        //}

        //ns.ArrangeParts();
        //}
    }
}
