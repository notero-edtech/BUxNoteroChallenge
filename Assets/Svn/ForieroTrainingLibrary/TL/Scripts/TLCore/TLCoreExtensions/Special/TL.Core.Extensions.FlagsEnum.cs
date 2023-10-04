/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training.Core.Extensions
{
    public static partial class TLCoreFlagEnumExtensions
    {
        //checks if the value contains the provided type
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        //checks if the value is only the provided type
        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        //appends a value
        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        //completely removes the value
        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        public static bool IsNone<T>(this Enum type, T value)
        {
            return (Convert.ToInt32(value)) == 0;
        }

        public static bool IsSingleValue<T>(this Enum type, T value) {
            var num = Convert.ToInt32(value);
            int count = 0;
            for (int i = 0; i < 32; i++)
            {
                var flag = 1 << i;
                if ((num & flag) != 0)
                {
                    count++;
                }
            }
            return count == 1;
        }

        public static bool IsMultipleValues<T>(this Enum type, T value)
        {
            return !IsSingleValue(type, value) && !IsNone(type, value);
        }

        public static int ToBitNumber(this Enum value)
        {
            var num = Convert.ToInt32(value);
            for (int i=0; i<32; i++)
            {
                var flag = 1 << i;
                if ((num & flag) !=0)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
