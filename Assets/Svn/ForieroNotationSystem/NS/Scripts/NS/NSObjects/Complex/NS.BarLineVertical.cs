/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSBarLineVertical : NSObject
    {
        public class Parsing
        {
            public double time = 0;
            public int number = 0;

            public void Reset()
            {
                time = 0;
                number = 0;
            }
        }

        public Parsing parsing = new Parsing();

        public class Options : INSObjectOptions<Options>
        {
            public BarLineEnum barLineEnum = BarLineEnum.Undefined;
            public float length = 0f;
            public bool screenHeight = false;

            public void Reset()
            {
                barLineEnum = BarLineEnum.Undefined;
                length = 0f;
                screenHeight = false;
            }

            public void CopyValuesFrom(Options o)
            {
                barLineEnum = o.barLineEnum;
                length = o.length;
                screenHeight = o.screenHeight;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        List<NSObjectVector> lines = new List<NSObjectVector>();

        public override void Reset()
        {
            base.Reset();
            options.Reset();

            DestroyChildren();

            lines = new List<NSObjectVector>();
        }

        public override void Commit()
        {
            base.Commit();

            DestroyChildren();

            switch (options.barLineEnum)
            {
                case BarLineEnum.Dashed:
                    Dashed();
                    break;
                case BarLineEnum.Dotted:
                    Dotted();
                    break;
                case BarLineEnum.Heavy:
                    Heavy();
                    break;
                case BarLineEnum.HeavyHeavy:
                    HeavyHeavy();
                    break;
                case BarLineEnum.HeavyLight:
                    HeavyLight();
                    break;
                case BarLineEnum.LightHeavy:
                    LightHeavy();
                    break;
                case BarLineEnum.LightLight:
                    LightLigh();
                    break;
                case BarLineEnum.Regular:
                    Regular();
                    break;
                case BarLineEnum.Short:
                    Short();
                    break;
                case BarLineEnum.Tick:
                    Tick();
                    break;
            }
        }

        void Dashed()
        {

        }

        void Dotted()
        {

        }

        PivotEnum GetScreenHeightPivotEnum()
        {
            var result = PivotEnum.BottomCenter;

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:
                    result = PivotEnum.BottomLeft;
                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    result = PivotEnum.BottomRight;
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:
                    result = PivotEnum.BottomCenter;
                    break;
            }

            return result;
        }

        void Regular()
        {
            var v = this.AddObject<NSObjectVector>(pool, options.screenHeight ? GetScreenHeightPivotEnum() : pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineVertical;
            v.vector.lineVertical.options.thickness = ns.BarLineWidth;
            v.vector.lineVertical.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineVertical.options.endsBlur = ns.BarLineBlur;

            if (options.screenHeight)
            {
                v.rectTransform.pivot = new Vector2(0.5f, 0);
                v.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                v.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                v.vector.lineVertical.options.followRectTransformHeight = true;
            }
            else
            {
                v.vector.lineVertical.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize * 4f : options.length;
            }

            v.Commit();

            if (!options.screenHeight)
            {
                switch (pivot)
                {
                    case PivotEnum.BottomLeft:
                    case PivotEnum.BottomRight:
                    case PivotEnum.BottomCenter:

                        break;
                    case PivotEnum.TopLeft:
                    case PivotEnum.TopRight:
                    case PivotEnum.TopCenter:
                        v.PixelShiftY(-v.vector.lineVertical.options.length, true);
                        break;
                    case PivotEnum.MiddleLeft:
                    case PivotEnum.MiddleRight:
                    case PivotEnum.MiddleCenter:
                        v.PixelShiftY(-v.vector.lineVertical.options.length / 2f, true);
                        break;
                }
            }
        }

        void Heavy()
        {
            var v = this.AddObject<NSObjectVector>(pool, options.screenHeight ? GetScreenHeightPivotEnum() : pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineVertical;
            v.vector.lineVertical.options.thickness = ns.BarLineWidth * 3f;
            v.vector.lineVertical.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineVertical.options.endsBlur = ns.BarLineBlur;

            if (options.screenHeight)
            {
                v.rectTransform.pivot = new Vector2(0.5f, 0);
                v.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                v.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                v.vector.lineVertical.options.followRectTransformHeight = true;
            }
            else
            {
                v.vector.lineVertical.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize * 4f : options.length;
            }

            v.Commit();

            if (!options.screenHeight)
            {
                switch (pivot)
                {
                    case PivotEnum.BottomLeft:
                    case PivotEnum.BottomRight:
                    case PivotEnum.BottomCenter:

                        break;
                    case PivotEnum.TopLeft:
                    case PivotEnum.TopRight:
                    case PivotEnum.TopCenter:
                        v.PixelShiftY(-v.vector.lineVertical.options.length, true);
                        break;
                    case PivotEnum.MiddleLeft:
                    case PivotEnum.MiddleRight:
                    case PivotEnum.MiddleCenter:
                        v.PixelShiftY(-v.vector.lineVertical.options.length / 2f, true);
                        break;
                }
            }

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:
                    v.PixelShiftX(v.vector.lineVertical.options.thickness / 2f, true);
                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    v.PixelShiftX(-v.vector.lineVertical.options.thickness / 2f, true);
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:

                    break;
            }
        }

        void HeavyHeavy()
        {
            Heavy();
            Heavy();

            var v1 = lines[0];
            var v2 = lines[1];

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:
                    v2.PixelShiftX(v2.vector.lineVertical.options.thickness * 2f + ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    v2.PixelShiftX(-v2.vector.lineVertical.options.thickness * 2f - ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:
                    v1.PixelShiftX(-v1.vector.lineVertical.options.thickness / 2f - ns.BarLineWidth, true);
                    v2.PixelShiftX(v2.vector.lineVertical.options.thickness / 2f + ns.BarLineWidth, true);
                    break;
            }
        }

        void HeavyLight()
        {
            Heavy();
            Regular();

            var v1 = lines[0];
            var v2 = lines[1];

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:
                    v2.PixelShiftX(ns.BarLineWidth * 5f, true);
                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    v1.PixelShiftX(-ns.BarLineWidth * 3f, true);
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:
                    v1.PixelShiftX(-v1.vector.lineVertical.options.thickness / 2f, true);
                    v2.PixelShiftX(ns.BarLineWidth * 2f, true);
                    break;
            }
        }

        void LightHeavy()
        {
            Regular();
            Heavy();

            var v1 = lines[0];
            var v2 = lines[1];

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:
                    v2.PixelShiftX(ns.BarLineWidth * 3f, true);
                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    v1.PixelShiftX(-ns.BarLineWidth * 5f, true);
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:
                    v1.PixelShiftX(-ns.BarLineWidth * 2f, true);
                    v2.PixelShiftX(v2.vector.lineVertical.options.thickness / 2f, true);
                    break;
            }
        }

        void LightLigh()
        {
            Regular();
            Regular();

            var v1 = lines[0];
            var v2 = lines[1];

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:
                    v2.PixelShiftX(v2.vector.lineVertical.options.thickness * 2f + ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    v2.PixelShiftX(-v2.vector.lineVertical.options.thickness * 2f - ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:
                    v1.PixelShiftX(-v1.vector.lineVertical.options.thickness / 2f - ns.BarLineWidth, true);
                    v2.PixelShiftX(v2.vector.lineVertical.options.thickness / 2f + ns.BarLineWidth, true);
                    break;
            }
        }

        void Short()
        {
            var v = this.AddObject<NSObjectVector>(pool, pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineVertical;
            v.vector.lineVertical.options.thickness = ns.BarLineWidth;
            v.vector.lineVertical.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineVertical.options.endsBlur = ns.BarLineBlur;
            v.vector.lineVertical.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize * 2f : options.length;
            v.Commit();

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:

                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    v.PixelShiftY(-v.vector.lineVertical.options.length, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    v.PixelShiftY(-v.vector.lineVertical.options.length / 2f, true);
                    break;
            }
        }

        void Tick()
        {
            var v = this.AddObject<NSObjectVector>(pool, pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineVertical;
            v.vector.lineVertical.options.thickness = ns.BarLineWidth;
            v.vector.lineVertical.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineVertical.options.endsBlur = ns.BarLineBlur;
            v.vector.lineVertical.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize : options.length;
            v.Commit();

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:

                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    v.PixelShiftY(-v.vector.lineVertical.options.length, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    v.PixelShiftY(-v.vector.lineVertical.options.length / 2f, true);
                    break;
            }
        }
    }
}
