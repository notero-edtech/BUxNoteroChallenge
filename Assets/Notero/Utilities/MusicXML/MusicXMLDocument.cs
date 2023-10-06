using System;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Notero.Utilities.MusicXML
{
    public class MusicXMLDocument
    {
        XmlDocument doc = new();

        public MusicXMLDocument LoadXML(string text)
        {
            doc.LoadXml(text);
            return this;
        }

        public MusicXMLDocument Load(string filePath)
        {
            doc.Load(filePath);
            return this;
        }

        public override string ToString()
        {
            StringWriter sw = new();
            XmlTextWriter xw = new(sw);
            doc.WriteTo(xw);
            return sw.ToString();
        }

        public TextAsset ToTextAsset()
        {
            return new TextAsset(ToString());
        }

        public MusicXMLDocument ModifyBPM(float bpm)
        {
            ModifyMetronomeMark(bpm);
            ModifySoundTempo(bpm);
            return this;
        }

        void ModifySoundTempo(float tempo)
        {
            // TODO: modify multiple sound tempos. 
            var measure = GetMeasure(1);
            var tag = measure.GetElementsByTagName("sound");

            if(tag.Count > 0)
            {
                var node = tag[0];
                node.Attributes.GetNamedItem("tempo").Value = tempo.ToString();
            }
        }

        void ModifyMetronomeMark(float bpm)
        {
            // TODO: modify multiple metronome marks. 
            var measure = GetMeasure(1);
            var tag = measure.GetElementsByTagName("per-minute");
            var bpmAfterCheck = (float)System.Math.Round(CheckMetronomeMark(bpm), 1);

            if(tag.Count == 0)
            {
                InsertMetronomeMark(bpmAfterCheck);
            }
            else
            {
                var node = tag[0];
                node.InnerText = bpmAfterCheck.ToString();
            }
        }

        private float CheckMetronomeMark(float bpm)
        {
            var measure = GetMeasure(1);
            var beatUnit = measure.GetElementsByTagName("beat-unit");
            var beatUnitDot = measure.GetElementsByTagName("beat-unit-dot");
            var metronomeBPM = bpm;

            switch(beatUnit[0].InnerText)
            {
                case "eighth":
                    metronomeBPM = bpm * 2;
                    break;
                case "quarter":
                    metronomeBPM = bpm;
                    break;
                case "half":
                    metronomeBPM = bpm / 2;
                    break;
            }

            // for dotted quarter metronome mark
            if(beatUnitDot.Count > 0)
            {
                metronomeBPM = bpm / 3 * 2;
            }

            return metronomeBPM;
        }

        void InsertSoundTempo(float tempo, int measureNumber = 1)
        {
            var _tempo = doc.CreateAttribute("tempo", "");
            _tempo.Value = tempo.ToString();

            var soundNode = doc.CreateNode(XmlNodeType.Element, "sound", "");
            soundNode.Attributes.SetNamedItem(_tempo);

            var directionNode = GetMeasureDirection(measureNumber);
            directionNode.AppendChild(soundNode);
        }

        void InsertMetronomeMark(float bpm, int measureNumber = 1)
        {
            var beatUnit = doc.CreateNode(XmlNodeType.Element, "beat-unit", "");
            beatUnit.InnerText = "quarter";

            var perMinute = doc.CreateNode(XmlNodeType.Element, "per-minute", "");
            perMinute.InnerText = bpm.ToString();

            var metronome = doc.CreateNode(XmlNodeType.Element, "metronome", "");
            metronome.AppendChild(beatUnit);
            metronome.AppendChild(perMinute);

            var directionType = doc.CreateNode(XmlNodeType.Element, "direction-type", "");
            directionType.AppendChild(metronome);

            var directionNode = GetMeasureDirection(measureNumber);
            directionNode.AppendChild(directionType);

            InsertBeatUnitDot(measureNumber);
        }

        /// <summary>
        /// Get the measure at number = measureNumber (not index, measureNumber = index + 1).
        /// </summary>
        /// <param name="measureNumber"></param>
        /// <returns></returns>
        XmlElement GetMeasure(int measureNumber)
        {
            var measureList = doc.GetElementsByTagName("measure");

            if(measureNumber > measureList.Count)
            {
                throw new Exception("measure number larger than listed.");
            }

            return measureList[measureNumber - 1] as XmlElement;
        }

        XmlElement GetMeasureDirection(int measureNumber = 1)
        {
            var measure = GetMeasure(measureNumber);
            var list = measure.GetElementsByTagName("direction");

            if(list.Count > 0)
            {
                return list[measureNumber - 1] as XmlElement;
            }

            var direction = doc.CreateNode(XmlNodeType.Element, "direction", "");
            var attributes = measure.GetElementsByTagName("attributes")[0];
            measure.InsertAfter(direction, attributes);

            return measure.GetElementsByTagName("direction")[0] as XmlElement;
        }

        /// <summary>
        /// If the time signature is compound (6/8, 9/8, 12/8, etc.),
        /// the beat type will be dotted quarter note.
        /// </summary>
        /// <param name="measureNumber"></param>
        void InsertBeatUnitDot(int measureNumber)
        {
            var measure = GetMeasure(measureNumber);
            var beats = float.Parse(measure.GetElementsByTagName("beats")[0].InnerText);
            var beatType = float.Parse(measure.GetElementsByTagName("beat-type")[0].InnerText);

            if(beatType == 8 && beats % 3 == 0 && beats / 3 > 1)
            {
                var dotted = doc.CreateNode(XmlNodeType.Element, "beat-unit-dot", "");
                var beatUnit = measure.GetElementsByTagName("beat-unit")[0];
                var metronome = measure.GetElementsByTagName("metronome")[0];

                metronome.InsertAfter(dotted, beatUnit);
            }
        }
    }
}
