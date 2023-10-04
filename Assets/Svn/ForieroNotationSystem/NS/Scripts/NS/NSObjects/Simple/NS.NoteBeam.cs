/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSNoteBeam : NSObjectVector
    {
        VerticalDirectionEnum _verticalDirection = VerticalDirectionEnum.Undefined;
        HorizontalDirectionEnum _horizontalDirection = HorizontalDirectionEnum.Undefined;

        public VerticalDirectionEnum verticalDirection
        {
            get
            {
                return _verticalDirection;
            }
            set
            {
                _verticalDirection = value;
                float absThickness = Mathf.Abs(vector.beam.options.thickness);
                switch (_verticalDirection)
                {
                    case VerticalDirectionEnum.Down:
                        thickness = -absThickness;
                        break;
                    case VerticalDirectionEnum.Up:
                        thickness = absThickness;
                        break;
                }
            }
        }

        public HorizontalDirectionEnum horizontalDirection
        {
            get
            {
                return _horizontalDirection;
            }
            set
            {
                _horizontalDirection = value;
                float absLength = Mathf.Abs(vector.beam.options.endPoint.x);
                switch (_horizontalDirection)
                {
                    case HorizontalDirectionEnum.Right:
                        vector.beam.options.endPoint = new Vector2(absLength, vector.beam.options.endPoint.y);
                        break;
                    case HorizontalDirectionEnum.Left:
                        vector.beam.options.endPoint = new Vector2(-absLength, vector.beam.options.endPoint.y);
                        break;
                }
            }
        }

        public Vector2 endPoint
        {
            get
            {
                return vector.beam.options.endPoint;
            }
            set
            {
                vector.beam.options.endPoint = value;
            }
        }

        public float thickness
        {
            get
            {
                return vector.beam.options.thickness;
            }
            set
            {
                vector.beam.options.thickness = value;
            }
        }

        public override void Reset()
        {
            base.Reset();

            endPoint = vector.beam.options.endPoint = new Vector2(ns.LineSize, 0);
            thickness = vector.beam.options.thickness = ns.LineHalfSize;
            verticalDirection = VerticalDirectionEnum.Down;
            horizontalDirection = HorizontalDirectionEnum.Right;
            vector.vectorEnum = VectorEnum.Beam;
        }
    }
}
