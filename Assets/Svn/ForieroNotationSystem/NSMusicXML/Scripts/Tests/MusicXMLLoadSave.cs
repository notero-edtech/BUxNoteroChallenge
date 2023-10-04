/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.IO;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.MusicXML.Xsl;
using UnityEngine;
using System.Diagnostics;
using TMPro;

public class MusicXMLLoadSave : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextAsset xml;
    public string fileName = "serizlized.xml";

    scorepartwise scorePW;
    scoretimewise scoreTW;
        
    public void CreateScore()
    {
        var watch = Stopwatch.StartNew();
        scorePW = new scorepartwise();
        scorePW.part = new scorepartwisePart[1];
        scorepartwisePart part = scorePW.part[0] = new scorepartwisePart();
        part.measure = new scorepartwisePartMeasure[2];
        part.measure[0] = new scorepartwisePartMeasure();
        part.measure[1] = new scorepartwisePartMeasure();
        text.text = watch.ElapsedMilliseconds.ToString();
    }

    public void LoadXML()
    {
        var watch = Stopwatch.StartNew();
        using (MemoryStream xmlStream = new MemoryStream(xml.bytes))
        {
            scorePW = scorePW.Load(xmlStream);
        }
        text.text = watch.ElapsedMilliseconds.ToString();
    }

    public void Save()
    {
        string f = "pw_" + fileName;

        if (scorePW != null)
        {
            if (File.Exists(f))
            {
                File.Delete(f);
            }

            using (FileStream xmlStream = new FileStream(f, FileMode.CreateNew, FileAccess.Write))
            {
                scorePW.Save(xmlStream);
            }
        }

        f = "tw_" + fileName;

        if (scoreTW != null)
        {
            if (File.Exists(f))
            {
                File.Delete(f);
            }

            using (FileStream xmlStream = new FileStream(f, FileMode.CreateNew, FileAccess.Write))
            {
                scoreTW.Save(xmlStream);
            }
        }
    }

    public void TransformToPartwise()
    {
        var watch = Stopwatch.StartNew();
        if (scoreTW != null)
        {
            scorePW = scoreTW.ToScorePartWise();
        }
        text.text = watch.ElapsedMilliseconds.ToString();
    }

    public void TransformToTimewise()
    {
        var watch = Stopwatch.StartNew();
        if (scorePW != null)
        {
            scoreTW = scorePW.ToScoreTimeWise();
        }
        text.text = watch.ElapsedMilliseconds.ToString();
    }
}
