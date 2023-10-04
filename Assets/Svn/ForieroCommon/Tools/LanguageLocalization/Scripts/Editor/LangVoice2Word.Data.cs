using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public partial class LangVoice2Word : EditorWindow
{
    void Load(Lang.LangDictionary dictionary, Lang.LanguageCode langCode, string recordId)
    {
        renderWidth = position.width;
        m_Data.Clear();
        waveforms.Clear();
        LoadAudioClip(dictionary, langCode, recordId);
        GenerateWaveforms();
        LoadAudioDescription(dictionary, langCode, recordId);
        Repaint();
    }

    void Save(Lang.LangDictionary dictionary, Lang.LanguageCode langCode, string recordId)
    {
        if (!audioClip)
        {
            Debug.LogError("No audio clip selected!");
            return;
        }


        m_Data = m_Data.OrderBy(o => o.wordIndex).ToList();

        string s = "";

        int tabs = 0;

        s += "<wordsexport audioLength=\"" + audioClip.length.ToString() + "\" audioDateTime=\"" + "AUDIOTIME" + "\">" + Environment.NewLine;
        tabs++;
        s += "<sentence>" + Lang.GetText(dictionary.aliasName, recordId, "") + "</sentence>" + Environment.NewLine;
        s += "<words>" + Environment.NewLine;
        tabs++;
        for (int i = 0; i < m_Data.Count; i++)
        {
            s += "<word time=\"" + m_Data[i].time + "\" wordIndex=\"" + m_Data[i].wordIndex + "\">" + m_Data[i].text + "</word>" + Environment.NewLine;
        }
        tabs--;
        s += "</words>" + Environment.NewLine;
        tabs--;
        s += "</wordsexport>" + Environment.NewLine;

        string filePath = "Assets/Resources/LanguageAudios/" + dictionary.aliasName + "/" + langCode.ToString() + "/" + recordId + ".xml";

        File.WriteAllText(filePath, s);

        AssetDatabase.Refresh();
    }

    void LoadAudioClip(Lang.LangDictionary dictionary, Lang.LanguageCode langCode, string recordId)
    {
        string audioPath = "LanguageAudios/" + dictionary.aliasName + "/" + langCode.ToString() + "/" + recordId;
        audioClip = Resources.Load(audioPath, typeof(AudioClip)) as AudioClip;
    }

    static Stream GetStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    void LoadAudioDescription(Lang.LangDictionary dictionary, Lang.LanguageCode langCode, string recordId)
    {
        string audioPath = "LanguageAudios/" + dictionary.aliasName + "/" + langCode.ToString() + "/" + recordId;
        TextAsset xmlText = Resources.Load(audioPath, typeof(TextAsset)) as TextAsset;

        if (xmlText)
        {
            XDocument doc = XDocument.Load(GetStreamFromString(xmlText.text));

            XElement wordsExport = doc.Element("wordsexport");

            //XElement sentence = wordsExport.Element("sentence");

            XElement words = wordsExport.Element("words");

            int wordIndex = 0;

            List<XElement> wordElements = new List<XElement>(words.Elements("word"));

            for (int i = 0; i < wordElements.Count; i++)
            {
                DragObject dragObject = new DragObject(wordElements[i].Value.ToString(), int.Parse(wordElements[i].Attribute("wordIndex").Value), float.Parse(wordElements[i].Attribute("time").Value));
                dragObject.rowIndex = GetWaveformIndexFormTime(dragObject.time);
                dragObject.totalTime = audioClip.length;
                dragObject.totalRows = GetWaveformIndexFormTime(audioClip.length) + 1;
                dragObject.playEvent += (data) =>
                {
                    PlayWord(data);
                };
                m_Data.Add(dragObject);
                wordIndex++;
            }

        }
        else
        {
            InitAudioDescription();
            Save(dictionary, langCode, recordId);
            Debug.LogWarning("No audio description found! : " + dictionary.aliasName + "|" + langCode.ToString() + "|" + recordId + ".xml");
        }
    }

    void InitAudioDescription()
    {
        if (!audioClip)
        {
            Debug.LogError("No audio clip selected!");
            return;
        }

        m_Data.Clear();

        List<Lang.Word> ws = Lang.ParseWords(records[recordIndex], Lang.selectedLanguage);

        for (int i = 0; i < ws.Count; i++)
        {
            DragObject dragObject = new DragObject(ws[i].text, i, audioClip.length / ws.Count * i);
            dragObject.rowIndex = GetWaveformIndexFormTime(dragObject.time);
            dragObject.totalTime = audioClip.length;
            dragObject.totalRows = GetWaveformIndexFormTime(audioClip.length) + 1;
            dragObject.playEvent += (data) =>
            {
                PlayWord(data);
            };
            m_Data.Add(dragObject);
        }
    }

    void GenerateWaveforms()
    {
        if (!audioClip)
        {
            Debug.LogError("No audio clip selected!");
            return;
        }

        int waveformCount = GetWaveformIndexFormTime(audioClip.length) + 1;

        float totalWidth = TimeToWidth(audioClip.length);

        for (int i = 0; i < waveformCount; i++)
        {
            float width = position.width - waveformBorder;
            if (totalWidth >= position.width - waveformBorder)
            {
                totalWidth -= position.width - waveformBorder;
            }
            else
            {
                width = totalWidth;
            }

            if (width <= 0) break;

            float from = WidthToTime(i * (position.width - waveformBorder));
            float to = from + WidthToTime(width);
            //Texture2D texture = AssetPreview.GetAssetPreview(audioClip);
            Texture2D texture = AudioWaveform(audioClip, from, to, Mathf.FloorToInt(width), (int)waveformHeight, Color.white);
            waveforms.Add(texture);
        }
    }
}