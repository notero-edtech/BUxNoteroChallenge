/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static clef[] SelfOrDefault(this clef[] clefs) => clefs ??= new clef[1] { ((clef)null).SelfOrDefault() };
        public static clef SelfOrDefault(this clef clef) => clef ??= new clef { line = "2", sign = clefsign.G };
        public static int GetStaveNumber(this clef clef, int defaultValue = -1) => clef == null || !int.TryParse(clef.number, out var temp) ? defaultValue : temp - 1;
        public static int GetLineNumber(this clef clef, int defaultValue = -1) => clef == null || !int.TryParse(clef.line, out var temp) ? defaultValue : temp;
        public static int GetOctaveChange(this clef clef, int defaultValue = 0) => clef == null || !int.TryParse(clef.clefoctavechange, out var temp) ? defaultValue : temp;
    }
}
