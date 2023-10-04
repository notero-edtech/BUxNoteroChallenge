using System.Runtime.InteropServices;
using UnityEngine;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        public static void Reset(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one;
        }

        public static bool HasParent(this Transform t, Transform parent) => t.parent == parent || t.parent.HasParent(parent);
        
        public static Transform Search(this Transform target, string name)
        {
            if (target == null) return null;
            if (target.name == name) return target;
            for (int i = 0; i < target.childCount; ++i)
            {
                var result = Search(target.GetChild(i), name);
                if (result != null) return result;
            }
            return null;
        }

        public static Transform SearchEndsWith(this Transform target, string name)
        {
            if (target == null) return null;
            if (target.name.EndsWith(name, System.StringComparison.InvariantCulture)) return target;
            for (int i = 0; i < target.childCount; ++i)
            {
                var result = SearchEndsWith(target.GetChild(i), name);
                if (result != null) return result;
            }
            return null;
        }

        public static Transform SearchStartsWith(this Transform target, string name)
        {
            if (target == null)
                return null;

            if (target.name.StartsWith(name, System.StringComparison.InvariantCulture))
                return target;

            for (int i = 0; i < target.childCount; ++i)
            {
                var result = SearchStartsWith(target.GetChild(i), name);

                if (result != null)
                    return result;
            }

            return null;
        }

        public static void CenterPivot(this Transform transform, bool recursive, bool twoD)
        {
            Vector3 sum = new Vector3(0, 0, 0);
            int count = 0;

            foreach (Transform t in transform)
            {
                if (recursive)
                {
                    t.CenterPivot(recursive, twoD);
                }

                sum = new Vector3(sum.x + t.position.x, sum.y + t.position.y, twoD ? 0 : sum.z + t.position.z);
                count++;
            }

            if (count == 0)
            {
                return;
            }

            Vector3 center = new Vector3(sum.x / (float)count, sum.y / (float)count, twoD ? transform.position.z : sum.z / (float)count);
            Vector3 diff = new Vector3(transform.position.x - center.x, transform.position.y - center.y, twoD ? 0 : transform.position.z - center.z);

            transform.position = center;

            foreach (Transform t in transform)
            {
                t.position = new Vector3(t.position.x + diff.x, t.position.y + diff.y, t.position.z + diff.z);
            }
        }

        public static void LookAt2D(this Transform t, Transform target)
        {
            LookAt2D(t, target.position);
        }

        public static void LookAt2D(this Transform t, Vector3 target)
        {
            Vector3 deltaPosition = target - t.position;
            float angle = Mathf.Atan2(deltaPosition.y, deltaPosition.x) * Mathf.Rad2Deg - 90f;
            t.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public static void CorrectPixelPosition(this Transform transform, Camera camera = null)
        {
            Camera c = camera ? camera : Camera.main;
            Vector3 position = c.WorldToScreenPoint(transform.position);
            float distance = Vector3.Distance(c.transform.position, transform.position);
            position = c.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), distance));
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        public static void CeilPixelPosition(this Transform transform, Camera camera = null)
        {
            Camera c = camera ? camera : Camera.main;
            Vector3 position = c.WorldToScreenPoint(transform.position);
            float distance = Vector3.Distance(c.transform.position, transform.position);
            position = c.ScreenToWorldPoint(new Vector3(Mathf.CeilToInt(position.x), Mathf.CeilToInt(position.y), distance));
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }


        public static void FloorPixelPosition(this Transform transform, Camera camera = null)
        {
            Camera c = camera ? camera : Camera.main;
            Vector3 position = c.WorldToScreenPoint(transform.position);
            float distance = Vector3.Distance(c.transform.position, transform.position);
            position = c.ScreenToWorldPoint(new Vector3(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), distance));
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        public static void SetPixelPosition(this Transform target, Vector2 pixels, float layer, Space space = Space.World, Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;
            switch (space)
            {
                case Space.World:
                    Vector3 worldPos2 = camera.ScreenToWorldPoint(new Vector3(pixels.x, pixels.y, 1));
                    target.position = worldPos2.ToVector2().ToVector3(layer);
                    break;
                case Space.Self:
                    if (target.parent == null)
                    {
                        SetPixelPosition(target, pixels, layer, Space.World);
                    }
                    else
                    {
                        Vector3 pixelPos = camera.WorldToScreenPoint(target.parent.position) + pixels.ToVector3();
                        Vector3 worldPos = camera.ScreenToWorldPoint(new Vector3(pixelPos.x, pixelPos.y, 1));
                        target.position = new Vector3(worldPos.x, worldPos.y, layer);
                    }
                    break;
            }
        }

        public static Vector3 GetPixelPosition(this Transform target, Space space = Space.World, Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;
            if (space == Space.Self)
            {
                if (target.parent != null)
                {
                    Vector3 parentPos = camera.WorldToScreenPoint(target.parent.position.ToVector2().ToVector3(1));
                    Vector3 myPos = camera.WorldToScreenPoint(target.position.SetLayer(1f));
                    Vector2 result = new Vector2(myPos.x - parentPos.x, myPos.y - parentPos.y);
                    return new Vector3(Mathf.RoundToInt(result.x), Mathf.RoundToInt(result.y), target.position.z);
                }
            }
            Vector3 var2 = camera.WorldToScreenPoint(target.position.SetLayer(1f));
            return var2.SetLayer(target.position.z);
        }
    }
}