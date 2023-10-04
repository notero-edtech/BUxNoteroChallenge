/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class UIVector : Graphic
    {
        public Beam beam = new Beam();

        [System.Serializable]
        public class Beam : IUIVector
        {
            [System.Serializable]
            public class Options : NSObject.INSObjectOptions<Options>
            {
                [Range(-30, 30)]
                public float thickness = 5;
                [Range(0, 10)]
                public float blur = 1;
                public Vector2 endPoint = new Vector2(50, 0);

                public void Reset()
                {
                    thickness = 5;
                    blur = 1;
                    endPoint = new Vector2(50, 0);
                }

                public void CopyValuesFrom(Options o)
                {
                    thickness = o.thickness;
                    blur = o.blur;
                    endPoint = o.endPoint;
                }
            }

            public Options options = new Options();

            List<UIVertex[]> verts = new List<UIVertex[]>();

            bool commited = false;

            Line line;

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.Beam;
            }

            public void OnPopulateMesh(VertexHelper vh, Color color)
            {
                if (!commited)
                {
                    return;
                }

                for (int i = 0; i < verts.Count; i++)
                {
                    vh.AddUIVertexQuad(verts[i]);
                }
            }

            public void Reset(VertexHelper vh, Color color)
            {
                options.Reset();

                verts = new List<UIVertex[]>();

                Commit(vh, color);
            }

            public void Commit(VertexHelper vh, Color color)
            {
                line = new Line(Vector2.zero, options.endPoint, options.thickness, options.blur, 0, color, LineAlignEnum.Right, LineEndsEnum.Vertical, false, false);
                verts = line.GetVertices();
                commited = true;
            }

            public void GetRectAndPivot(ref Rect r, ref Vector2 p)
            {
                if (commited)
                {
                    r.max = verts[0][0].position;
                    r.min = verts[0][0].position;

                    verts.FindRect(ref r);

                    var x = Mathf.Abs(r.x / r.width);
                    var y = Mathf.Abs(r.y / r.height);

                    p.Set(x, y);

                }
            }

            public void GetRaycastVertices(List<UIVertex[]> raycastVerts)
            {
                raycastVerts.Clear();
                raycastVerts.AddRange(verts);
            }
        }
    }
}
