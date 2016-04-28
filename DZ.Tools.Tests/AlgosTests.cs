using System.Collections.Generic;
using DZ.Tools.Interfaces;
using NUnit.Framework;

namespace DZ.Tools.Tests
{
    [TestFixture]
    public class AlgosTests
    {
        private static readonly List<C> L = new List<C>
        {
            R(0, 1),
            R(2, 8),
            R(3, 6),
            R(4, 6),
            R(10, 12),
            R(20, 14)
        };



        [Test, TestCaseSource(nameof(GetTestCases))]
        public void SearchClosestTest(KeyValuePair<C, int> test)
        {
            L.StartSearchIndex(test.Key.Begin, test.Key.End).AssertEqualTo(test.Value);
        }

        [Test]
        public void SearchCorrectLeftBorder()
        {
            var list = new List<C>()
            {
                R(0,1),//<== correct one left border
                R(0,2),
                R(1,2)
            };

            list.StartSearchIndex(0, 1).AssertEqualTo(0);
        }

        public static IEnumerable<KeyValuePair<C, int>> GetTestCases()
        {
            return new Dictionary<C, int>
            {
                {R(0,0),0 },
                {R(2,4),1 },
                {R(3,5),2 },
                {R(21,25),5 }
            };
        }

        private static C R(int b, int e)
        {
            return new C(b, e);
        }


        public struct C : IRange
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            internal C(int begin, int end)
                : this()
            {
                Begin = begin;
                End = end;
            }
            /// <summary>
            /// Start position
            /// </summary>
            public int Begin { get; private set; }

            /// <summary>
            /// End position
            /// </summary>
            public int End { get; private set; }
        }
    }
}