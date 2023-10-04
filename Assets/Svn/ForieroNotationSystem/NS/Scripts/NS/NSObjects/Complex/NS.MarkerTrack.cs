/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem.Extensions;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSMarkerTrack : NSObjectImage
    {
        public class Options : INSObjectOptions<Options>
        {
            public float length = 100f;
            public float height = 20f;
            public DirectionEnum directionEnum = DirectionEnum.Right;

            [Range(0f, 1f)]
            public bool background = false;
            public Color backgroundColor = Color.black;

            public void Reset()
            {
                length = 100f;
                height = 20f;
                directionEnum = DirectionEnum.Right;
                background = false;
                backgroundColor = Color.black;
            }

            public void CopyValuesFrom(Options o)
            {
                length = o.length;
                height = o.height;
                directionEnum = o.directionEnum;
                background = o.background;
                backgroundColor = o.backgroundColor;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public override void Reset()
        {
            DestroyChildren();

            base.Reset();
            options.Reset();

        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            rectTransform.SetWidth(GetTrackWidth());
            rectTransform.SetHeight(GetTrackHeight());

            image.enabled = options.background;
            image.color = options.backgroundColor;
        }

        PivotEnum GetGraphPivot()
        {
            var result = PivotEnum.MiddleLeft;
            switch (options.directionEnum)
            {
                case DirectionEnum.Right:
                    result = PivotEnum.MiddleLeft;
                    break;
                case DirectionEnum.Left:
                    result = PivotEnum.MiddleRight;
                    break;
                case DirectionEnum.Up:
                    result = PivotEnum.BottomCenter;
                    break;
                case DirectionEnum.Down:
                    result = PivotEnum.TopCenter;
                    break;
            }
            return result;
        }

        float GetTrackWidth()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return options.length;
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return options.height;
            }
            else
            {
                return options.length;
            }
        }

        float GetTrackHeight()
        {
            if (options.directionEnum == DirectionEnum.Right || options.directionEnum == DirectionEnum.Left)
            {
                return options.height;
            }
            else if (options.directionEnum == DirectionEnum.Up || options.directionEnum == DirectionEnum.Down)
            {
                return options.length;
            }
            else
            {
                return options.height;
            }
        }
    }
}
