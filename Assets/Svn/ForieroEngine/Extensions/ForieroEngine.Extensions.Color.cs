using UnityEngine;
using System;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        public static Color R(this Color color, float r) => new Color(r, color.g, color.b, color.a);
        public static Color G(this Color color, float g) => new Color(color.r, g, color.b, color.a);
        public static Color B(this Color color, float b) => new Color(color.r, color.g, b, color.a);
        public static Color A(this Color color, float a) => new Color(color.r, color.g, color.b, a);
        
        static string hex = "0123456789ABCDEF";
        public static char IntToHex(this int i) => hex[Mathf.Clamp(i, 0,15)];
        public static int HexToInt(this char hexChar) => hex.IndexOf(hexChar.ToString().ToUpper(), StringComparison.Ordinal);

        public static string ToHex(this Color color)
        {
            Color32 c = (Color32)color;
            return c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2") + c.a.ToString("X2");
        }

        public static Color HexToColor(this string hexChars, bool x = true)
        {
            byte r = (byte)(hexChars[0 + (x ? 1 : 0)].HexToInt() * 16 + hexChars[1 + (x ? 1 : 0)].HexToInt());
            byte g = (byte)(hexChars[2 + (x ? 1 : 0)].HexToInt() * 16 + hexChars[3 + (x ? 1 : 0)].HexToInt());
            byte b = (byte)(hexChars[4 + (x ? 1 : 0)].HexToInt() * 16 + hexChars[5 + (x ? 1 : 0)].HexToInt());

            byte a = 255;

            if(x && hexChars.Length == 9) a = (byte)(hexChars[6 + (x ? 1 : 0)].HexToInt() * 16 + hexChars[7 + (x ? 1 : 0)].HexToInt());
            if(!x && hexChars.Length == 8) a = (byte)(hexChars[6 + (x ? 1 : 0)].HexToInt() * 16 + hexChars[7 + (x ? 1 : 0)].HexToInt());

            return new Color32(r, g, b, a);            
        }

        // Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it. //

        public static string ToHex(this Color32 color)
        {
            byte[] bytes = new byte[4] {
            (byte)(color.r * 255),
            (byte)(color.g * 255),
            (byte)(color.b * 255),
            (byte)(color.a * 255)
        };

            string hex =
                BitConverter.ToString(bytes, 0, 1) +
                BitConverter.ToString(bytes, 1, 1) +
                BitConverter.ToString(bytes, 2, 1) +
                BitConverter.ToString(bytes, 3, 1);
            return hex;
        }

        public static Color ToColor(this string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r, g, b, a);
        }

        public static Color32 ToColor32(this string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, a);
        }

    }
}
