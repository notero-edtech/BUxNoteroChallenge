using UnityEngine;
using System;

namespace ForieroEditor.Extensions
{
    public static partial class ForieroEditorExtensions
    {
        public static Color R(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        public static Color G(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }

        public static Color B(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }

        public static Color A(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        static string hex = "0123456789ABCDEF";
        public static char IntToHex(this int i) => hex[Mathf.Clamp(i, 0,15)];
        public static int HexToInt(this char hexChar) => hex.IndexOf(hexChar.ToString().ToUpper(), StringComparison.Ordinal);

        public static string ToHex(this Color color)
        {
            Color32 c = (Color32)color;
            return c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2") + c.a.ToString("X2");
        }

        public static Color HexToColor(string hexChars)
        {
            if (hexChars.Length == 7)
            {
                byte r = (byte)(hexChars[1].HexToInt() * 16 + hexChars[2].HexToInt());
                byte g = (byte)(hexChars[3].HexToInt() * 16 + hexChars[4].HexToInt());
                byte b = (byte)(hexChars[5].HexToInt() * 16 + hexChars[6].HexToInt());

                return new Color32(r, g, b, 255);
            }
            else
            {
                byte r = (byte)(hexChars[1].HexToInt() * 16 + hexChars[2].HexToInt());
                byte g = (byte)(hexChars[3].HexToInt() * 16 + hexChars[4].HexToInt());
                byte b = (byte)(hexChars[5].HexToInt() * 16 + hexChars[6].HexToInt());
                byte a = (byte)(hexChars[7].HexToInt() * 16 + hexChars[8].HexToInt());

                return new Color32(r, g, b, a);
            }
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
