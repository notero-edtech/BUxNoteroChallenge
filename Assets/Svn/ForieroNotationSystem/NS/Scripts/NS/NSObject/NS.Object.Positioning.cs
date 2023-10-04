/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ForieroEngine.Music.NotationSystem.Extensions;
using DG.Tweening;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class NSObject
    {
        public void SetPivot(PivotEnum pivot)
        {
            this.pivot = pivot;
            this.rectTransform.SetPivot(this.pivot.ToPivotAndAnchors());
        }

        //public Vector2 AnchoredPosition(NSObject o)
        //{
        //Vector2 localPoint;
        //Vector2 fromPivotDerivedOffset = new Vector2(o.rectTransform.rect.width * o.rectTransform.pivot.x + o.rectTransform.rect.xMin, o.rectTransform.rect.height * o.rectTransform.pivot.y + o.rectTransform.rect.yMin);
        //Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, o.rectTransform.position);
        //screenP += fromPivotDerivedOffset;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, screenP, null, out localPoint);
        //Vector2 pivotDerivedOffset = new Vector2(this.rectTransform.rect.width * this.rectTransform.pivot.x + this.rectTransform.rect.xMin, this.rectTransform.rect.height * this.rectTransform.pivot.y + this.rectTransform.rect.yMin);
        //return this.rectTransform.anchoredPosition + localPoint - pivotDerivedOffset;
        //return o.rectTransform.anchoredPosition;
        //}

        public float DistanceX(NSObject o)
        {
            return Vector2.Distance(new Vector2(this.rectTransform.anchoredPosition.x, 0), new Vector2(o.rectTransform.anchoredPosition.x, 0f));
        }

        public float DistanceY(NSObject o)
        {
            return Vector2.Distance(new Vector2(0, this.rectTransform.anchoredPosition.y), new Vector2(0, o.rectTransform.anchoredPosition.y));
        }

        public float Distance(NSObject o)
        {
            return Vector2.Distance(this.rectTransform.anchoredPosition, o.rectTransform.anchoredPosition);
        }

        public void AlignXTo(NSObject o, bool recursive, bool relative)
        {
            var y = GetPositionY(relative);
            AlignTo(o, recursive, relative);
            SetPositionY(y, recursive, relative);
        }

        public void AlignYTo(NSObject o, bool recursive, bool relative)
        {
            var x = GetPositionX(relative);
            AlignTo(o, recursive, relative);
            SetPositionX(x, recursive, relative);
        }

        public void AlignTo(NSObject o, bool recursive, bool relative)
        {
            //Vector2 diff = AnchoredPosition(o) - rectTransform.anchoredPosition;
            //rectTransform.anchoredPosition = AnchoredPosition(o);

            Vector2 diff = o.rectTransform.anchoredPosition - rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = o.rectTransform.anchoredPosition;

            int i = 0;
            int count = 0;

            if (recursive)
            {
                count = allObjects.Count;
                for (i = 0; i < count; i++)
                {
                    if (relative)
                    {
                        allObjects[i].PixelShift(diff, recursive);
                    }
                    else
                    {
                        allObjects[i].AlignTo(o, recursive, relative);
                    }
                }
            }
        }

        public void AlignToParent(bool recursive, bool relative)
        {
            if (parent == null)
                return;

            AlignTo(parent, recursive, relative);
        }

        public float GetLastObjectPositionX(float x = 0, bool firstPass = true)
        {
            if (firstPass)
            {
                x = rectTransform.anchoredPosition.x;
                firstPass = false;
            }
            else
            {
                if (rectTransform.anchoredPosition.x > x)
                {
                    x = rectTransform.anchoredPosition.x;
                }
            }

            int count = allObjects.Count;
            for (int i = 0; i < count; i++)
            {
                x = allObjects[i].GetLastObjectPositionX(x, false);
            }

            return x;
        }

        public float GetFirstObjectPositionX(float x = 0, bool firstPass = true)
        {
            if (firstPass)
            {
                x = rectTransform.anchoredPosition.x;
            }
            else
            {
                if (rectTransform.anchoredPosition.x < x)
                {
                    x = rectTransform.anchoredPosition.x;
                }
            }

            int count = allObjects.Count;
            for (int i = 0; i < count; i++)
            {
                x = allObjects[i].GetFirstObjectPositionX(x, false);
            }

            return x;
        }

        public void SetScreenPosition(ScreenPositionEnum screenPositionEnum, bool recursive, bool relative)
        {
            switch (screenPositionEnum)
            {
                case ScreenPositionEnum.BottomCenter:
                    this.SetPosition(Vector2.down * poolRectTransform.GetHeight() / 2f, recursive, relative);
                    break;
                case ScreenPositionEnum.BottomLeft:
                    this.SetPosition(new Vector2(-poolRectTransform.GetWidth() / 2f, -poolRectTransform.GetHeight() / 2f), recursive, relative);
                    break;
                case ScreenPositionEnum.BottomRight:
                    this.SetPosition(new Vector2(poolRectTransform.GetWidth() / 2f, -poolRectTransform.GetHeight() / 2f), recursive, relative);
                    break;
                case ScreenPositionEnum.MiddleCenter:
                    this.SetPosition(Vector2.zero, recursive, relative);
                    break;
                case ScreenPositionEnum.MiddleLeft:
                    this.SetPosition(new Vector2(-poolRectTransform.GetWidth() / 2f, 0), recursive, relative);
                    break;
                case ScreenPositionEnum.MiddleRight:
                    this.SetPosition(new Vector2(-poolRectTransform.GetWidth() / 2f, 0), recursive, relative);
                    break;
                case ScreenPositionEnum.TopCenter:
                    this.SetPosition(Vector2.up * poolRectTransform.GetHeight() / 2f, recursive, relative);
                    break;
                case ScreenPositionEnum.TopLeft:
                    this.SetPosition(new Vector2(-poolRectTransform.GetWidth() / 2f, poolRectTransform.GetHeight() / 2f), recursive, relative);
                    break;
                case ScreenPositionEnum.TopRight:
                    this.SetPosition(new Vector2(poolRectTransform.GetWidth() / 2f, poolRectTransform.GetHeight() / 2f), recursive, relative);
                    break;
            }
        }

        public void SetPosition(Vector2 anchoredPosition, bool recursive, bool relative)
        {
            if (relative && parent != null)
            {
                anchoredPosition += parent.rectTransform.anchoredPosition;
            }

            Vector2 diff = anchoredPosition - rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = anchoredPosition;

            int i = 0;
            int count = 0;

            if (recursive)
            {
                count = allObjects.Count;
                for (i = 0; i < count; i++)
                {
                    if (relative)
                    {
                        allObjects[i].PixelShift(diff, recursive);
                    }
                    else
                    {
                        allObjects[i].SetPosition(anchoredPosition, recursive, relative);
                    }
                }
            }
        }

        public Vector2 GetPosition(bool relative)
        {
            if (relative && parent != null)
            {
                return this.rectTransform.anchoredPosition - parent.rectTransform.anchoredPosition;
            }
            else
            {
                return rectTransform.anchoredPosition;
            }
        }

        public float GetPositionX(bool relative)
        {
            if (relative && parent != null)
            {
                return this.rectTransform.anchoredPosition.x - parent.rectTransform.anchoredPosition.x;
            }
            else
            {
                return rectTransform.anchoredPosition.x;
            }
        }

        public float GetPositionY(bool relative)
        {
            if (relative && parent != null)
            {
                return this.rectTransform.anchoredPosition.y - parent.rectTransform.anchoredPosition.y;
            }
            else
            {
                return rectTransform.anchoredPosition.y;
            }
        }

        Tween positionTween;

        public Tween TweenPosition(Vector2 anchoredPosition, bool recursive, bool relative, float duration, Ease ease)
        {
            positionTween?.Kill();
            return positionTween = DOTween.To(() => GetPosition(relative), x => SetPosition(x, recursive, relative), anchoredPosition, duration).SetTarget(rectTransform).SetEase(ease);
        }

        public Tween TweenPositionX(float x, bool recursive, bool relative, float duration, Ease ease)
        {
            return TweenPosition(new Vector2(x, rectTransform.anchoredPosition.y), recursive, relative, duration, ease);
        }

        public Tween TweenPositionY(float y, bool recursive, bool relative, float duration, Ease ease)
        {
            return TweenPosition(new Vector2(rectTransform.anchoredPosition.x, y), recursive, relative, duration, ease);
        }

        public void SetPositionX(float x, bool recursive, bool relative)
        {

            if (relative && parent != null)
            {
                x += parent.rectTransform.anchoredPosition.x;
            }

            float diff = x - rectTransform.anchoredPosition.x;

            rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);

            int i = 0;
            int count = 0;

            if (recursive)
            {
                count = allObjects.Count;
                for (i = 0; i < count; i++)
                {
                    allObjects[i].PixelShift(new Vector2(diff, 0), recursive);
                }
            }
        }

        public void SetPositionY(float y, bool recursive, bool relative)
        {
            if (relative && parent != null)
            {
                y += parent.rectTransform.anchoredPosition.y;
            }

            float diff = y - rectTransform.anchoredPosition.y;

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);

            int i = 0;
            int count = 0;

            if (recursive)
            {
                count = allObjects.Count;
                for (i = 0; i < count; i++)
                {
                    allObjects[i].PixelShift(new Vector2(0, diff), recursive);
                }
            }
        }

        public void PixelShiftX(float x, bool recursive, bool scaled = false)
        {
            PixelShift(new Vector2(x, 0f), recursive, scaled);
        }

        public void PixelShiftY(float y, bool recursive, bool scaled = false)
        {
            PixelShift(new Vector2(0f, y), recursive, scaled);
        }

        public void PixelShift(Vector2 shiftVector, bool recursive, bool scaled = false)
        {
            rectTransform.anchoredPosition += shiftVector * (scaled ? scale : 1f);

            int i = 0;
            int count = 0;

            if (recursive)
            {
                count = allObjects.Count;
                for (i = 0; i < count; i++)
                {
                    allObjects[i].PixelShift(shiftVector, recursive, scaled);
                }
            }
        }

        public void Shift(DirectionEnum directionEnum, bool recursive, float count = 1f, ShiftStepEnum step = ShiftStepEnum.Whole, bool scaled = false)
        {
            if (directionEnum == DirectionEnum.Undefined)
                return;

            Vector2 v2 = Vector2.zero;
            float px = ns.LineSize / (float)(int)step * count;
            switch (directionEnum)
            {
                case DirectionEnum.Up:
                    v2 = new Vector2(0f, px);
                    break;
                case DirectionEnum.Down:
                    v2 = new Vector2(0f, -px);
                    break;
                case DirectionEnum.Left:
                    v2 = new Vector2(-px, 0f);
                    break;
                case DirectionEnum.Right:
                    v2 = new Vector2(px, 0f);
                    break;
            }

            PixelShift(v2, recursive, scaled);
        }

        public void SnapShift(float shift, DirectionEnum directionEnum, bool recursive, ShiftStepEnum step = ShiftStepEnum.Whole)
        {
            Vector2 v2 = Vector2.zero;
            float stepPx = ns.LineSize / (float)(int)step;
            float px = Mathf.FloorToInt(shift / stepPx) * stepPx;
            switch (directionEnum)
            {
                case DirectionEnum.Up:
                    v2 = new Vector2(0f, px);
                    break;
                case DirectionEnum.Down:
                    v2 = new Vector2(0f, -px);
                    break;
                case DirectionEnum.Left:
                    v2 = new Vector2(-px, 0f);
                    break;
                case DirectionEnum.Right:
                    v2 = new Vector2(px, 0f);
                    break;
            }

            PixelShift(v2, recursive);
        }

        public void SendVisuallyFront(NSObject o = null, bool children = false)
        {
            List<NSObject> list = null;
           
            if (o)
            {
                this.rectTransform.SetSiblingIndex(o.rectTransform.GetSiblingIndex() + 1);
            }
            else
            {
                this.rectTransform.SetAsLastSibling();
            }

            if (children) list = GetVisuallyOrderedList();

            if (children)
            {
                var index = list.IndexOf(this);
                NSObject lastObject = null;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > index)
                    {
                        if (lastObject) list[i].SendVisuallyFront(lastObject, children);
                        else list[i].SendVisuallyBack(this, children);
                    }
                    else if (i < index)
                    {
                        if (lastObject) list[i].SendVisuallyBack(lastObject, children);
                        else list[i].SendVisuallyFront(this, children);
                    }

                    lastObject = list[i];
                }
            }
        }

        public void SendVisuallyBack(NSObject o = null, bool children = false)
        {
            List<NSObject> list = null;
            if (children) list = GetVisuallyOrderedList();

            if (o)
            {
                this.rectTransform.SetSiblingIndex(o.rectTransform.GetSiblingIndex() - 1);
            }
            else
            {
                this.rectTransform.SetAsFirstSibling();
            }

            if (children)
            {
                var index = list.IndexOf(this);
                NSObject lastObject = null;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > index)
                    {
                        if (lastObject) list[i].SendVisuallyFront(lastObject);
                        else list[i].SendVisuallyBack(this);
                    }
                    else if (i < index)
                    {
                        if (lastObject) list[i].SendVisuallyBack(lastObject);
                        else list[i].SendVisuallyFront(this);
                    }

                    lastObject = list[i];
                }
            }
        }

        public List<NSObject> GetVisuallyOrderedList(List<NSObject> list = null)
        {
            if (list == null) list = new List<NSObject>();

            list.Add(this);

            for (int i = 0; i < allObjects.Count; i++)
            {
                allObjects[i].GetVisuallyOrderedList(list);
            }

            list = list.OrderBy(x => x.rectTransform.GetSiblingIndex()).ToList();
            return list;
        }
    }
}
