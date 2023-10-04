/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSRectTransformExtensions
    {
        public static void AlignAnchoredPositionToPixels(this RectTransform t, PixelAlignEnum xPixelAlign = PixelAlignEnum.Undefined, PixelAlignEnum yPixelAlign = PixelAlignEnum.Undefined)
        {
            var x = t.anchoredPosition.x;
            var y = t.anchoredPosition.y;

            x = xPixelAlign switch
            {
                PixelAlignEnum.Ceil => Mathf.Ceil(x),
                PixelAlignEnum.Floor => Mathf.Floor(x),
                PixelAlignEnum.Round => Mathf.Round(x),
                _ => x
            };

            y = yPixelAlign switch
            {
                PixelAlignEnum.Ceil => Mathf.Ceil(y),
                PixelAlignEnum.Floor => Mathf.Floor(y),
                PixelAlignEnum.Round => Mathf.Round(y),
                _ => y
            };

            t.anchoredPosition = new Vector2(x, y);
        }

        public static void FloorAnchoredPositionX(this RectTransform t)
        {
            t.anchoredPosition = new Vector2(Mathf.Floor(t.anchoredPosition.x), t.anchoredPosition.y);
        }

        public static Vector2 GetAnchoredPosition(this RectTransform rt, Camera cam, RectTransform target)
        {
            Vector2 localPoint;
            Vector2 targetPivotDerivedOffset = new Vector2(target.rect.width * target.pivot.x + target.rect.xMin, target.rect.height * target.pivot.y + target.rect.yMin);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(cam, target.position);
            screenP += targetPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenP, cam, out localPoint);
            Vector2 pivotDerivedOffset = new Vector2(rt.rect.width * rt.pivot.x + rt.rect.xMin, rt.rect.height * rt.pivot.y + rt.rect.yMin);
            return localPoint - pivotDerivedOffset;
        }

        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }

        public static void SetPivot(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
        }

        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        public static void SetAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        public static Vector3 Lerp(this Vector3 v, Vector3 destination, float speed)
        {
            return speed > 0 ? Vector3.Lerp(v, destination, Time.deltaTime * speed) : destination;
        }

        public static Vector2 Lerp(this Vector2 v, Vector2 destination, float speed)
        {
            return speed > 0 ? Vector2.Lerp(v, destination, Time.deltaTime * speed) : destination;
        }

        public static Vector2 GetAnchoredPosition(this RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            return localPoint;
        }
        
        public static Vector2 SetX(this Vector2 v, float x) => new Vector2(x, v.y);
        public static Vector2 SetY(this Vector2 v, float y) => new Vector2(v.x, y);
        public static Vector3 SetX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 SetY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 SetZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
    }
}
