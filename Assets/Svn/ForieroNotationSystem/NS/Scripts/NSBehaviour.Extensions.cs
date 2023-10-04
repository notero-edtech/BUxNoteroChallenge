/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem.Extensions
{
    public static class NSExtensions
    {
        public static Color R(this Color color, float r) => new Color(r, color.g, color.b, color.a);
        public static Color G(this Color color, float g) => new Color(color.r, g, color.b, color.a);
        public static Color B(this Color color, float b) => new Color(color.r, color.g, b, color.a);
        public static Color A(this Color color, float a) => new Color(color.r, color.g, color.b, a);
    }
    public static class NSBehaviourExtensions
    {
        public static void SetRect(this RectTransform trs, float left, float top, float right, float bottom)
        {
            trs.offsetMin = new Vector2(left, bottom);
            trs.offsetMax = new Vector2(-right, -top);
        }

        public static void SetRectLeft(this RectTransform trs, float left)
        {
            trs.offsetMin = new Vector2(left, trs.offsetMin.y);
        }

        public static void SetRectRight(this RectTransform trs, float right)
        {
            trs.offsetMax = new Vector2(-right, trs.offsetMax.y);
        }

        public static void SetRectTop(this RectTransform trs, float top)
        {
            trs.offsetMax = new Vector2(trs.offsetMax.x, -top);
        }

        public static void SetRectBottom(this RectTransform trs, float bottom)
        {
            trs.offsetMin = new Vector2(trs.offsetMin.x, bottom);
        }

        public static Rect GetScreenCoordinates(this RectTransform uiElement)
        {
            var worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            var result = new Rect(
                             worldCorners[0].x,
                             worldCorners[0].y,
                             worldCorners[2].x - worldCorners[0].x,
                             worldCorners[2].y - worldCorners[0].y);
            return result;
        }

        public static Vector2 Pixel2Units2D(this Camera c)
        {
            if (!c.orthographic)
            {
                Debug.LogError("Pixel2Units2D works only with orthographics camera");
                return Vector2.zero;
            }

            Vector3 point = c.WorldToScreenPoint(c.transform.position + c.transform.forward);
            Vector3 centerPoint = c.ScreenToWorldPoint(point);
            Vector3 upPoint = c.ScreenToWorldPoint(point + new Vector3(0, 1, 0));
            Vector3 rightPoint = c.ScreenToWorldPoint(point + new Vector3(1, 0, 0));

            return new Vector2(Mathf.Abs(rightPoint.x - centerPoint.x), Mathf.Abs(upPoint.y - centerPoint.y));
        }

        public static Vector2 Unit2Pixels2D(this Camera c)
        {
            if (!c.orthographic)
            {
                Debug.LogError("Unit2Pixels2D works only with orthographics camera");
                return Vector2.zero;
            }

            Vector3 point = c.transform.position + c.transform.forward;
            Vector3 centerPoint = c.WorldToScreenPoint(point);
            Vector3 upPoint = c.WorldToScreenPoint(point + new Vector3(0, 1, 0));
            Vector3 rightPoint = c.WorldToScreenPoint(point + new Vector3(1, 0, 0));


            return new Vector2(Mathf.Abs(rightPoint.x - centerPoint.x), Mathf.Abs(upPoint.y - centerPoint.y));
        }
    }
}

