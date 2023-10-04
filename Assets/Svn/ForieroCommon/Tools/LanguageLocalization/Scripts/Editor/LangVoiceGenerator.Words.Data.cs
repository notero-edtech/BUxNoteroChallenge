using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public partial class LangVoiceGenerator : EditorWindow
{
    partial class WordsClip
    {
        void InitAudio()
        {
            if (clip == null) return;

            renderWidth = window.position.width;
            m_Data.Clear();
            waveforms.Clear();
            GenerateWaveforms();           
            window?.Repaint();
        }

        void InitWords()
        {
            LoadAudioDescription();
        }

        void Save()
        {
            if (!clip)
            {
                Debug.LogError("No audio clip selected!");
                return;
            }


            m_Data = m_Data.OrderBy(o => o.wordIndex).ToList();

            string s = "";

            int tabs = 0;

            s += "<wordsexport audioLength=\"" + clip.length.ToString() + "\" audioDateTime=\"" + "AUDIOTIME" + "\">" + Environment.NewLine;
            tabs++;
            s += "<sentence>" + text + "</sentence>" + Environment.NewLine;
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

            File.WriteAllText(AssetDatabase.GetAssetPath(words), s);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(words));
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

        void LoadAudioDescription()
        {
            if (words)
            {
                XDocument doc = XDocument.Load(GetStreamFromString(words.text));

                XElement wordsExport = doc.Element("wordsexport");

                //XElement sentence = wordsExport.Element("sentence");

                XElement w = wordsExport.Element("words");

                int wordIndex = 0;

                List<XElement> wordElements = new List<XElement>(w.Elements("word"));

                for (int i = 0; i < wordElements.Count; i++)
                {
                    DragObject dragObject = new DragObject(wordElements[i].Value.ToString(), int.Parse(wordElements[i].Attribute("wordIndex").Value), float.Parse(wordElements[i].Attribute("time").Value));
                    dragObject.rowIndex = GetWaveformIndexFormTime(dragObject.time);
                    dragObject.totalTime = clip.length;
                    dragObject.totalRows = GetWaveformIndexFormTime(clip.length) + 1;
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
                Save();               
            }
        }

        void InitAudioDescription()
        {
            if (!clip)
            {
                Debug.LogError("No audio clip selected!");
                return;
            }

            m_Data.Clear();

            List<Lang.Word> ws = Lang.ParseWords(records[recordIndex], Lang.selectedLanguage);

            for (int i = 0; i < ws.Count; i++)
            {
                DragObject dragObject = new DragObject(ws[i].text, i, clip.length / ws.Count * i);
                dragObject.rowIndex = GetWaveformIndexFormTime(dragObject.time);
                dragObject.totalTime = clip.length;
                dragObject.totalRows = GetWaveformIndexFormTime(clip.length) + 1;
                dragObject.playEvent += (data) =>
                {
                    PlayWord(data);
                };
                m_Data.Add(dragObject);
            }
        }

        void GenerateWaveforms()
        {
            if (!clip)
            {
                Debug.LogError("No audio clip selected!");
                return;
            }

            int waveformCount = GetWaveformIndexFormTime(clip.length) + 1;

            float totalWidth = TimeToWidth(clip.length);

            for (int i = 0; i < waveformCount; i++)
            {
                float width = window.position.width - waveformBorder;
                if (totalWidth >= window.position.width - waveformBorder)
                {
                    totalWidth -= window.position.width - waveformBorder;
                }
                else
                {
                    width = totalWidth;
                }

                if (width <= 0) break;

                float from = WidthToTime(i * (window.position.width - waveformBorder));
                float to = from + WidthToTime(width);
                //Texture2D texture = AssetPreview.GetAssetPreview(clip);
                Texture2D texture = AudioWaveform(clip, from, to, Mathf.FloorToInt(width), (int)waveformHeight, Color.white);
                waveforms.Add(texture);
            }
        }
    }
}