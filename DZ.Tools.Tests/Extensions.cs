using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DZ.Tools.Tests
{
    public delegate void Asserter<in T>(T expected, T actual, int elementIndex);

    public delegate string ContextProvider();

    public static class TestExtensions
    {

        public static int GetInt32Value(this XAttribute attribute)
        {
            return int.Parse(attribute.Value);
        }

        /// <summary>
        /// Asserts that <paramref name="actual"/> list is equal to <paramref name="expected"/> using <paramref name="asserter"/> comparator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="asserter"></param>
        /// <param name="message"></param>
        /// <param name="contextProvider"></param>
        public static void AssertEqualTo<T>(this IList<T> actual, IList<T> expected, Asserter<T> asserter, string message = null, ContextProvider contextProvider = null)
        {
            try
            {
                if (expected == null)
                {
                    NUnit.Framework.Assert.That(actual, Is.Null);
                }
                else
                {
                    NUnit.Framework.Assert.That(actual, Is.Not.Null);
                    NUnit.Framework.Assert.That(actual.Count, Is.EqualTo(expected.Count), message);

                    for (int i = 0; i < expected.Count; i++)
                    {
                        asserter(expected[i], actual[i], i);
                    }
                }
            }
            catch
            {
                contextProvider.Trace();
                throw;
            }

        }

        /// <summary>
        /// Asserts that actual value follows <paramref name="expression"/> constrains
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expression"></param>
        /// <param name="message"></param>
        /// <param name="contextProvider"></param>
        public static void Assert<T>(this T actual, IResolveConstraint expression, string message = null,
            ContextProvider contextProvider = null)
        {
            try
            {
                NUnit.Framework.Assert.That(actual, expression, message);
            }
            catch
            {
                contextProvider.Trace();
                throw;
            }
        }

        /// <summary>
        /// Asserts that <paramref name="actual"/> is equal to <paramref name="expected"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        /// <param name="contextProvider"></param>
        public static void AssertEqualTo<T>(this T actual, T expected, string message = null, ContextProvider contextProvider = null)
        {
            try
            {
                NUnit.Framework.Assert.That(actual, Is.EqualTo(expected), message);
            }
            catch
            {
                contextProvider.Trace();
                throw;
            }
        }

        /// <summary>
        /// Asserts that <paramref name="actual"/> is true
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <param name="contextProvider"></param>
        public static void AssertTrue(this bool actual, string message = null, ContextProvider contextProvider = null)
        {
            try
            {
                NUnit.Framework.Assert.That(actual, Is.True, message);
            }
            catch
            {
                contextProvider.Trace();
                throw;
            }
        }

        /// <summary>
        /// Asserts that <paramref name="actual"/> is false
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <param name="contextProvider"></param>
        public static void AssertFalse(this bool actual, string message = null, ContextProvider contextProvider = null)
        {
            try
            {
                NUnit.Framework.Assert.That(actual, Is.False, message);
            }
            catch
            {
                contextProvider.Trace();
                throw;
            }
        }

        /// <summary>
        /// Asserts that <paramref name="actual"/> is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <param name="contextProvider"></param>
        public static void AssertNull<T>(this T actual, string message = null, ContextProvider contextProvider = null)
        {
            try
            {
                NUnit.Framework.Assert.That(actual, Is.Null, message);
            }
            catch
            {
                contextProvider.Trace();
                throw;
            }
        }

        /// <summary>
        /// Asserts that <paramref name="actual"/> is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <param name="contextProvider"></param>
        [ContractAnnotation("actual:null=>halt")]
        public static void AssertNotNull<T>(this T actual, string message = null, ContextProvider contextProvider = null)
        {
            try
            {
                NUnit.Framework.Assert.That(actual, Is.Not.Null, message);
            }
            catch
            {
                contextProvider.Trace();
                throw;
            }
        }

        public static XElement WrapTextInTextNode(this XElement xml)
        {
            var textNodes = xml.DescendantNodes().OfType<XText>().ToList();
            foreach (var node in textNodes)
            {
                var parent = node.Parent;
                if (parent != null)
                {
                    parent.AddFirst(new XElement("Text", node.Value));
                }
                node.Remove();
            }
            return xml;
        }

        private static void Trace(this ContextProvider contextProvider)
        {
            if (contextProvider != null)
            {
                TracingContext.TraceInformation("Context: [" + contextProvider() + "]");
            }
        }

        public static void ToConsole(this string message)
        {
            Console.WriteLine(message);
        }
    }

    public static class TracingContext
    {
        /// <summary>
        /// Traces information about test process
        /// </summary>
        /// <param name="info"></param>
        public static void TraceInformation(string info)
        {
            Trace.WriteLine("<!-- " + info + " -->");
        }
    }
}