/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class MeshVector 
    {
        public interface IMeshVector
        {
            VectorEnum GetVectorEnum();

            void OnPopulateMesh(Mesh vh, Color color);

            void Commit(Mesh vh, Color color);

            void Reset(Mesh vh, Color color);

            void GetRectAndPivot(ref Rect rect, ref Vector2 pivot);

            void GetRaycastVertices(List<UIVertex[]> raycastVerts);
        }
    }
}
