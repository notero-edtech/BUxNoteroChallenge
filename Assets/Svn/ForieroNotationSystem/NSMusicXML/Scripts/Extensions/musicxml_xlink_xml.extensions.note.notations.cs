/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Linq;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        #region HAS 
        public static bool HasAccidentalMark(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<accidentalmark>());
        public static bool HasArpeggiate(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<arpeggiate>());
        public static bool HasArticulations(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<articulations>());
        public static bool HasDynamics(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<dynamics>());
        public static bool HasFermata(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<fermata>());
        public static bool HasGlissando(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<glissando>());
        public static bool HasNonArpeggiate(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<nonarpeggiate>());
        public static bool HasOrnaments(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<ornaments>());
        public static bool HasOtherNotation(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<othernotation>());
        public static bool HasSlide(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<slide>());
        public static bool HasSlur(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<slur>());
        public static bool HasTechnical(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<technical>());
        public static bool HasTied(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<tied>());
        public static bool HasTuplet(this notations[] notations) => notations != null && notations.Any(i => i.Items.ContainsType<tuplet>());
        #endregion
    }
}
