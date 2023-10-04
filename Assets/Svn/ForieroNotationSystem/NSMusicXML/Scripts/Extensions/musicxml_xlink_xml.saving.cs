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
        public static void Save(this scorepartwise spw, Stream stream) { scorepartwiseSerializer.Serialize(stream, spw); stream.Position = 0; }
        public static void Save(this scoretimewise stw, Stream stream) { scoretimewiseSerializer.Serialize(stream, stw); stream.Position = 0; }
        public static void Save(this MIDIFile mf, Stream stream) { midifileSerializer.Serialize(stream, mf); stream.Position = 0; }
        public static void Save(this container c, Stream stream) { containerSerialzer.Serialize(stream, c); stream.Position = 0; }
    }

}
