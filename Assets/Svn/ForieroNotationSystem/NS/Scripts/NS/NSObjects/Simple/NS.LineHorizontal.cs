/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSLineHorizontal : NSObjectVector
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

        public readonly Options options = new ();
        public static readonly Options _options = new ();

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            vector.vectorEnum = VectorEnum.LineHorizontal;
            vector.lineHorizontal.options.Reset();
            vector.lineHorizontal.options.thickness = ns.BarLineWidth;
            vector.lineHorizontal.options.sidesBlur = ns.BarLineBlur;
            vector.lineHorizontal.options.endsBlur = ns.BarLineBlur;
            vector.lineHorizontal.options.followRectTransformWidth = false;
        }

        public override void Commit()
        {
            base.Commit();

            vector.lineHorizontal.options.length = options.length;
        }
    }
}
