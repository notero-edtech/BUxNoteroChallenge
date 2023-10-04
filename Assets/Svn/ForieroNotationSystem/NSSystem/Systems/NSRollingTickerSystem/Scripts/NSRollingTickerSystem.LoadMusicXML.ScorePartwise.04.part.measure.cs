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
                    internal static scorepartwisePartMeasure spMeasure;

                    internal static void Parse(scorepartwisePartMeasure measure)
                    {
                        spMeasure = measure;

                        measureTime = new MusicXML.MeasureTime();
                    }
                }
            }
        }
    }
}
