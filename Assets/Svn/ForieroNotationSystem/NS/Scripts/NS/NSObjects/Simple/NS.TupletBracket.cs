/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSTupletBracket : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public int tupletNumber = 3;
            public float thickness = 2;
            public float blur = 1;
            public float gap = 10;
            public float height = 10;
            public Vector2 endPoint = new Vector2(10, 10);

            public void Reset()
            {
                tupletNumber = 3;
                thickness = 2;
                blur = 1;
                gap = 10;
                height = 10;
                endPoint = new Vector2(10, 10);
            }

            public void CopyValuesFrom(Options o)
            {
                tupletNumber = o.tupletNumber;
                thickness = o.thickness;
                blur = o.blur;
                gap = o.gap;
                height = o.height;
                endPoint = o.endPoint;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public NSObjectText numberText;

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            numberText = AddObject<NSObjectText>( pool);
            numberText.text.SetText(options.tupletNumber.ToString());
            numberText.Commit();

            vector.tuplet.options.thickness = options.thickness;
            vector.tuplet.options.blur = options.blur;
            vector.tuplet.options.gap = options.gap;
            vector.tuplet.options.height = options.height;
            vector.tuplet.options.endPoint = options.endPoint;
            vector.tuplet.textRT = numberText.rectTransform;
            vector.Commit();
        }

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

            vector.tuplet.options.thickness = options.thickness;
            vector.tuplet.options.blur = options.blur;
            vector.tuplet.options.gap = options.gap;
            vector.tuplet.options.height = options.height;
            vector.tuplet.options.endPoint = options.endPoint;

            vector.vectorEnum = VectorEnum.Tuplet;

            numberText = null;
        }
    }
}
