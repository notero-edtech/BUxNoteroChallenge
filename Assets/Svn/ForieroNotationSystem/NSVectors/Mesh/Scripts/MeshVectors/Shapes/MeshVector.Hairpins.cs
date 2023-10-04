/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class MeshVector 
    {
        public Hairpin hairpin = new Hairpin();

        [System.Serializable]
        public class Hairpin : IMeshVector
        {
            public enum HairpinEnum
            {
                Crescendo,
                Dercescendo
            }

            [System.Serializable]
            public class Options : NSObject.INSObjectOptions<Options>
            {
                public HairpinEnum hairpinEnum = HairpinEnum.Crescendo;
                public float thickness = 1;
                public float length = 100;
                public float height = 10;
                public float blur = 2f;

                public void Reset()
                {
                    hairpinEnum = HairpinEnum.Crescendo;
                    thickness = 1;
                    length = 100;
                    height = 10;
                    blur = 2f;
                }

                public void CopyValuesFrom(Options o)
                {
                    hairpinEnum = o.hairpinEnum;
                    thickness = o.thickness;
                    length = o.length;
                    height = o.height;
                    blur = o.blur;
                }
            }

            public Options options = new Options();

            bool commited = false;

            List<UIVertex[]> verts1 = new List<UIVertex[]>();
            List<UIVertex[]> verts2 = new List<UIVertex[]>();

            Line line1;
            Line line2;

            public void OnPopulateMesh(Mesh vh, Color color)
            {
                if (!commited)
                {
                    return;
                }

                for (int i = 0; i < verts1.Count; i++)
                {
                    vh.AddUIVertexQuad(verts1[i]);
                }

                for (int i = 0; i < verts2.Count; i++)
                {
                    vh.AddUIVertexQuad(verts2[i]);
                }
            }

            public void Reset(Mesh vh, Color color)
            {
                options.Reset();

                Commit(vh, color);
            }

            public void Commit(Mesh vh, Color color)
            {
                switch (options.hairpinEnum)
                {
                    case HairpinEnum.Crescendo:
                        {
                            var startPoint = Vector2.zero;
                            var endA = new Vector2(options.length, Mathf.Abs(options.height) / 2f);
                            var endB = new Vector2(options.length, -Mathf.Abs(options.height) / 2f);

                            line1 = new Line(startPoint, endA, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.Vertical, true, true);
                            line2 = new Line(startPoint, endB, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.Vertical, true, true);

                            verts1 = line1.GetVertices();
                            verts2 = line2.GetVertices();
                            break;
                        }

                    case HairpinEnum.Dercescendo:
                        {
                            var startPoint = new Vector2(options.length, 0);
                            var endA = new Vector2(0, Mathf.Abs(options.height) / 2f);
                            var endB = new Vector2(0, -Mathf.Abs(options.height) / 2f);

                            line1 = new Line(startPoint, endA, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.Vertical, true, true);
                            line2 = new Line(startPoint, endB, options.thickness, options.blur, options.blur, color, LineAlignEnum.Middle, LineEndsEnum.Vertical, true, true);

                            verts1 = line1.GetVertices();
                            verts2 = line2.GetVertices();
                            break;
                        }
                }

                commited = true;
            }

            public VectorEnum GetVectorEnum()
            {
                return VectorEnum.Hairpin;
            }

            public void GetRectAndPivot(ref Rect r, ref Vector2 p)
            {
                if (commited)
                {
                    r.max = verts1[0][0].position;
                    r.min = verts1[0][0].position;

                    verts1.FindRect(ref r);
                    verts2.FindRect(ref r);

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
            }
        }
    }
}
