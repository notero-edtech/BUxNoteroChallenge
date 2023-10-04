/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class UIVector : Graphic
    {
        public Tie tie = new Tie();

        [System.Serializable]
        public class Tie : IUIVector
        {
            [System.NonSerialized]
            float start = 0f;

            [System.Serializable]
            public class Options
            {
                public float end = 100f;
                public float height = 50f;
                public float middleThickness = 1f;
                public float endsThickness = 0f;
                public float middleBlur = 2f;
                public float endsBlur = 1f;
            }

            public Options options = new Options();

            Vector2 vStart;
            Vector2 vEnd;
            Vector2 vMidPointA;
            Vector2 vMidPointB;

            UIVertex[] verts;

            bool commited = false;

            public void OnPopulateMesh(VertexHelper vh, Color color)
            {
                if (!commited)
                {
                    return;
                }

                vh.DrawCachedQuads(verts);
            }

            public void Reset(VertexHelper vh, Color color)
            {
                start = 0f;
                options.end = 100f;
                options.height = 50f;
                options.middleThickness = 2f;
                options.endsThickness = 0f;

                options.middleBlur = 2f;
                options.endsBlur = 1f;

                Commit(vh, color);
            }

            public void Commit(VertexHelper vh, Color color)
            {
                vStart = new Vector2(start, 0);
                vEnd = new Vector2(options.end, 0);
                vMidPointA = new Vector2(start + (options.end - start) / 2f - (options.end - start) / 4f, options.height);
                vMidPointB = new Vector2(start + (options.end - start) / 2f + (options.end - start) / 4f, options.height);

                var baseBezier = new Bezier(vStart, vMidPointA, options.endsThickness, options.endsBlur, vEnd, vMidPointB, options.endsThickness, options.endsBlur);

                var beziers = new Bezier[2];
                baseBezier.Split(out beziers[0], out beziers[1]);

                beziers[0].endThickness = options.middleThickness;
                beziers[0].endBlur = options.middleBlur;
                beziers[1].startThickness = options.middleThickness;
                beziers[1].startBlur = options.middleBlur;

                beziers[0].startAlpha = 0.4f;
                beziers[1].endAlpha = 0.4f;

                beziers[0].thicknessEasing = Easing.Mode.easeOutSine;
                beziers[1].thicknessEasing = Easing.Mode.easeInSine;

                var groups = new List<Bezier[]>(1); // force initial size alloc

                groups.Add(beziers);

                verts = vh.BuildBeziers(groups, color, 0);

                commited = true;
            }

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.Tie;
            }

            public void GetRectAndPivot(ref Rect r, ref Vector2 p)
            {
                if (commited)
                {
                    r.max = verts[0].position;
                    r.min = verts[0].position;

                    verts.FindRect(ref r);

                    var x = Mathf.Abs(r.x / r.width);
                    var y = Mathf.Abs(r.y / r.height);

                    p.Set(x, y);
                }
            }

            public void GetRaycastVertices(List<UIVertex[]> raycastVerts)
            {
                raycastVerts.Clear();
                raycastVerts.Add(verts);
            }
        }
    }
}
