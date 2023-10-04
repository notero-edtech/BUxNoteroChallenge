/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;

namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static bool ContainsType(this Array array, Type type)
        {
            if (array == null) return false;
            var len = array.Length;
            for (var i = 0; i < len; i++) { if (array.GetValue(i).GetType() == type) return true; }
            return false;
        }

        public static bool ContainsType<T>(this Array array) => array != null && array.ContainsType(typeof(T));

        public static T ObjectOfType<T>(this Array array)
        {
            if (array == null) return default;
            var len = array.Length;
            for (var i = 0; i < len; i++) { if (array.GetValue(i) != null && array.GetValue(i).GetType() == typeof(T)) return (T)array.GetValue(i); }
            return default;
        }

        public static List<T> ObjectsOfType<T>(this Array array)
        {
            var list = new List<T>();
            if (array == null) return list;
            var len = array.Length;
            for (var i = 0; i < len; i++) { if (array.GetValue(i) != null && array.GetValue(i).GetType() == typeof(T)) { list.Add((T)array.GetValue(i)); } }
            return list;
        }

        public static int CountOfType(this Array array, Type type)
        {
            if (array == null) return 0;
            var result = 0; var len = array.Length;
            for (var i = 0; i < len; i++) { if (array.GetValue(i).GetType() == type) result++; }
            return result;
        }

        public static int CountOfType<T>(this Array array) => array?.CountOfType(typeof(T)) ?? 0;

        public static bool Exists(this string s) => !string.IsNullOrEmpty(s);

        public static object[] Add<T>(this object[] e, T item)
        {
            if (e == null) { e = new object[1]; }
            else { Array.Resize<object>(ref e, e.Length + 1); }
            e[^1] = item;
            return e;
        }

        public static bool Contains(this ItemChoiceType[] e, ItemChoiceType item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemChoiceType[] e, ItemChoiceType item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemChoiceType[] e, ItemChoiceType item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static int CountOf(this ItemsChoiceType[] e, ItemsChoiceType item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType[] e, ItemsChoiceType item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static bool Contains(this ItemsChoiceType[] e, ItemsChoiceType item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static ItemsChoiceType[] Add(this ItemsChoiceType[] e, ItemsChoiceType item)
        {
            if (e == null) { e = new ItemsChoiceType[1]; }
            else { Array.Resize<ItemsChoiceType>(ref e, e.Length + 1); }
            e[^1] = item;
            return e;
        }

        public static T ValueOf<T>(this ItemsChoiceType[] e, ItemsChoiceType item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static List<T> ValuesOf<T>(this ItemsChoiceType[] e, ItemsChoiceType item, object[] items)
        {
            var l = new List<T>();
            for (var i = 0; i < e.Length; i++) { if (e[i] == item) l.Add((T)items[i]); }
            return l;
        }

        public static bool Contains(this Item1ChoiceType[] e, Item1ChoiceType item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this Item1ChoiceType[] e, Item1ChoiceType item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this Item1ChoiceType[] e, Item1ChoiceType item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static bool Contains(this ItemChoiceType1[] e, ItemChoiceType1 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemChoiceType1[] e, ItemChoiceType1 item)
        {
            var result = 0; var len = e.Length;
            for (int i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemChoiceType1[] e, ItemChoiceType1 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static bool Contains(this ItemsChoiceType1[] e, ItemsChoiceType1 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType1[] e, ItemsChoiceType1 item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType1[] e, ItemsChoiceType1 item)
        {
            var len = e.Length;
            for (int i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static ItemsChoiceType1[] Add(this ItemsChoiceType1[] e, ItemsChoiceType1 item)
        {
            if (e == null) { e = new ItemsChoiceType1[1]; }
            else { Array.Resize<ItemsChoiceType1>(ref e, e.Length + 1); }
            e[^1] = item;
            return e;
        }

        public static T ValueOf<T>(this ItemsChoiceType1[] e, ItemsChoiceType1 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default(T);
        }

        public static bool Contains(this ItemsChoiceType2[] e, ItemsChoiceType2 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType2[] e, ItemsChoiceType2 item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType2[] e, ItemsChoiceType2 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType2[] e, ItemsChoiceType2 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default(T);
        }

        public static bool Contains(this ItemsChoiceType3[] e, ItemsChoiceType3 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType3[] e, ItemsChoiceType3 item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType3[] e, ItemsChoiceType3 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType3[] e, ItemsChoiceType3 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static bool Contains(this ItemsChoiceType4[] e, ItemsChoiceType4 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType4[] e, ItemsChoiceType4 item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType4[] e, ItemsChoiceType4 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType4[] e, ItemsChoiceType4 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static bool Contains(this ItemsChoiceType5[] e, ItemsChoiceType5 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType5[] e, ItemsChoiceType5 item)
        {
            var result = 0; var len = e.Length;
            for (int i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType5[] e, ItemsChoiceType5 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType5[] e, ItemsChoiceType5 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default(T);
        }

        public static bool Contains(this ItemsChoiceType6[] e, ItemsChoiceType6 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType6[] e, ItemsChoiceType6 item)
        {
            var result = 0; var len = e.Length;
            for (int i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType6[] e, ItemsChoiceType6 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType6[] e, ItemsChoiceType6 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static bool Contains(this ItemsChoiceType7[] e, ItemsChoiceType7 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType7[] e, ItemsChoiceType7 item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType7[] e, ItemsChoiceType7 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType7[] e, ItemsChoiceType7 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static List<T> ValuesOf<T>(this ItemsChoiceType7[] e, ItemsChoiceType7 item, object[] items)
        {
            var l = new List<T>();
            for (var i = 0; i < e.Length; i++) { if (e[i] == item) l.Add((T)items[i]); }
            return l;
        }

        public static bool Contains(this ItemsChoiceType8[] e, ItemsChoiceType8 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType8[] e, ItemsChoiceType8 item)
        {
            var result = 0;
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType8[] e, ItemsChoiceType8 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType8[] e, ItemsChoiceType8 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static bool Contains(this ItemsChoiceType9[] e, ItemsChoiceType9 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType9[] e, ItemsChoiceType9 item)
        {
            var result = 0; var len = e.Length;
            for (int i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType9[] e, ItemsChoiceType9 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType9[] e, ItemsChoiceType9 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static ItemsChoiceType9[] Add(this ItemsChoiceType9[] e, ItemsChoiceType9 item)
        {
            if (e == null) { e = new ItemsChoiceType9[1]; }
            else { Array.Resize<ItemsChoiceType9>(ref e, e.Length + 1); }
            e[^1] = item;
            return e;
        }

        public static bool Contains(this ItemsChoiceType10[] e, ItemsChoiceType10 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType10[] e, ItemsChoiceType10 item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType10[] e, ItemsChoiceType10 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType10[] e, ItemsChoiceType10 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static ItemsChoiceType10[] Add(this ItemsChoiceType10[] e, ItemsChoiceType10 item)
        {
            if (e == null) { e = new ItemsChoiceType10[1]; }
            else { Array.Resize<ItemsChoiceType10>(ref e, e.Length + 1); }
            e[^1] = item;
            return e;
        }

        public static bool Contains(this ItemsChoiceType11[] e, ItemsChoiceType11 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return true; }
            return false;
        }

        public static int CountOf(this ItemsChoiceType11[] e, ItemsChoiceType11 item)
        {
            var result = 0; var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) result++; }
            return result;
        }

        public static int IndexOf(this ItemsChoiceType11[] e, ItemsChoiceType11 item)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return i; }
            return -1;
        }

        public static T ValueOf<T>(this ItemsChoiceType11[] e, ItemsChoiceType11 item, object[] items)
        {
            var len = e.Length;
            for (var i = 0; i < len; i++) { if (e[i] == item) return (T)items[i]; }
            return default;
        }

        public static ItemsChoiceType11[] Add(this ItemsChoiceType11[] e, ItemsChoiceType11 item)
        {
            if (e == null) { e = new ItemsChoiceType11[1]; }
            else { Array.Resize<ItemsChoiceType11>(ref e, e.Length + 1); }
            e[^1] = item;
            return e;
        }
    }
}
