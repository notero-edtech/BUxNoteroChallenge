/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSLineVertical : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public float length = 0f;

            public void Reset()
            {
                length = 0f;
            }

            public void CopyValuesFrom(Options o)
            {
                length = o.length;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            vector.vectorEnum = VectorEnum.LineVertical;
            vector.lineVertical.options.Reset();
            vector.lineVertical.options.thickness = ns.BarLineWidth;
            vector.lineVertical.options.sidesBlur = ns.BarLineBlur;
            vector.lineVertical.options.endsBlur = ns.BarLineBlur;
            vector.lineVertical.options.followRectTransformHeight = false;
        }

        public override void Commit()
        {
            base.Commit();

            vector.lineVertical.options.length = options.length;
        }
    }
}
