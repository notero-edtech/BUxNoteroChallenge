/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.SMuFL.Extensions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSLedgerLine : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public void Reset()
            {

            }

            public void CopyValuesFrom(Options o)
            {

            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            vector.vectorEnum = VectorEnum.LineHorizontal;
            vector.lineHorizontal.options.thickness = ns.LineWidth;
            vector.lineHorizontal.options.sidesBlur = ns.LineWidth / 2f;
            vector.lineHorizontal.options.endsBlur = ns.LineWidth / 2f;
            vector.lineHorizontal.options.followRectTransformWidth = false;
            vector.lineHorizontal.options.length = ns.LedgerLineLength;
        }

        public override void Commit()
        {
            base.Commit();
            this.PixelShiftX(-vector.lineHorizontal.options.length / 2f, true);
        }
    }
}
