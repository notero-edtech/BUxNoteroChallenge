/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static bool HasParenthesis(this accidental accidental)
        {
            if (accidental is not { parenthesesSpecified: true }) { return false; }
            return accidental.parentheses == yesno.yes;
        }
    }
}
