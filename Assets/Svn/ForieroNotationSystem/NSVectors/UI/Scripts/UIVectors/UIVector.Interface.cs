/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class UIVector : Graphic
    {
        public interface IUIVector
        {
            VectorEnum GetVectorEnum();

            void OnPopulateMesh(VertexHelper vh, Color color);

            void Commit(VertexHelper vh, Color color);

            void Reset(VertexHelper vh, Color color);

            void GetRectAndPivot(ref Rect rect, ref Vector2 pivot);

            void GetRaycastVertices(List<UIVertex[]> raycastVerts);
        }
    }
}
