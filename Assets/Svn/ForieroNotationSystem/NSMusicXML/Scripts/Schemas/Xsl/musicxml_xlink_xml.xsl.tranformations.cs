/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.IO;
using System.Xml;
#if UNITY_ANDROID || UNITY_IOS || (UNITY_WSA && ENABLE_IL2CPP)
using System.Xml.Serialization;
using System.Xml.Xsl;
#elif UNITY_WSA && NETFX_CORE
using System.Xml.Serialization;
using Windows.Data.Xml.Xsl; 
#else
using System.Xml.Serialization;
using System.Xml.Xsl;
using Microsoft.Xml.Serialization.GeneratedAssembly;
#endif

using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.MusicXML.Xsl
{
    public static partial class MusicXMLXsl
    {
        private static T ConvertTo<T, TP>(this TP p, XmlSerializer serializer, string xmlString, string xslString) where T : class where TP : class
        {
            T r = null;
#if NETFX_CORE
            // Load xslt content
            var xsltDoc = new Windows.Data.Xml.Dom.XmlDocument();
            xsltDoc.LoadXml(xslString);

            var doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(xmlString);            

            var xsltProcessor = new Windows.Data.Xml.Xsl.XsltProcessor(xsltDoc);
            string transformedStr = xsltProcessor.TransformToString(doc);

            // Load xml content
            doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(transformedStr);

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(transformedStr);
            writer.Flush();
            stream.Position = 0;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            r = serializer.Deserialize(stream) as T;
            stream.Dispose();            
#else
            var xsl = new XslCompiledTransform();

            using (var sr = new StringReader(xslString)) { using (var xr = XmlReader.Create(sr)) { xsl.Load(xr); } }
            using (var sr = new StringReader(xmlString))
            {
                using (var xr = XmlReader.Create(sr))
                {
                    using (var stream = new MemoryStream())
                    {
                        xsl.Transform(xr, null, stream);
                        stream.Position = 0;
                        r = serializer.Deserialize(stream) as T;
                    }
                }
            }
#endif
            return r;
        }
        
        public static scorepartwise ToScorePartWise(this scoretimewise score)
        {
            var xmlString = score.ToXMLString();
            var xslString = PartTimeXsl.text;
            return ConvertTo<scorepartwise, scoretimewise>(score, MusicXMLExtensions.scorepartwiseSerializer,  xmlString, xslString);
        }

        public static scoretimewise ToScoreTimeWise(this scorepartwise score)
        {
            var xmlString = score.ToXMLString();
            var xslString = TimePartXsl.text;
            return ConvertTo<scoretimewise, scorepartwise>(score, MusicXMLExtensions.scoretimewiseSerializer, xmlString, xslString);
        }

        public static MIDIFile ToMIDIFileAbsoluteTimeStamp(this MIDIFile midi)
        {
            var xmlString = midi.ToXMLString();
            var xslString = MidiXmlXsl.text;
            return ConvertTo<MIDIFile, MIDIFile>(midi, MusicXMLExtensions.midifileSerializer, xmlString, xslString);
        }
    }
}
