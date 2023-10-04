/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class MeshVector
    {
        public static void FindRect(List<UIVertex[]> verts, ref Rect rect)
        {
            foreach (UIVertex[] v in verts)
            {
                FindRect(v, ref rect);
            }
        }

        public static void FindRect(UIVertex[] verts, ref Rect rect)
        {
            foreach (UIVertex v in verts)
            {
                FindRect(v, ref rect);
            }
        }

        public static void FindRect(UIVertex vert, ref Rect rect)
        {
            if (vert.position.x > rect.xMax)
            {
                rect.xMax = vert.position.x;
            }

            if (vert.position.x < rect.xMin)
            {
                rect.xMin = vert.position.x;
            }

            if (vert.position.y > rect.yMax)
            {
                rect.yMax = vert.position.y;
            }

            if (vert.position.y < rect.yMin)
            {
                rect.yMin = vert.position.y;
            }
        }
    }
}
