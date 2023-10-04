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

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLXsd
    {
        public static bool ValidateMusicXml(this MemoryStream stream)
        {
            var valid = true;

            var schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(MusicXmlXsd.text));
            schemas.Add("", XmlReader.Create(XLinkXsd.text));
            schemas.Add("", XmlReader.Create(MidiXmlXsd.text));
            schemas.Add("", XmlReader.Create(XmlXsd.text));

            var doc = new XDocument(stream);
            doc.Validate(schemas, (object sender, ValidationEventArgs e) =>
            {
                Debug.LogErrorFormat("{0}", e.Message);
                valid = false;
            });
            stream.Position = 0;
            return valid;
        }
    }
}
