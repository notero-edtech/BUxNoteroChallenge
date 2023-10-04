/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class MeshVector 
    {
        public LineTest lineTest = new LineTest();

        [System.Serializable]
        public class LineTest : IMeshVector
        {
            [Range(0, 30)]
            public float thickness = 5;

            public LineAlignEnum lineAlignEnum = LineAlignEnum.Middle;
            public LineEndsEnum lineEndsEnum = LineEndsEnum.None;
            public bool includeBlur = false;
            public bool preserveWidth = false;

            public Vector2 startPoint = Vector2.zero;
            public Vector2 endPoint = new Vector2(50, 0);

            [Range(0, 50)]
            public float sidesBlur = 2;
            [Range(0, 50)]
            public float endsBlur = 0;

            List<UIVertex[]> verts = new List<UIVertex[]>();

            bool commited = false;

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.LineTest;
            }

            public void OnPopulateMesh(Mesh vh, Color color)
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

            public void Reset(Mesh vh, Color color)
            {
                thickness = 5;
                endPoint = new Vector2(50, 0);

                sidesBlur = 2;
                endsBlur = 0;

                verts = new List<UIVertex[]>();

                Commit(vh, color);
            }

            public void Commit(Mesh vh, Color color)
            {
                verts = new Line(startPoint, endPoint, thickness, sidesBlur, endsBlur, color, lineAlignEnum, lineEndsEnum, includeBlur, preserveWidth).GetVertices();
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
