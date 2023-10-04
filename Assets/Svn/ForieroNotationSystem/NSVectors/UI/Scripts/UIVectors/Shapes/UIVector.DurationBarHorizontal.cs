/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class UIVector : Graphic
    {
        public DurationBarHorizontal durationBarHorizontal = new DurationBarHorizontal();

        [System.Serializable]
        public class DurationBarHorizontal : IUIVector
        {
            [System.Serializable]
            public class Options : NSObject.INSObjectOptions<Options>
            {
                [Range(-1000, 1000)]
                public float length = 5;
                [Range(0, 50)]
                public float height = 5;

                [Range(0, 1)]
                public float roundness = 0.5f;

                [Range(1, 32)]
                public int segments = 16;

                [Range(-45, 45)]
                public float slate = 30;

                [Range(0, 4)]
                public float endsBlur = 2;

                [Range(0, 4)]
                public float sidesBlur = 2;

                public void Reset()
                {
                    length = 20;
                    height = 10;
                    segments = 16;
                    roundness = 0.5f;
                    slate = 30;

                    endsBlur = 2;
                    sidesBlur = 2;
                }

                public void CopyValuesFrom(Options o)
                {
                    length = o.length;
                    height = o.height;
                    roundness = o.roundness;
                    segments = o.segments;
                    slate = o.slate;
                    endsBlur = o.endsBlur;
                    sidesBlur = o.sidesBlur;
                }
            }

            public Options options = new Options();

            List<UIVertex> verts = new List<UIVertex>();
            List<int> indices = new List<int>();

            bool commited = false;

            Line line;

            public VectorEnum GetVectorEnum() => VectorEnum.Beam;
            
            public void OnPopulateMesh(VertexHelper vh, Color color)
            {
                if (!commited) { return; }

                foreach (var v in verts)
                {
                    vh.AddVert(v);
                }

                var i = 0;
                while (i < indices.Count)
                {
                    int a = indices[i]; i++;
                    int b = indices[i]; i++;
                    int c = indices[i]; i++;
                    vh.AddTriangle(a, b, c);
                }
            }

            public void Reset(VertexHelper vh, Color color)
            {
                options.Reset();

                verts = new List<UIVertex>();

                Commit(vh, color);
            }

            private float slateAbs;
            private float ofsX;
            private float ofsY;

            public void Commit(VertexHelper vh, Color color)
            {
                verts.Clear();
                indices.Clear();

                var c = color;

                var w = Mathf.Abs(options.length);
                var h = options.height;

                ofsX = 0;
                ofsY = -h * 0.5f;

                float r = h * 0.5f * options.roundness;

                if (r > w / 2)
                {
                    r = w / 2;
                }

                w -= r;

                float sin = Mathf.Sin(Mathf.Deg2Rad * options.slate);
                float cos = Mathf.Cos(Mathf.Deg2Rad * options.slate);
                float tan = Mathf.Tan(Mathf.Deg2Rad * options.slate);
                float slate = h * sin;

                if (slate > w / 2)
                {
                    slate = w / 2;
                }

                float slateTop;
                float slateBottom;

                if (options.slate > 0)
                {
                    slateTop = slate * -0.5f;
                    slateBottom = slate * 0.5f;
                }
                else
                {
                    slateTop = -slate * 0.5f;
                    slateBottom = slate * 0.5f;
                }

                slateAbs = Mathf.Max(Mathf.Abs(slateTop), Mathf.Abs(slateBottom));

                verts.Add(new UIVertex() { position = new Vector2(slateTop, 0), color = c });
                verts.Add(new UIVertex() { position = new Vector2(slateBottom, h), color = c });
                verts.Add(new UIVertex() { position = new Vector2(w, h), color = c });
                verts.Add(new UIVertex() { position = new Vector2(w, 0), color = c });

                verts.Add(new UIVertex() { position = new Vector2(w, h * 0.5f), color = c });

                DoQuad(0, 1, 2, 3);

                var padX = 0; // h * (1-options.roundness);
                var padY = h - (r * 2);

                // bottom round
                var botOfs = verts.Count;
                for (int i = 0; i <= options.segments; i++)
                {
                    var delta = i / (float)options.segments;
                    var angle = Mathf.Deg2Rad * (-90 + 90 * delta);

                    var cx = Mathf.Cos(angle);
                    var cy = Mathf.Sin(angle);

                    cx *= r;
                    cy *= r;

                    cx += w;
                    cy += r;

                    if (i > 0) cx += padX;

                    verts.Add(new UIVertex() { position = new Vector2(cx, cy), color = c });
                }

                for (int i = 0; i < options.segments; i++)
                {
                    indices.Add(4);
                    indices.Add(5 + i);
                    indices.Add(6 + i);
                }

                var midOfs = verts.Count;

                // top round
                var topOfs = verts.Count;
                for (int i = 0; i <= options.segments; i++)
                {
                    var delta = i / (float)options.segments;
                    var angle = Mathf.Deg2Rad * (90 * delta);

                    var cx = Mathf.Cos(angle);
                    var cy = Mathf.Sin(angle);

                    cx *= r;
                    cy *= r;

                    cx += w;
                    cy += r;

                    if (i < options.segments) cx += padX;
                    cy += padY;

                    verts.Add(new UIVertex() { position = new Vector2(cx, cy), color = c });
                }

                for (int i = 0; i < options.segments; i++)
                {
                    indices.Add(4);
                    indices.Add(midOfs + i);
                    indices.Add(midOfs + i + 1);
                }

                // close gap

                indices.Add(4);
                indices.Add(midOfs - 1);
                indices.Add(midOfs);

                // add AA
                if (options.endsBlur > 0 || options.sidesBlur > 0)
                {
                    var sx = options.endsBlur;
                    var sy = options.sidesBlur;

                    var baseOfs = verts.Count;

                    var ca = Color.clear;

                    verts.Add(new UIVertex() { position = new Vector2(-sx + slateTop, 0), color = ca });
                    verts.Add(new UIVertex() { position = new Vector2(-sx + slateBottom, 0 + h), color = ca });

                    verts.Add(new UIVertex() { position = new Vector2(slateBottom, h + sy), color = ca });
                    verts.Add(new UIVertex() { position = new Vector2(w, h + sy), color = ca });

                    verts.Add(new UIVertex() { position = new Vector2(slateTop, -sy), color = ca });
                    verts.Add(new UIVertex() { position = new Vector2(w, -sy), color = ca });

                    verts.Add(new UIVertex() { position = new Vector2(-sx + slateBottom + sx * sin, h + sy), color = ca });
                    verts.Add(new UIVertex() { position = new Vector2(-sx + slateTop, -sy), color = ca });

                    verts.Add(new UIVertex() { position = new Vector2(sx + w, h * 0.5f), color = ca });

                    DoQuad(0, baseOfs + 0, baseOfs + 1, 1);

                    DoQuad(1, baseOfs + 2, baseOfs + 3, 2);
                    DoQuad(0, baseOfs + 4, baseOfs + 5, 3);

                    DoQuad(1, baseOfs + 2, baseOfs + 6, baseOfs + 1);
                    DoQuad(0, baseOfs + 0, baseOfs + 7, baseOfs + 4);

                    // note, there is no quad from 2 to 3 because thats the side curved part

                    //botOfs++;

                    var halfSegs = options.segments / 2;

                    var blurOfs = verts.Count;
                    for (var i = 0; i <= options.segments; i++)
                    {
                        //var kk = Mathf.Abs(i - halfSegs) / (float)halfSegs;

                        var v = verts[botOfs + i];
                        v.color = ca; // Color.blue;
                        v.position += new Vector3(options.endsBlur, -sy, 0);
                        verts.Add(v);

                        if (i > 0)
                        {
                            DoQuad(botOfs + i - 1, blurOfs + i - 1, blurOfs + i, botOfs + i);
                        }
                    }

                    var prevBlurOfs = blurOfs;

                    blurOfs = verts.Count;
                    for (var i = 0; i <= options.segments; i++)
                    {
                        //var kk = 1; // (Mathf.Abs(i - halfSegs) / (float)halfSegs) * sign;

                        var v = verts[topOfs + i];
                        v.color = ca; // Color.red;
                        v.position += new Vector3(options.endsBlur, sy, 0);
                        verts.Add(v);

                        if (i > 0)
                        {
                            DoQuad(topOfs + i - 1, blurOfs + i - 1, blurOfs + i, topOfs + i);
                        }
                    }

                    // close tris on right side
                    DoTri(baseOfs + 3, 2, verts.Count - 1);
                    DoTri(baseOfs + 5, 3, prevBlurOfs);

                    // close AA gap
                    DoQuad(prevBlurOfs + options.segments, botOfs + options.segments, topOfs + 0, blurOfs + 0);
                }

                for (int i = 0; i < verts.Count; i++)
                {
                    var temp = verts[i];
                    temp.position += new Vector3(ofsX, ofsY, 0);

                    if (options.length<0)
                    {
                        temp.position = new Vector3(-temp.position.x + w, temp.position.y, temp.position.z);
                    }

                    verts[i] = temp;
                }

                commited = true;
            }

            private void DoQuad(int a, int b, int c, int d)
            {
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);

                indices.Add(c);
                indices.Add(d);
                indices.Add(a);
            }

            private void DoTri(int a, int b, int c)
            {
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
            }

            public void GetRectAndPivot(ref Rect r, ref Vector2 p)
            {
                if (commited)
                {
                    r.max = verts[0].position;
                    r.min = verts[0].position;

                    var list = new List<UIVertex[]>();
                    list.Add(verts.ToArray());
                    list.FindRect(ref r);

                    var x = Mathf.Abs(r.x / r.width);
                    //var y = Mathf.Abs(r.y / r.height);

                    p.Set(x, 0.5f);

                }
            }

            public void GetRaycastVertices(List<UIVertex[]> raycastVerts)
            {
                raycastVerts.Clear();
                raycastVerts.AddRange(new List<UIVertex[]>() { verts.ToArray() });
            }
        }
    }
}
