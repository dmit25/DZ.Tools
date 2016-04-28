using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace DZ.Tools
{
    /// <summary>
    /// Contains extension methods
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Returs formatted string
        /// </summary>
        /// <param name="formatText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        [NotNull]
        [StringFormatMethod("formatText")]
        public static string FormatWith([NotNull]this string formatText, [NotNull] params object[] args)
        {
            return string.Format(formatText, args);
        }

        /// <summary>
        /// Returs formatted string
        /// </summary>
        /// <param name="formatText"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        [NotNull]
        [StringFormatMethod("formatText")]
        public static string FormatWith<T>([NotNull]this string formatText, [NotNull] T arg)
        {
            return string.Format(formatText, arg);
        }

        /// <summary>
        /// Returs formatted string
        /// </summary>
        /// <param name="formatText"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        [NotNull]
        [StringFormatMethod("formatText")]
        public static string FormatWith<T1, T2>([NotNull]this string formatText, [NotNull] T1 arg1, [NotNull] T2 arg2)
        {
            return string.Format(formatText, arg1, arg2);
        }

        /// <summary>
        /// Returs formatted string
        /// </summary>
        /// <param name="formatText"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        [NotNull]
        [StringFormatMethod("formatText")]
        public static string FormatWith<T1, T2, T3>([NotNull]this string formatText, [NotNull] T1 arg1, [NotNull] T2 arg2, [NotNull] T3 arg3)
        {
            return string.Format(formatText, arg1, arg2, arg3);
        }

        /// <summary>
        /// Aggregates collection of elements into string internally using stringbuilder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static string AggregateString<T>(
            this IEnumerable<T> collection,
            Func<StringBuilder, T, StringBuilder> aggregator)
        {
            return collection.Aggregate(new StringBuilder(), aggregator).ToString();
        }

        /// <summary>
        /// Throws <exception cref="ArgumentNullException"></exception> if <paramref name="obj"/> is null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="argName"></param>
        [NotNull]
        [ContractAnnotation("halt <= obj:null;obj:notnull=>notnull")]
        public static T ThrowIfNull<T>([CanBeNull]this T obj, [NotNull][InvokerParameterName]string argName)
            where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(argName);
            return obj;
        }
    }
}