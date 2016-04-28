using System.Collections.Generic;
using DZ.Tools.Interfaces;

namespace DZ.Tools
{
    /// <summary>
    /// Class that contains common alghoritms
    /// </summary>
    public static class Algo
    {
        /// <summary>
        /// Returns index of element from which search should be started
        /// Uses specific type of binary search
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="posStart"></param>
        /// <param name="posEnd"></param>
        /// <returns></returns>
        public static int StartSearchIndex<TItem>(this List<TItem> elements, int posStart, int posEnd)
            where TItem : IRange
        {
            int low = 0, high = elements.Count - 1, midpoint = 0;
            while (low <= high)
            {
                midpoint = low + (high - low) / 2;
                var current = elements[midpoint];
                if (posStart == current.Begin)
                {
                    return LeftEqual(elements, midpoint);
                }
                //if start > current end then search in the second part
                if (posStart > current.End)
                {
                    low = midpoint + 1;
                }
                //if end lesser than start search in first start 
                else if (posEnd <= current.Begin)
                {
                    high = midpoint - 1;
                }
                //if start lesser then current start then search in the first part
                else if (posStart <= current.Begin)
                {
                    high = midpoint - 1;
                }
                //start and end lies inside current start and end
                else
                {
                    return LeftEqual(elements, midpoint);
                }
            }
            return midpoint;
        }

        /// <summary>
        /// Retrieves position of most left equal element starting from index.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="elements"></param>
        /// <param name="index">Start position</param>
        /// <returns></returns>
        private static int LeftEqual<TItem>(List<TItem> elements, int index)
            where TItem : IRange
        {
            //look backward, because there can be tags with same begin.
            while (index > 0 && elements[index].Begin == elements[index - 1].Begin)
            {
                index--;
            }
            return index;
        }

        private class StartPosComparer<TItem> : IComparer<TItem>
            where TItem : IRange
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(TItem x, TItem y)
            {
                return x.Begin == y.Begin
                    ? 0
                    : (
                        x.Begin > y.End
                            ? 1
                            : (
                                x.End <= y.Begin || x.Begin <= y.Begin
                                    ? -1
                                    : 0));
            }
        }
    }
}