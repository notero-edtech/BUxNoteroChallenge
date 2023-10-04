/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
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
        private static TextAsset _musicXmlXsd = null;
        public static TextAsset MusicXmlXsd { get { if (_musicXmlXsd == null) { _musicXmlXsd = Resources.Load<TextAsset>("Xsd/musicxml"); } return _musicXmlXsd; } }
        
        private static TextAsset _xLinkXsd = null;
        public static TextAsset XLinkXsd { get { if (_xLinkXsd == null) { _xLinkXsd = Resources.Load<TextAsset>("Xsd/xlink"); } return _xLinkXsd; } }
        
        static TextAsset _xmlXsd = null;
        public static TextAsset XmlXsd { get { if (_xmlXsd == null) { _xmlXsd = Resources.Load<TextAsset>("Xsd/xml"); } return _xmlXsd; } }
        
        private static TextAsset _midiXmlXsd = null;
        public static TextAsset MidiXmlXsd { get { if (_midiXmlXsd == null) { _midiXmlXsd = Resources.Load<TextAsset>("Xsd/midixml"); } return _midiXmlXsd; } }
        
        private static TextAsset _containerXsd = null;
        public static TextAsset ContainerXsd { get { if (_containerXsd == null) { _containerXsd = Resources.Load<TextAsset>("Xsd/container"); } return _containerXsd; } }
    }
}
