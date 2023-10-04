/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.IO;
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
    public static partial class MusicXMLExtensions
    {
#if UNITY_ANDROID || UNITY_IOS || NETFX_CORE || (UNITY_WSA && ENABLE_IL2CPP)
        public static readonly XmlSerializer scorepartwiseSerializer = new XmlSerializer(typeof(scorepartwise));
        public static readonly XmlSerializer scoretimewiseSerializer = new XmlSerializer(typeof(scoretimewise));
        public static readonly XmlSerializer midifileSerializer = new XmlSerializer(typeof(MIDIFile));
        public static readonly XmlSerializer containerSerialzer = new XmlSerializer(typeof(container));
#else
        public static readonly scorepartwiseSerializer scorepartwiseSerializer = new scorepartwiseSerializer();
        public static readonly scoretimewiseSerializer scoretimewiseSerializer = new scoretimewiseSerializer();
        public static readonly MIDIFileSerializer midifileSerializer = new MIDIFileSerializer();
        public static readonly containerSerializer containerSerialzer = new containerSerializer();
#endif

        public static scorepartwise Load(this scorepartwise spw, MemoryStream stream)
        {
            spw = scorepartwiseSerializer.Deserialize(stream) as scorepartwise;
            stream.Position = 0;
            return spw;
        }
        public static scoretimewise Load(this scoretimewise stw, MemoryStream stream){
            stw = scoretimewiseSerializer.Deserialize(stream) as scoretimewise;
            stream.Position = 0;
            return stw;
        }

        public static MIDIFile Load(this MIDIFile mf, MemoryStream stream)
        {
            mf = midifileSerializer.Deserialize(stream) as MIDIFile;
            stream.Position = 0;
            return mf;
        }

        public static container Load(this container c, MemoryStream stream)
        {
            c = containerSerialzer.Deserialize(stream) as container;
            stream.Position = 0;
            return c;
        }
    }
}
