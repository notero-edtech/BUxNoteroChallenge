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

namespace ForieroEngine.Music.MusicXML.Xsl
{
    public static partial class MusicXMLXsl
    {
        private static TextAsset _timePartXsl = null;
        public static TextAsset TimePartXsl { get { if (_timePartXsl == null) { _timePartXsl = Resources.Load<TextAsset>("Xsl/timepart"); } return _timePartXsl; } }

        private static TextAsset _partTimeXsl = null;
        public static TextAsset PartTimeXsl { get { if (_partTimeXsl == null) { _partTimeXsl = Resources.Load<TextAsset>("Xsl/parttimes"); } return _partTimeXsl; } }

        private static TextAsset _midiXmlXsl = null;
        public static TextAsset MidiXmlXsl { get { if (_midiXmlXsl == null) { _midiXmlXsl = Resources.Load<TextAsset>("Xsl/midixml"); } return _midiXmlXsl; } }
    }
}
