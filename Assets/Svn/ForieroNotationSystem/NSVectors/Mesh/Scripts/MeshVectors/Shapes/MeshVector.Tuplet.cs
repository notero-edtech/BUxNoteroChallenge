/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class MeshVector
    {
        public Tuplet tuplet = new Tuplet();

        [System.Serializable]
        public class Tuplet : IMeshVector
        {
            [System.Serializable]
            public class Options : NSObject.INSObjectOptions<Options>
            {
                [Range(0, 30)]
                public float thickness = 2;
                [Range(0, 20)]
                public float blur = 1;
                [Range(0, 30)]
                public float gap = 10;
                public float height = 10;
                public Vector2 endPoint = new Vector2(10, 10);

                public void Reset()
                {
                    thickness = 2;
                    blur = 1;
                    gap = 10;
                    height = 10;
                    endPoint = new Vector2(10, 10);
                }

                public void CopyValuesFrom(Options o)
                {
                    thickness = o.thickness;
                    blur = o.blur;
                    gap = o.gap;
                    height = o.height;
                    endPoint = o.endPoint;
                }
            }

            public Options options = new Options();

            public RectTransform textRT;

            private bool commited = false;

            List<UIVertex[]> verts1 = new List<UIVertex[]>();
            List<UIVertex[]> verts2 = new List<UIVertex[]>();
            List<UIVertex[]> verts3 = new List<UIVertex[]>();
            List<UIVertex[]> verts4 = new List<UIVertex[]>();

            public void OnPopulateMesh(Mesh vh, Color color)
            {

                if (!commited)
                {
                    Commit(vh, color);
                    commited = true;
                }

                //vh.DrawCachedQuads (verts);

                for (int i = 0; i < verts1.Count; i++)
                {
                    vh.AddUIVertexQuad(verts1[i]);
                }

                for (int i = 0; i < verts2.Count; i++)
                {
                    vh.AddUIVertexQuad(verts2[i]);
                }

                for (int i = 0; i < verts3.Count; i++)
                {
                    vh.AddUIVertexQuad(verts3[i]);
                }

                for (int i = 0; i < verts4.Count; i++)
                {
                    vh.AddUIVertexQuad(verts4[i]);
                }
            }

            public void Reset(Mesh vh, Color color)
            {

                options.Reset();

                verts1 = new List<UIVertex[]>();
                verts2 = new List<UIVertex[]>();
                verts3 = new List<UIVertex[]>();
                verts4 = new List<UIVertex[]>();

                Commit(vh, color);
            }

            public void Commit(Mesh vh, Color color)
            {
                var startPos = Vector2.zero;
                var hOfs = new Vector2(0, options.height);

                var midPoint = Vector2.Lerp(Vector2.zero, options.endPoint, 0.5f) + hOfs;

                //var lines = new Line[4];

                var fixHThickness = new Vector2(options.thickness, 0f);
                //var fixVThickenss = new Vector2(0, thickness);

                //var fixHBlur = new Vector2(blur, 0f);
                //var fixVBlur = new Vector2(0f, blur);

                var norm = (options.endPoint - startPos).normalized;
                var gOfs = norm * options.gap;

                var line1 = new Line(startPos, startPos + hOfs, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.None);
                var line2 = new Line(startPos + hOfs - fixHThickness / 2f, midPoint - gOfs, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.Vertical, false, true);
                line2.Move(new Vector2(0, -line2.calculatedThickness / 2f));
                var line3 = new Line(options.endPoint + hOfs + fixHThickness / 2f, midPoint + gOfs, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.Vertical, false, true);
                line3.Move(new Vector2(0, -line3.calculatedThickness / 2f));
                var line4 = new Line(options.endPoint, options.endPoint + hOfs, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.None);

                Vector2 v2out;

                Vector2 directionV2 = options.height >= 0 ? Vector2.up : Vector2.down;

                Line.LineIntersection(line1.corner1, line1.corner4 + directionV2 * 100f, (line2.corner1 + line2.corner2) / 2f, (line2.corner3 + line2.corner4) / 2f, out v2out);
                line1.corner4 = v2out;
                Line.LineIntersection(line1.blurCorner1, line1.blurCorner4 + directionV2 * 100f, (line2.blurCorner1 + line2.blurCorner2) / 2f, (line2.blurCorner3 + line2.blurCorner4) / 2f, out v2out);
                line1.blurCorner4 = v2out;

                Line.LineIntersection(line4.corner2, line4.corner3 + directionV2 * 100f, line3.corner2, line3.corner3, out v2out);
                line4.corner3 = v2out;
                Line.LineIntersection(line4.blurCorner2, line4.blurCorner3 + directionV2 * 100f, line3.blurCorner2, line3.blurCorner3, out v2out);
                line4.blurCorner3 = v2out;


                verts1 = line1.GetVertices();
                verts2 = line2.GetVertices();
                verts3 = line3.GetVertices();
                verts4 = line4.GetVertices();

                if (textRT)
                {
                    textRT.transform.localPosition = midPoint + new Vector2(0, 1 * options.height);
                }

            }

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.Tuplet;
            }

            public void GetRectAndPivot(ref Rect r, ref Vector2 p)
            {
                if (commited)
                {
                    r.max = verts1[0][0].position;
                    r.min = verts1[0][0].position;

                    verts1.FindRect(ref r);
                    verts2.FindRect(ref r);
                    verts3.FindRect(ref r);
                    verts4.FindRect(ref r);

                    var x = Mathf.Abs(r.x / r.width);
                    var y = Mathf.Abs(r.y / r.height);

                    p.Set(x, y);
                }
            }

            public void GetRaycastVertices(List<UIVertex[]> raycastVerts)
            {
                raycastVerts.Clear();
                raycastVerts.AddRange(verts1);
                raycastVerts.AddRange(verts2);
                raycastVerts.AddRange(verts3);
                raycastVerts.AddRange(verts4);
            }
        }
    }
}
