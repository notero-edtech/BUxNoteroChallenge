/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static int GetStaveNumber(this stafflayout stafflayout, int defaultValue = -1) => stafflayout == null || !int.TryParse(stafflayout.number, out var temp) ? defaultValue : temp - 1;
    }
}
