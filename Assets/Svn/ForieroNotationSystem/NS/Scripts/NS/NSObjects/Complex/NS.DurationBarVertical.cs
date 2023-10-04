/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSDurationBarVertical : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public AccidentalEnum accidentalEnum = AccidentalEnum.Undefined;
            public float lenght = 100;
            public float thickness = 0;

            public void Reset()
            {
                accidentalEnum = AccidentalEnum.Undefined;
                lenght = 100;
                thickness = 0;
            }

            public void CopyValuesFrom(Options o)
            {
                accidentalEnum = o.accidentalEnum;
                lenght = o.lenght;
                thickness = o.thickness;
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

            vector.vectorEnum = VectorEnum.LineVertical;
            vector.lineVertical.options.Reset();
            vector.lineVertical.options.followRectTransformHeight = false;
            vector.lineVertical.options.thickness = Mathf.Approximately(options.thickness, 0f) ? ns.LineSize : options.thickness;
            vector.lineVertical.options.length = options.lenght;
            vector.Commit();

            accidental = null;
        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            vector.lineVertical.options.followRectTransformHeight = false;
            vector.lineVertical.options.thickness = Mathf.Approximately(options.thickness, 0f) ? ns.LineSize : options.thickness;
            vector.lineVertical.options.length = options.lenght;
            vector.Commit();

            if (options.accidentalEnum != AccidentalEnum.Undefined)
            {
                accidental = AddObject<NSAccidental>( pool);
                accidental.options.accidentalEnum = options.accidentalEnum;
                accidental.Commit();
            }
            
            this.SetVisible(NSDisplaySettings.DurationBars);
        }
    }
}
