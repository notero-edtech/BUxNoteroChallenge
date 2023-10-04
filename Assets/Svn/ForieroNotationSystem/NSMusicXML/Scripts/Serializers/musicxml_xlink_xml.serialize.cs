/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.IO;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static class MusicXMLSerialize
    {
        public static string ToXMLString(this scoretimewise score)
        {
            using var stream = new MemoryStream();
            score.Save(stream);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();;
        }

        public static string ToXMLString(this scorepartwise score)
        {
            using var stream = new MemoryStream();
            score.Save(stream);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();;
        }

        public static string ToXMLString(this MIDIFile midi)
        {
            using var stream = new MemoryStream();
            midi.Save(stream);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();;
        }
        
        public static MemoryStream ToXMLStream(this scoretimewise score)
        {
            using var stream = new MemoryStream();
            score.Save(stream);
            return stream;
        }

        public static MemoryStream ToXMLStream(this scorepartwise score)
        {
            using var stream = new MemoryStream();
            score.Save(stream);
            return stream;
        }

        public static MemoryStream ToXMLStream(this MIDIFile midi)
        {
            using var stream = new MemoryStream();
            midi.Save(stream);
            return stream;
        }
    }
}
