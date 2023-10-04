using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{

		public static Canvas GetCanvas(this RectTransform rt, bool root = true)
		{
			var r = rt.GetComponentInParent<Canvas>();
			if (!root) return r;
			while (r != null && !r.isRootCanvas) { r = r.transform.parent.GetComponentInParent<Canvas>(); }
			return r;
		}
		
		public static void Reset(this RectTransform t)
		{
			(t as Transform).Reset();						
		}

		public static void SetDefaultScale (this RectTransform trans)
		{
			trans.localScale = new Vector3 (1, 1, 1);
		}

		public static void SetPivotAndAnchors (this RectTransform trans, Vector2 aVec)
		{
			trans.pivot = aVec;
			trans.anchorMin = aVec;
			trans.anchorMax = aVec;
		}

		public static void SetAnchors (this RectTransform trans, Vector2 aVec)
		{
			trans.anchorMin = aVec;
			trans.anchorMax = aVec;
		}

		public static Vector2 GetSize (this RectTransform trans)
		{
			return trans.rect.size;
		}

		public static float GetWidth (this RectTransform trans)
		{
			return trans.rect.width;
		}

		public static float GetHeight (this RectTransform trans)
		{
			return trans.rect.height;
		}

		public static void SetPositionOfPivot (this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3 (newPos.x, newPos.y, trans.localPosition.z);
		}

		public static void SetLeftBottomPosition (this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3 (newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}

		public static void SetLeftTopPosition (this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3 (newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}

		public static void SetRightBottomPosition (this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3 (newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}

		public static void SetRightTopPosition (this RectTransform trans, Vector2 newPos)
		{
			trans.localPosition = new Vector3 (newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}

		public static void SetSize (this RectTransform trans, Vector2 newSize)
		{
			Vector2 oldSize = trans.rect.size;
			Vector2 deltaSize = newSize - oldSize;
			trans.offsetMin = trans.offsetMin - new Vector2 (deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
			trans.offsetMax = trans.offsetMax + new Vector2 (deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
		}

		public static void SetWidth (this RectTransform trans, float newSize)
		{
			SetSize (trans, new Vector2 (newSize, trans.rect.size.y));
		}

		public static void SetHeight (this RectTransform trans, float newSize)
		{
			SetSize (trans, new Vector2 (trans.rect.size.x, newSize));
		}
		
		public static Rect GetScreenCoordinatesOfCorners(RectTransform rt)
		{
			var worldCorners = new Vector3[4];
			rt.GetWorldCorners(worldCorners);
			var result = new Rect(
				worldCorners[0].x,
				worldCorners[0].y,
				worldCorners[2].x - worldCorners[0].x,
				worldCorners[2].y - worldCorners[0].y);
			return result;
		}
 
		public static Vector2 GetPixelPositionOfRect(RectTransform rt)
		{
			Rect screenRect = GetScreenCoordinatesOfCorners(rt);
        
			return new Vector2(screenRect.center.x, screenRect.center.y);
		}
	}
}
