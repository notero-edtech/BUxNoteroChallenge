using UnityEngine;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        public static Vector3 ToIntVector3(this Vector3 v)
        {
            return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector3 ToIntXYVector3(this Vector3 v)
        {
            return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), v.z);
        }

        public static Vector2 ToRoundVector2(this Vector2 v)
        {
            return new Vector2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        public static Vector2 ToCeilVector2(this Vector2 v)
        {
            return new Vector2(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
        }

        public static Vector2 ToFloorVector2(this Vector2 v)
        {
            return new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 ToVector3(this Vector2 v, float z = 0f)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 SetLayer(this Vector3 v, float layer)
        {
            return new Vector3(v.x, v.y, layer);
        }

        static public string ToStringCSV(this Vector3 vct)
        {
            return vct.x.ToString() + ";" + vct.y.ToString() + ";" + vct.z.ToString();
        }

        static public Vector3 ParseCSV(this Vector3 vct, string aStringVector, Vector3 aDefaultVector)
        {
            string[] indexes = aStringVector.Split(';');
            if (indexes.Length == 3)
            {
                Vector3 result = new Vector3(int.Parse(indexes[0]), int.Parse(indexes[1]), int.Parse(indexes[2]));
                return result;
            }
            else
            {
                return aDefaultVector;
            }
            //return aDefaultVector;
        }
        public static Vector2 SetX(this Vector2 v, float x) => new Vector2(x, v.y);
        public static Vector2 SetY(this Vector2 v, float y) => new Vector2(v.x, y);
        public static Vector3 SetX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 SetY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 SetZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
        public static Vector2 SnapCalculate(this Vector2 p, Vector2 s)
        {
            var snapX = p.x + ((Mathf.Round(p.x / s.x) - p.x / s.x) * s.x);
            var snapY = p.y + ((Mathf.Round(p.y / s.y) - p.y / s.y) * s.y);
            return new Vector2(Mathf.RoundToInt(snapX), Mathf.RoundToInt(snapY));
        }
    }
}