/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;


namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSDurationBarHorizontal : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public AccidentalEnum accidentalEnum = AccidentalEnum.Undefined;
            public float lenght = 100;

            public void Reset()
            {
                accidentalEnum = AccidentalEnum.Undefined;
                lenght = 100;
            }

            public void CopyValuesFrom(Options o)
            {
                accidentalEnum = o.accidentalEnum;
                lenght = o.lenght;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        NSAccidental accidental;

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

            vector.vectorEnum = VectorEnum.LineHorizontal;
            vector.lineHorizontal.options.Reset();
            vector.lineHorizontal.options.followRectTransformWidth = false;
            vector.lineHorizontal.options.thickness = ns.LineSize;
            vector.lineHorizontal.options.length = options.lenght;
            vector.Commit();

            accidental = null;
        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            vector.vectorEnum = VectorEnum.LineHorizontal;
            vector.lineHorizontal.options.followRectTransformWidth = false;
            vector.lineHorizontal.options.thickness = ns.LineSize;
            vector.lineHorizontal.options.length = options.lenght;
            vector.Commit();

            if (options.accidentalEnum != AccidentalEnum.Undefined)
            {
                accidental = AddObject<NSAccidental>(pool);
                accidental.options.accidentalEnum = options.accidentalEnum;
                accidental.Commit();
            }
            
            this.SetVisible(NSDisplaySettings.DurationBars);
        }
    }
}
