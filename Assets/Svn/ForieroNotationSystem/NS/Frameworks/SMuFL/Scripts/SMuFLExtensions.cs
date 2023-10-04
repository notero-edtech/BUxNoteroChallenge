/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System;
using System.Collections;

namespace ForieroEngine.Music.SMuFL.Extensions
{
    public static class SMuFLExtensions
    {
        public static string ToCharString(this Enum e) => ((char)Convert.ToInt32(e)).ToString();
        public static string ToCharString(this int i) => ((char)Convert.ToInt32(i)).ToString();

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
