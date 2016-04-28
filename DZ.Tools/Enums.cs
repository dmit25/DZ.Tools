using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace DZ.Tools
{
    /// <summary>
    /// Contains enumeration extension methods
    /// </summary>
    internal static class Enumers
    {
        /// <summary>
        /// Returns dictionary fullfilled with enum values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, T> GetEnumValuesDictionary<T>()
        {
            var dict = new Dictionary<string, T>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                //Add case sensitive and case insensitive keys
                dict.Add(value.ToString().ToLower(), (T)value);
                dict.Add(value.ToString(), (T)value);
            }
            return dict;
        }

        /// <summary>
        /// Returns dictionary fullfilled with enum values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<T, string> GetEnumStringValuesDictionary<T>()
        {
            var dict = new Dictionary<T, string>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                //Add case sensitive and case insensitive keys
                dict.Add((T)value, value.ToString());
            }
            return dict;
        }

        /// <summary>
        /// Converts string value into enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>([NotNull]this string value)
            where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Returns list of enum's values
        /// </summary>
        public static List<T> Values<T>()
            where T : struct
        {
            var res = new List<T>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                res.Add((T)value);
            }
            return res;
        }

        /// <summary>
        /// Tries to parse enum value from string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T? TryParseEnum<T>(this string value) where T : struct
        {
            T result;
            if (Enum.TryParse(value, true, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Converts enum value to int
        /// </summary>
        public static int ToInt<T>(this T val)
            where T : struct
        {
            return EnumConverter<T>.ConvertBack(val);
        }
        /// <summary>
        /// Converts enum value to int
        /// </summary>
        public static uint ToUInt<T>(this T val)
            where T : struct
        {
            return (uint)EnumConverter<T>.ConvertBack(val);
        }

        /// <summary>
        /// Returns true if <paramref name="valLeft"/> equals to <paramref name="valRight"/>
        /// </summary>
        public static bool EqualsTo<T>(this T valLeft, T valRight)
            where T : struct
        {
            //default value is defined as 1 (Undefined)
            return EnumConverter<T>.ConvertBack(valLeft) == EnumConverter<T>.ConvertBack(valRight);
        }

        /// <summary>
        /// Returns true if <paramref name="valLeft"/> equals to <paramref name="valRight"/>
        /// </summary>
        public static bool IntersectsWith<T>(this T valLeft, T valRight)
            where T : struct
        {
            return (EnumConverter<T>.ConvertBack(valLeft) & EnumConverter<T>.ConvertBack(valRight)) > 1;
        }
    }

    /// <summary>
    /// Provides enumerables convertion
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    internal static class EnumConverter<TEnum> where TEnum : struct
    {
        public static readonly Func<int, TEnum> Convert = ToEnum();
        public static readonly Func<TEnum, int> ConvertBack = FromEnum();

        private static Func<int, TEnum> ToEnum()
        {
            var parameter = Expression.Parameter(typeof(int));
            var dynamicMethod = Expression.Lambda<Func<int, TEnum>>(
                Expression.Convert(parameter, typeof(TEnum)),
                parameter);
            return dynamicMethod.Compile();
        }

        private static Func<TEnum, int> FromEnum()
        {
            var parameter = Expression.Parameter(typeof(TEnum));
            var dynamicMethod = Expression.Lambda<Func<TEnum, int>>(
                Expression.Convert(parameter, typeof(int)),
                parameter);
            return dynamicMethod.Compile();
        }
    }
}