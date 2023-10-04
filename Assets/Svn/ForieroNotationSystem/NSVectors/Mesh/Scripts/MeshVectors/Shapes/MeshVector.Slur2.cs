/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class MeshVector 
    {
        public Slur2 slur2 = new Slur2();

        [System.Serializable]
        public class Slur2 : IMeshVector
        {
            [System.Serializable]
            public class Options : NSObject.INSObjectOptions<Options>
            {
                public float middleThickness = 1f;
                public float endsThickness = 0f;
                public Vector2 vMidPointA = new Vector2(30, 50);
                public Vector2 vMidPointB = new Vector2(70, 50);
                public Vector2 vEnd = new Vector2(100, 0);
                public float endsBlur = 1f;
                public float middleBlur = 2f;

                public void Reset()
                {
                    middleThickness = 1f;
                    endsThickness = 0f;
                    vMidPointA = new Vector2(30, 50);
                    vMidPointB = new Vector2(70, 50);
                    vEnd = new Vector2(100, 0);
                    endsBlur = 1f;
                    middleBlur = 2f;
                }

                public void CopyValuesFrom(Options o)
                {
                    middleThickness = o.middleThickness;
                    endsThickness = o.endsThickness;
                    vMidPointA = o.vMidPointA;
                    vMidPointB = o.vMidPointB;
                    vEnd = o.vEnd;
                    endsBlur = o.endsBlur;
                    middleBlur = o.middleBlur;
                }
            }

            public Options options = new Options();

            [System.NonSerialized]
            Vector2 vStart = Vector2.zero;


            UIVertex[] verts;

            bool commited = false;

            public void OnPopulateMesh(Mesh vh, Color color)
            {
                if (!commited)
                {
                    return;
                }

                //vh.DrawCachedQuads(verts);
            }

            public void Reset(Mesh vh, Color color)
            {
                vStart = Vector2.zero;

                options.Reset();

                Commit(vh, color);
            }

            public void Commit(Mesh vh, Color color)
            {
                var baseBezier = new Bezier(vStart, options.vMidPointA, options.endsThickness, options.endsBlur, options.vEnd, options.vMidPointB, options.endsThickness, options.endsBlur);

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

                var groups = new List<Bezier[]>();

                groups.Add(beziers);

                throw new System.NotImplementedException();
                //verts = vh.BuildBeziers(groups, color, 0);
                commited = true;
            }

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.Slur2;
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
