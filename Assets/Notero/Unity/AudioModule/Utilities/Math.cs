using UnityEngine;

namespace Notero.Unity.AudioModule.Utilities
{
    public static class Math
    {
        public static float NormalToDecibel(float value)
        {
            value = Mathf.Clamp(value, 0.0001F, 1F);
            return Mathf.Log10(value) * 20;
        }

        public static float DecibelToNormal(float value)
        {
            return Mathf.Pow(10, (value / 20));
        }
    }
}
