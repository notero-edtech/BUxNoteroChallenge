/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
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

namespace ForieroEngine.Music.MusicXML.Dtd
{
    public static partial class MusicXMLDtd
    {
        public static bool ValidateScorePartWise(this MemoryStream stream) => Validate(stream, PartWiseDtd.text);
        public static bool ValidateScoreTimeWise(this MemoryStream stream) => Validate(stream, TimeWiseDtd.text);
        public static bool ValidateMidiXml(this MemoryStream stream) => Validate(stream, MidiXmlDtd.text);
        public static bool ValidateContainer(this MemoryStream stream) => Validate(stream, ContainerDtd.text);
        private static bool Validate(MemoryStream stream, string schema)
        {
            var valid = true;
            XmlSchemaSet schemas = new ();
            schemas.Add("", XmlReader.Create(schema));
            XDocument doc = new (stream);
            doc.Validate(schemas, (object sender, ValidationEventArgs e) => { Debug.LogErrorFormat("{0}", e.Message); valid = false; });
            stream.Position = 0;
            return valid;
        }
    }
}
