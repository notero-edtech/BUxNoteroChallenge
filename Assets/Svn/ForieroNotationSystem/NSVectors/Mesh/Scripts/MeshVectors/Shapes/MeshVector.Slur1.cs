/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class MeshVector 
    {
        public Slur1 slur1 = new Slur1();

        [System.Serializable]
        public class Slur1 : IMeshVector
        {
            [System.Serializable]
            public class Options : NSObject.INSObjectOptions<Options>
            {
                public float middleThickness = 1;
                public float endsThickness = 0f;
                public Vector2 vMidPointH = new Vector2(50, 25);
                public Vector2 vEnd = new Vector2(100, 0);
                public float endsBlur = 1f;
                public float middleBlur = 2f;

                public void Reset()
                {
                    middleThickness = 1;
                    endsThickness = 0f;
                    vMidPointH = new Vector2(50, 25);
                    vEnd = new Vector2(100, 0);
                    endsBlur = 1f;
                    middleBlur = 2f;
                }

                public void CopyValuesFrom(Options o)
                {
                    middleThickness = o.middleThickness;
                    endsThickness = o.endsThickness;
                    vMidPointH = o.vMidPointH;
                    vEnd = o.vEnd;
                    endsBlur = o.endsBlur;
                    middleBlur = o.middleBlur;
                }
            }

            public Options options = new Options();

            [System.NonSerialized]
            Vector2 vStart = Vector2.zero;

            Vector2 vMidPointA = new Vector2(30, 50);
            Vector2 vMidPointB = new Vector2(70, 50);

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
                vMidPointA = new Vector2(vStart.x + (options.vMidPointH.x - vStart.x) / 2f, options.vMidPointH.y);
                vMidPointB = new Vector2(options.vMidPointH.x + (options.vEnd.x - options.vMidPointH.x) / 2f, options.vMidPointH.y);

                var baseBezier = new Bezier(vStart, vMidPointA, options.endsThickness, options.endsBlur, options.vEnd, vMidPointB, options.endsThickness, options.endsBlur);

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
                /*verts = vh.BuildBeziers(groups, color, 0);*/
                commited = true;
            }

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.Slur1;
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
