using UnityEngine;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        #region FLOAT

        public static float Distance(this float f1, float f2)
        {
            return Mathf.Sqrt(Mathf.Pow(f2 - f1, 2));
        }

        public static int ToInt(this float f)
        {
            return Mathf.RoundToInt(f);
        }

        #endregion

        #region BOOL

        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        #endregion

        #region INT

        public static float Distance(this int i1, int i2)
        {
            return Mathf.Sqrt(Mathf.Pow(i1 - i2, 2f));
        }

        public static bool ToBool(this int i)
        {
            return i != 0;
        }

        #endregion
    }
}
