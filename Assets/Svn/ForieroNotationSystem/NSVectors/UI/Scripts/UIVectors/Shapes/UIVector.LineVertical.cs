/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class UIVector : Graphic
    {
        public LineVertical lineVertical = new LineVertical();

        [System.Serializable]
        public class LineVertical : IUIVector
        {
            [System.Serializable]
            public class Options : NSObject.INSObjectOptions<Options>
            {
                public float thickness = 1;
                public float length = 100;
                public bool followRectTransformHeight = false;
                public float sidesBlur = 1;
                public float endsBlur = 1;

                public void Reset()
                {
                    thickness = 1;
                    length = 100;
                    followRectTransformHeight = false;
                    sidesBlur = 1;
                    endsBlur = 1;
                }

                public void CopyValuesFrom(Options o)
                {
                    thickness = o.thickness;
                    length = o.length;
                    followRectTransformHeight = o.followRectTransformHeight;
                    sidesBlur = o.sidesBlur;
                    endsBlur = o.endsBlur;
                }
            }

            public Options options = new Options();

            List<UIVertex[]> verts = new List<UIVertex[]>();

            bool commited = false;

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
                verts = new Line(Vector2.zero, Vector2.up * options.length, options.thickness, options.sidesBlur, options.endsBlur, color, LineAlignEnum.Middle, LineEndsEnum.None).GetVertices();
                commited = true;
            }

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.LineVertical;
            }

            public void GetRectAndPivot(ref Rect r, ref Vector2 p)
            {
                if (commited)
                {
                    r.max = verts[0][0].position;
                    r.min = verts[0][0].position;

                    verts.FindRect(ref r);

                    //var x = Mathf.Abs(r.x / r.width);
                    var y = Mathf.Abs(r.y / r.height);

                    p.Set(0.5f, y);
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
