/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTopBottomSystem : NS
    {
        static partial class ScorePartwise
        {
            internal static partial class Part
            {
                internal static partial class Measure
                {
                    internal static partial class Item
                    {
                        internal static partial class NoteRest
                        {
                            internal static partial class Rest
                            {
                                internal static rest spRest = null;

                                internal static void Parse(note note)
                                {
                                    spRest = note.GetRest();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
