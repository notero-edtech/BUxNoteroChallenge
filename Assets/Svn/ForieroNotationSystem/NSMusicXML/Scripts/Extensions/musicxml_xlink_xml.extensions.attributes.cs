/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static float GetDivisions(this attributes attributes, float defaultValue = 1f)
        {
            if (attributes == null) { return defaultValue; }
            if (attributes.divisionsSpecified) { defaultValue = Convert.ToSingle(attributes.divisions); }
            return defaultValue;
        }

        public static int GetStaveCount(this attributes attributes, int defaultValue = 0)
        {
            if (attributes == null) { return defaultValue; }
            return int.TryParse(attributes.staves, out var temp) ? temp : defaultValue;
        }
    }
}
