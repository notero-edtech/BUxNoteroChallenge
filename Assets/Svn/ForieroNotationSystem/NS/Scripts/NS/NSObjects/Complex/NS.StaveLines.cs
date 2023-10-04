/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSStaveLines : NSObject
    {
        public class Options : INSObjectOptions<Options>
        {
            public StaveEnum staveEnum = StaveEnum.Undefined;
            public float sideOffset = 0f;
            public Color lineColor = Color.black;

            public void Reset()
            {
                staveEnum = StaveEnum.Undefined;
                sideOffset = 0;
                lineColor = Color.black;
            }

            public void CopyValuesFrom(Options o)
            {
                staveEnum = o.staveEnum;
                sideOffset = o.sideOffset;
                lineColor = o.lineColor;
            }
        }

        public readonly Options options = new ();
        public static readonly Options _options = new ();

        public List<NSObjectVector> lines = new ();

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            lines = new List<NSObjectVector>();

            if (options.staveEnum == StaveEnum.Undefined) return;

            int lineCount = (int)options.staveEnum;

            NSObjectVector line = null;

            for (int i = 0; i < lineCount; i++)
            {
                line = this.AddObject<NSObjectVector>( pool);
                line.vector.vectorEnum = VectorEnum.LineHorizontal;
                line.rectTransform.pivot = new Vector2(0f, 0.5f);
                line.vector.lineHorizontal.options.thickness = ns.LineWidth;
                line.vector.lineHorizontal.options.sidesBlur = ns.LineWidth / 2f;
                line.vector.lineHorizontal.options.endsBlur = ns.LineWidth / 2f;
                line.followParentRectWidth = true;
                line.vector.lineHorizontal.options.followRectTransformWidth = true;

                lines.Add(line);

                line.Commit();

                line.Shift(DirectionEnum.Up, true, i);
            }

            Shift(DirectionEnum.Down, true, 2);
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();
        }
    }
}
