/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSTie : NSObjectVector
    {
        public class Options : INSObjectOptions<Options>
        {
            public float length = 100f;
            public float height = 20f;
            public float thickness = 10f;
            public float endsThickness = 1f;
            public OverUnderEnum orientationEnum = OverUnderEnum.Undefined;

            public void Reset()
            {
                length = 100f;
                height = 20f;
                thickness = 10f;
                endsThickness = 1f;
                orientationEnum = OverUnderEnum.Undefined;
            }

            public void CopyValuesFrom(Options o)
            {
                length = o.length;
                height = o.height;
                thickness = o.thickness;
                endsThickness = o.endsThickness;
                orientationEnum = o.orientationEnum;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Commit()
        {
            base.Commit();

            vector.vectorEnum = VectorEnum.Tie;
            vector.tie.options.end = options.length;
            switch (options.orientationEnum)
            {
                case OverUnderEnum.Over:
                    vector.tie.options.height = Mathf.Abs(options.height);
                    Shift(DirectionEnum.Up, false, 1.1f, ShiftStepEnum.Half);
                    break;
                case OverUnderEnum.Under:
                case OverUnderEnum.Undefined:
                    vector.tie.options.height = Mathf.Abs(options.height) * -1f;
                    Shift(DirectionEnum.Down, false, 1.1f, ShiftStepEnum.Half);
                    break;
            }

            vector.tie.options.middleThickness = options.thickness;
            vector.tie.options.endsThickness = options.endsThickness;

            vector.Commit();
        }

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            vector.vectorEnum = VectorEnum.Tie;
            vector.tie.options.end = options.length;
            vector.tie.options.height = options.height;
            vector.tie.options.middleThickness = options.thickness = ns.TieThickness;
            vector.tie.options.endsThickness = options.endsThickness = ns.TieEndsThickness;
        }
    }
}
