/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensions
    {
        public static bool Approximately(this double v1, double v2) => System.Math.Abs(v1 - v2) < 0.001;
        public static bool Approximately(this float v1, float v2) => Mathf.Approximately(v1, v2);
        public static double Round(this double value, int digits)
        {
            var mult = System.Math.Pow(10.0f, digits);
            return System.Math.Round(value * mult) / mult;
        }

        public static int Repeat(this int repeater, int value) => value < 0 ? repeater + value % repeater : value % repeater;
        
        public static double Clamp(this double value, double min, double max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
        
        public static double Round(this double f, RoundingEnum roundingEnum = RoundingEnum.Undefined)
        {
            var result = f;
            switch (roundingEnum)
            {
                case RoundingEnum.Round: result = System.Math.Round(f); break;
                case RoundingEnum.Ceil: result = System.Math.Ceiling(f); break;
                case RoundingEnum.Floor: result = System.Math.Floor(f); break;
                case RoundingEnum.Even:
                    result = System.Math.Round(f);
                    if (!Approximately(result % 2, 0f)) { result = result > f ? result + 1 : result - 1; }
                    break;
                case RoundingEnum.Odd:
                    result = System.Math.Round(f);
                    if (Approximately(result % 2, 0f)) { result = result > f ? result - 1 : result + 1; }
                    break;
            }
            return result;
        }

        public static float Round(this float value, int digits)
        {
            var mult = Mathf.Pow(10.0f, digits);
            return Mathf.Round(value * mult) / mult;
        }

        public static float Round(this float f, RoundingEnum roundingEnum = RoundingEnum.Undefined)
        {
            var result = f;
            switch (roundingEnum)
            {
                case RoundingEnum.Round: result = Mathf.Round(f); break;
                case RoundingEnum.Ceil: result = Mathf.Ceil(f); break;
                case RoundingEnum.Floor: result = Mathf.Floor(f); break;
                case RoundingEnum.Even:
                    result = Mathf.Round(f);
                    if (!Mathf.Approximately(result % 2, 0f)) { result = result > f ? result + 1 : result - 1; }
                    break;
                case RoundingEnum.Odd:
                    result = Mathf.Round(f);
                    if (Mathf.Approximately(result % 2, 0f)) { result = result > f ? result - 1 : result + 1; }
                    break;
            }
            return result;
        }

        public static float Distance(this float f1, float f2) => Mathf.Abs(f1 - f2);
    }
}
