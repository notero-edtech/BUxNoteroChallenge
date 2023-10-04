using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System;

namespace ExcelDataReader
{
    public static class XslxExtensions
    {
        public static string GetContent(this object cell)
        {
            if (cell == null) return "";

            string result = "";

            if(cell is string)
            {
                result = cell as string;
                result = result.Replace("&apos;", "'");
                result = result.Replace("&amp;", "&");
                result = result.Replace("&quot;", "\"");
                result = result.Replace(";", "_semicolon");
                result = result.Replace("\n", @"\n");
            }
            else if(cell.IsNumeric1())
            {
                result = cell.ToString();
            }
            else if(cell is DBNull)
            {

            }
            else
            {
                Debug.LogError("Cell is expected to be string but is : " + cell.GetType().Name);
            }

            return result;
        }

        public static HashSet<Type> NumericTypes = new HashSet<Type>()
        {
            typeof(byte), typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong), typeof(short), typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(float)
        };

        public static bool IsNumeric1(this object o) => NumericTypes.Contains(o.GetType());

        public static bool IsNumeric2(this object o) => o is byte || o is sbyte || o is ushort || o is uint || o is ulong || o is short || o is int || o is long || o is decimal || o is double || o is float;

        public static bool IsNumeric3(this object o)
        {
            switch (o)
            {
                case Byte b:
                case SByte sb:
                case UInt16 u16:
                case UInt32 u32:
                case UInt64 u64:
                case Int16 i16:
                case Int32 i32:
                case Int64 i64:
                case Decimal m:
                case Double d:
                case Single f:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumeric4(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
