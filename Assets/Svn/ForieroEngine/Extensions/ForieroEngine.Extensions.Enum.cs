using System;
using System.Collections.Generic;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        static Dictionary<Type, Dictionary<Enum, string>> cachedEnums = new Dictionary<Type, Dictionary<Enum, string>>();
        public static string ToStringCached<T>(this T e) where T : Enum
        {
            if (!cachedEnums.ContainsKey(e.GetType()))
            {
                var d = new Dictionary<Enum, string>();
                d.Add(e, e.ToString());
                cachedEnums.Add(e.GetType(), d);
            }

            if (!cachedEnums[e.GetType()].ContainsKey(e))
            {
                cachedEnums[e.GetType()].Add(e, e.ToString());
            }

            return cachedEnums[e.GetType()][e];
        }

        public static int ToInt(this System.Enum enumValue)
        {
            return System.Convert.ToInt32(enumValue);
        }
    }
}
