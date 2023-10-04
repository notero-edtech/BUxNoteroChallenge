/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public static class MeshVectorExtensions
    {
        public static void AddUIVertexQuad(this Mesh mesh, UIVertex[] vertices)
        {
            throw new System.NotImplementedException();
        }
    }

    public partial class MeshVector : Object
    {
        public VectorEnum vectorEnum = VectorEnum.Undefined;
        public bool useRect = false;


        [System.NonSerialized]
        public IMeshVector iMeshVector = null;
        [System.NonSerialized]
        public Rect rect = new Rect();
        [System.NonSerialized]
        public Vector2 pivot = Vector2.zero;

        public List<UIVertex[]> raycastVerts = new List<UIVertex[]>();

        bool commit = true;
        bool reset = false;

        void CheckIMeshVector()
        {
            if (vectorEnum == VectorEnum.Undefined)
            {
                iMeshVector = null;
                return;
            }
            else if (iMeshVector != null && iMeshVector.GetVectorEnum() == vectorEnum)
            {
                return;
            }

            switch (vectorEnum)
            {
                case VectorEnum.LineTest:
                    iMeshVector = lineTest as IMeshVector;
                    break;
                case VectorEnum.LineHorizontal:
                    iMeshVector = lineHorizontal as IMeshVector;
                    break;
                case VectorEnum.LineVertical:
                    iMeshVector = lineVertical as IMeshVector;
                    break;
                case VectorEnum.Beam:
                    iMeshVector = beam as IMeshVector;
                    break;
                case VectorEnum.Slur1:
                    iMeshVector = slur1 as IMeshVector;
                    break;
                case VectorEnum.Slur2:
                    iMeshVector = slur2 as IMeshVector;
                    break;
                case VectorEnum.Tie:
                    iMeshVector = tie as IMeshVector;
                    break;
                case VectorEnum.Hairpin:
                    iMeshVector = hairpin as IMeshVector;
                    break;
                case VectorEnum.Tuplet:
                    iMeshVector = tuplet as IMeshVector;
                    break;
            }
        }

        protected void OnPopulateMesh(Mesh vh)
        {
            vh.Clear();

            Color color = Color.white; // TODO 

            CheckIMeshVector();

            if (iMeshVector == null)
                return;

            if (reset)
            {
                iMeshVector.Reset(vh, color);
                reset = false;
            }

            if (commit)
            {
                iMeshVector.Commit(vh, color);

                if (useRect)
                {
                    rect.Set(0, 0, 0, 0);
                    pivot.Set(0, 0);

                    iMeshVector.GetRectAndPivot(ref rect, ref pivot);
                    iMeshVector.GetRaycastVertices(raycastVerts);
                    //Invoke("SetRectAndPivot", 0); FIXME
                }

                commit = false;
            }

            /*
            switch (vectorEnum)
            {
                case VectorEnum.LineHorizontal:
                    if (lineHorizontal.options.followRectTransformWidth)
                    {
                        if (!Mathf.Approximately(lineHorizontal.options.length, rectTransform.rect.size.x))
                        {
                            lineHorizontal.options.length = rectTransform.rect.size.x;
                            lineHorizontal.Commit(vh, color);
                        }
                    }
                    break;
                case VectorEnum.LineVertical:
                    if (lineVertical.options.followRectTransformHeight)
                    {
                        if (!Mathf.Approximately(lineHorizontal.options.length, rectTransform.rect.size.y))
                        {
                            lineVertical.options.length = rectTransform.rect.size.y;
                            lineVertical.Commit(vh, color);
                        }
                    }
                    break;
            }
            */

           iMeshVector.OnPopulateMesh(vh, color);
        }

        void SetRectAndPivot()
        {
            /*    Vector2 anchoredPosition = rectTransform.anchoredPosition;

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

                rectTransform.pivot = pivot;

                rectTransform.anchoredPosition = anchoredPosition;*/
        }

        public void Commit()
        {
            commit = true;
        }

        public void ResetVector()
        {
            reset = true;
        }
    }
}
