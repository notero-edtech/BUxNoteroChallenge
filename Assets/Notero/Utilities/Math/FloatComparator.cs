using UnityEngine;

namespace Notero.Utilities.Math
{
    public class FloatComparator
    {
        public static bool IsEquals(float num1, float num2, float precision = 0.0001f)
        {
            float diff = Mathf.Abs(num1 - num2);
            return diff < precision; 
        }
    }
}