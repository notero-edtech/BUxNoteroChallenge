/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static double GetPixelsPerSecond(this scoretimewise score, double defaultValue = 100)
        {
            //double greatestDivision = 1;

            //for (int measureIndexer = 0; measureIndexer < score.measure.Length; measureIndexer++)
            //{
            //    var spMeasure = score.measure[measureIndexer];
            //    double spAttributesDivisions = 1.0;

            //    for (int partIndexer = 0; partIndexer < spMeasure.part.Length; partIndexer++)
            //    {
            //        var spPart = spMeasure.part[partIndexer];

            //        var spAttributes = spPart.Items.ObjectOfType<attributes>();
            //        if (spAttributes != null)
            //        {
            //            spAttributesDivisions = spAttributes.GetDivisions(spAttributesDivisions);

            //            greatestDivision = System.Math.Max(greatestDivision, spAttributesDivisions);
            //        }
            //    }
            //}

            return defaultValue;
        }
    }
}
