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

namespace ForieroEngine.Music.MusicXML.Dtd
{
    public static partial class MusicXMLDtd
    {
        private static TextAsset _partWiseDtd = null;
        public static TextAsset PartWiseDtd { get { if (_partWiseDtd == null) { _partWiseDtd = Resources.Load<TextAsset>("Dtd/partwise"); } return _partWiseDtd; } }

        private static TextAsset _timeWiseDtd = null;
        public static TextAsset TimeWiseDtd { get { if (_timeWiseDtd == null) { _timeWiseDtd = Resources.Load<TextAsset>("Dtd/timewise"); } return _timeWiseDtd; } }

        private static TextAsset _midiXmlDtd = null;
        public static TextAsset MidiXmlDtd { get { if (_midiXmlDtd == null) { _midiXmlDtd = Resources.Load<TextAsset>("Dtd/midixml"); } return _midiXmlDtd; } }

        private static TextAsset _containerDtd = null;
        public static TextAsset ContainerDtd { get { if (_containerDtd == null) { _containerDtd = Resources.Load<TextAsset>("Dtd/container"); } return _containerDtd; } }
    }
}
