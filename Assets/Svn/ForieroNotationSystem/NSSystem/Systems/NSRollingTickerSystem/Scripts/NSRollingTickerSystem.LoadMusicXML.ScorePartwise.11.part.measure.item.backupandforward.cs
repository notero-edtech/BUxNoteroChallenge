/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.NotationSystem.Systems
{
    public partial class NSRollingTickerSystem : NS
    {
        static partial class ScorePartwise
        {
            internal static partial class Part
            {
                internal static partial class Measure
                {
                    internal static partial class Item
                    {
                        internal static partial class BackupAndForward
                        {
                            internal static backup spBackup;
                            internal static forward spForward;

                            internal static void Parse(backup backup)
                            {
                                spBackup = backup;

                                if (spBackup != null)
                                {
                                    Part.measureTime.divisions -= spBackup.GetDuration();
                                    Part.measureTime.time -= spBackup.GetTime(spAttributesDivisions, tempo);
                                }
                            }

                            internal static void Parse(forward forward)
                            {
                                spForward = forward;

                                if (spForward != null)
                                {
                                    Part.measureTime.divisions += spForward.GetDuration();
                                    Part.measureTime.time += spForward.GetTime(spAttributesDivisions, tempo);
                                }
                            }

                            internal static void Reset()
                            {
                                spBackup = null;
                                spForward = null;
                            }
                        }
                    }
                }
            }
        }
    }
}
