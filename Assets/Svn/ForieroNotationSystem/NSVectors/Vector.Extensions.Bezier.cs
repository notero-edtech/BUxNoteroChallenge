/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
#define genAA

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    /*
     * Cubic bezier curves are defined as 2 points and 2 tangents
     * With this they can represent both curves and straight lines
     * To understand tangents use this link: http://www.useragentman.com/tests/textpath/bezier-curve-construction-set.html
     */

    public struct Bezier
    {
        public readonly BezierEnum bezierEnum;
        public LineEndsEnum lineEndsEnum;

        public Vector2 startPosition;
        public Vector2 startTangent;
        public float startThickness;

        public Vector2 endPosition;
        public Vector2 endTangent;
        public float endThickness;

        public int startOrder;
        public int endOrder;

        public float startAlpha;
        public float endAlpha;

        public float startBlur;
        public float endBlur;

        public Easing.Mode alphaEasing;
        public Easing.Mode thicknessEasing;

        /// <summary>
        /// Use this constructor to initalize Curve
        /// </summary>
        /// <param name="startPos">Start position.</param>
        /// <param name="startTangent">Start tangent.</param>
        /// <param name="startThickness">Start thickness.</param>
        /// <param name="startBlur">Start blur.</param>
        /// <param name="endPos">End position.</param>
        /// <param name="endTangent">End tangent.</param>
        /// <param name="endThickness">End thickness.</param>
        /// <param name="endBlur">End blur.</param>
        public Bezier(Vector2 startPos, Vector2 startTangent, float startThickness, float startBlur, Vector2 endPos, Vector2 endTangent, float endThickness, float endBlur)
        {
            this.bezierEnum = BezierEnum.Curve;
            this.lineEndsEnum = LineEndsEnum.None;

            this.startPosition = startPos;
            this.startTangent = startTangent;
            this.startThickness = startThickness;

            this.endPosition = endPos;
            this.endTangent = endTangent;
            this.endThickness = endThickness;

            this.startOrder = -1;
            this.endOrder = -1;

            this.startBlur = startBlur;
            this.endBlur = endBlur;

            this.startAlpha = 1;
            this.endAlpha = 1;

            this.alphaEasing = Easing.Mode.easeLinear;
            this.thicknessEasing = Easing.Mode.easeLinear;
        }

        /// <summary>
        /// Use this constructor to intialize Line
        /// </summary>
        /// <param name="startPos">Start position.</param>
        /// <param name="startThickness">Start thickness.</param>
        /// <param name="startBlur">Start blur.</param>
        /// <param name="endPos">End position.</param>
        /// <param name="endThickness">End thickness.</param>
        /// <param name="endBlur">End blur.</param>
        /// <param name="bezierEnum">Bezier enum.</param>
        public Bezier(Vector2 startPos, float startThickness, float startBlur, Vector2 endPos, float endThickness, float endBlur, LineEndsEnum lineEndsEnum = LineEndsEnum.None)
        {
            this.bezierEnum = BezierEnum.Line;
            this.lineEndsEnum = lineEndsEnum;

            this.startPosition = startPos;
            this.startTangent = endPos;
            this.startThickness = startThickness;

            this.endPosition = endPos;
            this.endTangent = startPosition;
            this.endThickness = endThickness;

            this.startOrder = 1;
            this.endOrder = -1;

            this.startBlur = startBlur;
            this.endBlur = endBlur;

            this.startAlpha = 1;
            this.endAlpha = 1;

            this.alphaEasing = Easing.Mode.easeLinear;
            this.thicknessEasing = Easing.Mode.easeLinear;
        }

        public void Split(out Bezier bezier1, out Bezier bezier2)
        {
            Vector2 A = startPosition;
            Vector2 B = startTangent;
            Vector2 C = endTangent;
            Vector2 D = endPosition;

            Vector2 E = (A + B) * 0.5f;
            Vector2 F = (B + C) * 0.5f;
            Vector2 G = (C + D) * 0.5f;
            Vector2 H = (E + F) * 0.5f;
            Vector2 J = (F + G) * 0.5f;
            Vector2 K = (H + J) * 0.5f;

            bezier1 = this;
            bezier2 = this;

            bezier1.startPosition = A;
            bezier1.startTangent = E;
            bezier1.endTangent = H;
            bezier1.endPosition = K;

            bezier2.startPosition = K;
            bezier2.startTangent = J;
            bezier2.endTangent = G;
            bezier2.endPosition = D;
        }

        private const float refDistance = 10;
        public int GetSegments()
        {
            return Mathf.FloorToInt((this.startPosition - this.endPosition).magnitude / refDistance) * 3; // Three segments per distance of 20
        }
    }

    public static class UIVectorCoreExtensions
    {
        public struct VertexInfo
        {
            public Vector2 mid;
            public Vector2 tangent;
            public Vector2 normal;
            public Vector2 left;
            public Vector2 right;
            public Vector2 AAL;
            public Vector2 AAR;
            public int order;
            public float thickness;
            public float alpha;
            public float blur;
            public Vector2 temp;
        }

        private static UIVertex[] _verts = new UIVertex[4];
        private static VertexInfo[] _info = new VertexInfo[100];


        public static void DrawCachedQuads(this VertexHelper vh, UIVertex[] vertices)
        {
            if (vertices == null)
            {
                return;
            }

            int n = 0;
            while (n < vertices.Length)
            {
                _verts[0] = vertices[n];
                n++;
                _verts[1] = vertices[n];
                n++;
                _verts[2] = vertices[n];
                n++;
                _verts[3] = vertices[n];
                n++;

                vh.AddUIVertexQuad(_verts);
            }
        }

        public static Color ColorWithAlpha(Color c, float alpha)
        {
            return new Color(c.r, c.g, c.b, c.a * alpha);
        }

        public static Vector2 CubicBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
        {
            float rt = 1 - t;
            float rtt = rt * t;
            return rt * rt * rt * s + 3 * rt * rtt * st + 3 * rtt * t * et + t * t * t * e;
        }

        public static UIVertex[] BuildBezierLines(this VertexHelper vh, Line[] lines, Color color)
        {
            var groups = new List<Bezier[]>(lines.Length); // optimize with known size
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var beziers = new Bezier[1];
                beziers[0] = new Bezier(line.startPosition, line.thickness, line.sidesBlur, line.endPosition, line.thickness, line.endsBlur);
                groups.Add(beziers);
            }

            return vh.BuildBeziers(groups, color);
        }

        public static UIVertex[] BuildBeziers(this VertexHelper vh, List<Bezier[]> bezierGroups, Color color, int smoothnessLevel = 0)
        {
            var result = new List<UIVertex>(1024); // optimize with large constant size

            int requiredTotal = 0;
            foreach (var beziers in bezierGroups)
            {
                for (int k = 0; k < beziers.Length; k++)
                {
                    int segments = beziers[k].GetSegments();
                    requiredTotal += (segments + 1);
                }
            }

            if (_info.Length < requiredTotal) // realloc arrays if theres not enough space
            {
                requiredTotal *= 2; // expand alloc size a bit to reduce frequency of reallocs
                _info = new VertexInfo[requiredTotal];
            }

            foreach (var beziers in bezierGroups)
            {
                int total = 0;
                // 1st pass, calculate the curve with thickness = 0 for all beziers 
                for (int k = 0; k < beziers.Length; k++)
                {
                    var bezier = beziers[k];

                    int segments = bezier.GetSegments();

                    int mid = segments / 2;
                    for (int i = 0; i <= segments; i++)
                    {
                        float mu = i / (float)segments;

                        _info[total + i].thickness = Mathf.Lerp(bezier.startThickness, bezier.endThickness, Easing.GetEase(mu, bezier.thicknessEasing));
                        _info[total + i].alpha = Mathf.Lerp(bezier.startAlpha, bezier.endAlpha, Easing.GetEase(mu, bezier.alphaEasing));
                        _info[total + i].blur = Mathf.Lerp(bezier.startBlur, bezier.endBlur, mu);

                        _info[total + i].mid = CubicBezier(bezier.startPosition, bezier.startTangent, bezier.endPosition, bezier.endTangent, mu);
                        _info[total + i].order = i < mid ? bezier.startOrder : bezier.endOrder; // store Bezier index per point

                        /*if (k>0 && i == 0)
                        {
                            var mix = Vector2.Lerp(_mid[total + i], _mid[total + i - 1], 0.5f);
                            _mid[total + i] += new Vector2(50, 0);
                            //_mid[total + i - 1] = mix;
                        }*/
                    }

                    total += segments;
                }

                // 2nd pass generate normals and tangents
                for (int i = 1; i <= total; i++)
                {
                    Vector2 normal;
                    if (_info[i].order < 0)
                    {
                        normal = (_info[i - 1].mid - _info[i].mid).normalized;
                    }
                    else
                    {
                        normal = (_info[i].mid - _info[i + 1].mid).normalized;
                    }

                    _info[i].normal = normal;
                    _info[i].tangent = new Vector2(normal.y, -normal.x);
                }

                _info[0].normal = _info[1].normal;
                _info[0].tangent = _info[1].tangent;

                // 3nd pass generate left and right curves
                for (int i = 0; i <= total; i++)
                {
                    var thickness = _info[i].thickness;
                    var tangent = _info[i].tangent;
                    //var normal = _normal[i];

                    float blur = _info[i].blur;

                    _info[i].left = _info[i].mid - tangent * thickness;
                    _info[i].right = _info[i].mid + tangent * thickness;

                    _info[i].AAL = _info[i].mid - tangent * (thickness + blur);
                    _info[i].AAR = _info[i].mid + tangent * (thickness + blur);
                }

                // apply smoothing
                for (int s = 1; s <= smoothnessLevel; s++)
                {
                    for (int i = 0; i <= total; i++)
                    {
                        _info[i].temp = _info[i].left;
                    }

                    for (int i = 1; i < total; i++)
                    {
                        Vector2 a = i > 0 ? _info[i - 1].temp : _info[0].temp;
                        Vector2 b = _info[i].temp;
                        Vector2 c = i < total ? _info[i + 1].temp : _info[total].temp;

                        _info[i].left = a * 0.3f + b * 0.4f + c * 0.3f;
                    }

                    for (int i = 0; i <= total; i++)
                    {
                        _info[i].temp = _info[i].AAL;
                    }

                    for (int i = 1; i < total; i++)
                    {
                        Vector2 a = i > 0 ? _info[i - 1].temp : _info[0].temp;
                        Vector2 b = _info[i].temp;
                        Vector2 c = i < total ? _info[i + 1].temp : _info[total].temp;

                        _info[i].AAL = a * 0.3f + b * 0.4f + c * 0.3f;
                    }

                    for (int i = 0; i <= total; i++)
                    {
                        _info[i].temp = _info[i].right;
                    }

                    for (int i = 1; i < total; i++)
                    {
                        Vector2 a = i > 0 ? _info[i - 1].temp : _info[0].temp;
                        Vector2 b = _info[i].temp;
                        Vector2 c = i < total ? _info[i + 1].temp : _info[total].temp;

                        _info[i].right = a * 0.3f + b * 0.4f + c * 0.3f;
                    }

                    for (int i = 0; i <= total; i++)
                    {
                        _info[i].temp = _info[i].AAR;
                    }

                    for (int i = 1; i < total; i++)
                    {
                        Vector2 a = i > 0 ? _info[i - 1].temp : _info[0].temp;
                        Vector2 b = _info[i].temp;
                        Vector2 c = i < total ? _info[i + 1].temp : _info[total].temp;

                        _info[i].AAR = a * 0.3f + b * 0.4f + c * 0.3f;
                    }

                }

#if genAA
                // 4nd pass emit AA geometry -> before solid geometry, order is important!

                for (int i = 1; i <= total; i++)
                {

                    float alpha = _info[i].alpha;

                    _verts[0] = new UIVertex();
                    _verts[0].position = _info[i - 1].left;
                    _verts[0].color = ColorWithAlpha(color, alpha);

                    _verts[1] = new UIVertex();
                    _verts[1].position = _info[i].left;
                    _verts[1].color = ColorWithAlpha(color, alpha);


                    _verts[2] = new UIVertex();
                    _verts[2].position = _info[i].AAL;
                    _verts[2].color = Color.clear;

                    _verts[3] = new UIVertex();
                    _verts[3].position = _info[i - 1].AAL;
                    _verts[3].color = Color.clear;

                    for (int n = 0; n < 4; n++)
                    {
                        result.Add(_verts[n]);
                    }

                    _verts[0] = new UIVertex();
                    _verts[0].position = _info[i - 1].right;
                    _verts[0].color = ColorWithAlpha(color, alpha);

                    _verts[1] = new UIVertex();
                    _verts[1].position = _info[i].right;
                    _verts[1].color = ColorWithAlpha(color, alpha);


                    _verts[2] = new UIVertex();
                    _verts[2].position = _info[i].AAR;
                    _verts[2].color = Color.clear;

                    _verts[3] = new UIVertex();
                    _verts[3].position = _info[i - 1].AAR;
                    _verts[3].color = Color.clear;

                    for (int n = 0; n < 4; n++)
                    {
                        result.Add(_verts[n]);
                    }
                }
#endif

                // 5nd pass emit solid  geometry
                for (int i = 1; i <= total; i++)
                {
                    float alpha = _info[i].alpha;

                    _verts[0] = new UIVertex();
                    _verts[0].position = _info[i - 1].left;
                    _verts[0].color = ColorWithAlpha(color, alpha);

                    _verts[1] = new UIVertex();
                    _verts[1].position = _info[i - 1].right;
                    _verts[1].color = ColorWithAlpha(color, alpha);


                    _verts[2] = new UIVertex();
                    _verts[2].position = _info[i].right;
                    _verts[2].color = ColorWithAlpha(color, alpha);

                    _verts[3] = new UIVertex();
                    _verts[3].position = _info[i].left;
                    _verts[3].color = ColorWithAlpha(color, alpha);

                    for (int n = 0; n < 4; n++)
                    {
                        result.Add(_verts[n]);
                    }

                }

#if genAA
                // 6th pass emit AA on start edge
                if (_info[0].thickness > 0)
                {
                    float alpha = _info[0].alpha;
                    float blur = _info[0].blur;

                    _verts[0] = new UIVertex();
                    _verts[0].position = _info[0].left + _info[0].normal * blur;
                    _verts[0].color = Color.clear;

                    _verts[1] = new UIVertex();
                    _verts[1].position = _info[0].right + _info[0].normal * blur;
                    _verts[1].color = Color.clear;

                    _verts[2] = new UIVertex();
                    _verts[2].position = _info[0].right;
                    _verts[2].color = ColorWithAlpha(color, alpha);

                    _verts[3] = new UIVertex();
                    _verts[3].position = _info[0].left;
                    _verts[3].color = ColorWithAlpha(color, alpha);

                    for (int n = 0; n < 4; n++)
                    {
                        result.Add(_verts[n]);
                    }
                }

                // 7th pass emit AA on final edge
                if (_info[total].thickness > 0)
                {
                    float alpha = _info[total].alpha;
                    float blur = _info[total].blur;

                    _verts[0] = new UIVertex();
                    _verts[0].position = _info[total].left - _info[total].normal * blur;
                    _verts[0].color = Color.clear;

                    _verts[1] = new UIVertex();
                    _verts[1].position = _info[total].right - _info[total].normal * blur;
                    _verts[1].color = Color.clear;

                    _verts[2] = new UIVertex();
                    _verts[2].position = _info[total].right;
                    _verts[2].color = ColorWithAlpha(color, alpha);

                    _verts[3] = new UIVertex();
                    _verts[3].position = _info[total].left;
                    _verts[3].color = ColorWithAlpha(color, alpha);

                    for (int n = 0; n < 4; n++)
                    {
                        result.Add(_verts[n]);
                    }
                }
#endif
            }

            return result.ToArray();
        }


    }
}
