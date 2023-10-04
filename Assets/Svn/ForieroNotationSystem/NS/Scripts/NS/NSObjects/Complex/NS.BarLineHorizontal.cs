/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public class NSBarLineHorizontal : NSObject
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
            public float length = 0;
            public bool screenWidth = false;

            public void Reset()
            {
                barLineEnum = BarLineEnum.Undefined;
                length = 0f;
                screenWidth = false;
            }

            public void CopyValuesFrom(Options o)
            {
                barLineEnum = o.barLineEnum;
                length = o.length;
                screenWidth = o.screenWidth;
            }
        }

        public readonly Options options = new Options();
        public static readonly Options _options = new Options();

        public List<NSObjectVector> lines = new List<NSObjectVector>();

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

        PivotEnum GetScreenWidthPivotEnum()
        {
            var result = PivotEnum.MiddleLeft;

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:
                    result = PivotEnum.BottomLeft;
                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    result = PivotEnum.TopLeft;
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    result = PivotEnum.MiddleLeft;
                    break;
            }

            return result;
        }

        void Regular()
        {
            var v = this.AddObject<NSObjectVector>(pool, options.screenWidth ? GetScreenWidthPivotEnum() : pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineHorizontal;
            v.vector.lineHorizontal.options.thickness = ns.BarLineWidth;
            v.vector.lineHorizontal.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineHorizontal.options.endsBlur = ns.BarLineBlur;

            if (options.screenWidth)
            {
                v.rectTransform.pivot = new Vector2(0f, 0.5f);
                v.rectTransform.anchorMin = new Vector2(0f, 0.5f);
                v.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                v.vector.lineHorizontal.options.followRectTransformWidth = true;
            }
            else
            {
                v.vector.lineHorizontal.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize * 4f : options.length;
            }

            v.Commit();

            if (!options.screenWidth)
            {
                switch (pivot)
                {
                    case PivotEnum.TopLeft:
                    case PivotEnum.BottomLeft:
                    case PivotEnum.MiddleLeft:

                        break;
                    case PivotEnum.TopRight:
                    case PivotEnum.MiddleRight:
                    case PivotEnum.BottomRight:
                        v.PixelShiftX(-v.vector.lineHorizontal.options.length, true);
                        break;
                    case PivotEnum.TopCenter:
                    case PivotEnum.BottomCenter:
                    case PivotEnum.MiddleCenter:
                        v.PixelShiftX(-v.vector.lineHorizontal.options.length / 2f, true);
                        break;
                }
            }

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:
                    v.PixelShiftY(v.vector.lineHorizontal.options.thickness / 2f, true);
                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    v.PixelShiftY(-v.vector.lineHorizontal.options.thickness / 2f, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    break;
            }
        }

        void Heavy()
        {
            var v = this.AddObject<NSObjectVector>(pool, options.screenWidth ? GetScreenWidthPivotEnum() : pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineHorizontal;
            v.vector.lineHorizontal.options.thickness = ns.BarLineWidth * 3f;
            v.vector.lineHorizontal.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineHorizontal.options.endsBlur = ns.BarLineBlur;

            if (options.screenWidth)
            {
                v.rectTransform.pivot = new Vector2(0f, 0.5f);
                v.rectTransform.anchorMin = new Vector2(0f, 0.5f);
                v.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                v.vector.lineHorizontal.options.followRectTransformWidth = true;
            }
            else
            {
                v.vector.lineHorizontal.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize * 4f : options.length;
            }

            v.Commit();

            if (!options.screenWidth)
            {
                switch (pivot)
                {
                    case PivotEnum.TopLeft:
                    case PivotEnum.BottomLeft:
                    case PivotEnum.MiddleLeft:

                        break;
                    case PivotEnum.TopRight:
                    case PivotEnum.MiddleRight:
                    case PivotEnum.BottomRight:
                        v.PixelShiftX(-v.vector.lineHorizontal.options.length, true);
                        break;
                    case PivotEnum.TopCenter:
                    case PivotEnum.BottomCenter:
                    case PivotEnum.MiddleCenter:
                        v.PixelShiftX(-v.vector.lineHorizontal.options.length / 2f, true);
                        break;
                }
            }

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:
                    v.PixelShiftY(v.vector.lineHorizontal.options.thickness / 2f, true);
                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    v.PixelShiftY(-v.vector.lineHorizontal.options.thickness / 2f, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
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
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:
                    v2.PixelShiftY(v2.vector.lineHorizontal.options.thickness * 2f + ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    v2.PixelShiftY(-v2.vector.lineHorizontal.options.thickness * 2f - ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    v1.PixelShiftY(-v1.vector.lineHorizontal.options.thickness / 2f - ns.BarLineWidth, true);
                    v2.PixelShiftY(v2.vector.lineHorizontal.options.thickness / 2f + ns.BarLineWidth, true);
                    break;
            }
        }

        void HeavyLight()
        {
            Heavy();
            Regular();

            var heavy = lines[0];
            var light = lines[1];

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:
                    light.PixelShiftY(ns.BarLineWidth * 5f, true);
                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    heavy.PixelShiftY(-ns.BarLineWidth * 4f, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    heavy.PixelShiftY(-heavy.vector.lineHorizontal.options.thickness / 2f - ns.BarLineWidth, true);
                    light.PixelShiftY(light.vector.lineHorizontal.options.thickness / 2f + ns.BarLineWidth, true);
                    break;
            }
        }

        void LightHeavy()
        {
            Regular();
            Heavy();

            var light = lines[0];
            var heavy = lines[1];

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:
                    heavy.PixelShiftY(ns.BarLineWidth * 3f, true);
                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    light.PixelShiftY(-ns.BarLineWidth * 4f, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    light.PixelShiftY(-light.vector.lineHorizontal.options.thickness / 2f - ns.BarLineWidth, true);
                    heavy.PixelShiftY(heavy.vector.lineHorizontal.options.thickness / 2f + ns.BarLineWidth, true);
                    break;
            }
        }

        void LightLigh()
        {
            Regular();
            Regular();

            var lightBottom = lines[0];
            var lightTop = lines[1];

            switch (pivot)
            {
                case PivotEnum.BottomLeft:
                case PivotEnum.BottomRight:
                case PivotEnum.BottomCenter:
                    lightTop.PixelShiftY(lightTop.vector.lineHorizontal.options.thickness * 2f + ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.TopLeft:
                case PivotEnum.TopRight:
                case PivotEnum.TopCenter:
                    lightTop.PixelShiftY(-lightTop.vector.lineHorizontal.options.thickness * 2f - ns.BarLineWidth * 2f, true);
                    break;
                case PivotEnum.MiddleLeft:
                case PivotEnum.MiddleRight:
                case PivotEnum.MiddleCenter:
                    lightBottom.PixelShiftY(-lightBottom.vector.lineHorizontal.options.thickness / 2f - ns.BarLineWidth, true);
                    lightTop.PixelShiftY(lightTop.vector.lineHorizontal.options.thickness / 2f + ns.BarLineWidth, true);
                    break;
            }
        }

        void Short()
        {
            var v = this.AddObject<NSObjectVector>(pool, pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineHorizontal;
            v.vector.lineHorizontal.options.thickness = ns.BarLineWidth;
            v.vector.lineHorizontal.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineHorizontal.options.endsBlur = ns.BarLineBlur;
            v.vector.lineHorizontal.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize * 2f : options.length;
            v.Commit();

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:

                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    v.PixelShiftX(-v.vector.lineHorizontal.options.length, true);
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:
                    v.PixelShiftX(-v.vector.lineHorizontal.options.length / 2f, true);
                    break;
            }
        }

        void Tick()
        {
            var v = this.AddObject<NSObjectVector>(pool, pivot);
            lines.Add(v);
            v.vector.vectorEnum = VectorEnum.LineHorizontal;
            v.vector.lineHorizontal.options.thickness = ns.BarLineWidth;
            v.vector.lineHorizontal.options.sidesBlur = ns.BarLineBlur;
            v.vector.lineHorizontal.options.endsBlur = ns.BarLineBlur;
            v.vector.lineHorizontal.options.length = Mathf.Approximately(options.length, 0f) ? ns.LineSize : options.length;
            v.Commit();

            switch (pivot)
            {
                case PivotEnum.TopLeft:
                case PivotEnum.BottomLeft:
                case PivotEnum.MiddleLeft:

                    break;
                case PivotEnum.TopRight:
                case PivotEnum.MiddleRight:
                case PivotEnum.BottomRight:
                    v.PixelShiftX(-v.vector.lineHorizontal.options.length, true);
                    break;
                case PivotEnum.TopCenter:
                case PivotEnum.BottomCenter:
                case PivotEnum.MiddleCenter:
                    v.PixelShiftX(-v.vector.lineHorizontal.options.length / 2f, true);
                    break;
            }
        }
    }
}
