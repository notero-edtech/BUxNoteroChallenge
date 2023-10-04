/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class UIVector : Graphic
    {
        public VectorEnum vectorEnum = VectorEnum.Undefined;
        public bool useRect = false;


        [System.NonSerialized]
        public IUIVector iUIVector = null;
        [System.NonSerialized]
        public Rect rect = new Rect();
        [System.NonSerialized]
        public Vector2 pivot = Vector2.zero;

        public List<UIVertex[]> raycastVerts = new List<UIVertex[]>();

        bool commit = true;
        bool reset = false;

        void CheckIUIVector()
        {
            if (vectorEnum == VectorEnum.Undefined)
            {
                iUIVector = null;
                return;
            }
            else if (iUIVector != null && iUIVector.GetVectorEnum() == vectorEnum)
            {
                return;
            }

            switch (vectorEnum)
            {
                case VectorEnum.LineTest:
                    iUIVector = lineTest as IUIVector;
                    break;
                case VectorEnum.LineHorizontal:
                    iUIVector = lineHorizontal as IUIVector;
                    break;
                case VectorEnum.LineVertical:
                    iUIVector = lineVertical as IUIVector;
                    break;
                case VectorEnum.Beam:
                    iUIVector = beam as IUIVector;
                    break;
                case VectorEnum.Slur1:
                    iUIVector = slur1 as IUIVector;
                    break;
                case VectorEnum.Slur2:
                    iUIVector = slur2 as IUIVector;
                    break;
                case VectorEnum.Tie:
                    iUIVector = tie as IUIVector;
                    break;
                case VectorEnum.Hairpin:
                    iUIVector = hairpin as IUIVector;
                    break;
                case VectorEnum.Tuplet:
                    iUIVector = tuplet as IUIVector;
                    break;

                case VectorEnum.DurationBarHorizontal:
                    iUIVector = durationBarHorizontal as IUIVector;
                    break;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            CheckIUIVector();

            if (iUIVector == null)
                return;

            if (reset)
            {
                iUIVector.Reset(vh, color);
                reset = false;
            }

            if (commit)
            {
                iUIVector.Commit(vh, color);

                if (useRect)
                {
                    rect.Set(0, 0, 0, 0);
                    pivot.Set(0, 0);

                    iUIVector.GetRectAndPivot(ref rect, ref pivot);
                    iUIVector.GetRaycastVertices(raycastVerts);
                    Invoke("SetRectAndPivot", 0);
                }

                commit = false;
            }

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

            iUIVector.OnPopulateMesh(vh, color);
        }

        void SetRectAndPivot()
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

            rectTransform.pivot = pivot;

            rectTransform.anchoredPosition = anchoredPosition;
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
