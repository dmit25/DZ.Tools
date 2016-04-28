using System.Collections.Generic;

namespace DZ.Tools
{
    public interface ITagComparer<TType>
    {
        /// <summary>
        /// Compares two tags and returns match score
        /// </summary>
        double Compare(Tag<TType> x, Tag<TType> y);
    }

    public static class TagsMatchers<TType>
    {
        public static readonly ITagComparer<TType> Strict = new StrictComparer();
        public static readonly ITagComparer<TType> Lenient = new LenientComparer();
        public static readonly ITagComparer<TType> SemiStrict = new SemiStrictComparer();
        public static readonly IComparer<Tag<TType>> Pos = new PosComparer<TType>();

        private static bool HaveIntersection(Tag<TType> x, Tag<TType> y)
        {
            return //<current_s><candidate_s></current_e>
                y.Begin >= x.Begin && y.Begin <= x.End ||
                //<current_s></candidate_e></current_e>
                y.End >= x.Begin && y.End <= x.End ||
                //<candidate_s><current_s></current_s><candidate_s>
                y.Begin <= x.Begin && y.End >= x.End;
        }

        private class StrictComparer : ITagComparer<TType>
        {
            /// <summary>
            ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
            ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
            ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
            ///     <paramref name="y" />.
            /// </returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            public double Compare(Tag<TType> x, Tag<TType> y)
            {
                if (x.Begin == y.Begin && x.End == y.End)
                {
                    return 1;
                }
                return 0;
            }
        }

        private class LenientComparer : ITagComparer<TType>
        {
            /// <summary>
            ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
            ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
            ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
            ///     <paramref name="y" />.
            /// </returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            public double Compare(Tag<TType> x, Tag<TType> y)
            {
                var res = Strict.Compare(x, y);
                if (res > 0)
                {
                    return res;
                }
                if (HaveIntersection(x, y))
                {
                    return 1;
                }
                return 0;
            }
        }

        private class SemiStrictComparer : ITagComparer<TType>
        {
            /// <summary>
            ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
            ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
            ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
            ///     <paramref name="y" />.
            /// </returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            public double Compare(Tag<TType> x, Tag<TType> y)
            {
                var res = Strict.Compare(x, y);
                if (res > 0)
                {
                    return res;
                }
                if (HaveIntersection(x, y))
                {
                    return 0.5;
                }
                return 0;
            }
        }

        private class PosComparer<T> : IComparer<Tag<T>>
        {
            public int Compare(Tag<T> x, Tag<T> y)
            {
                var beginComparison = x.Begin.CompareTo(y.Begin);
                return beginComparison == 0 ? -x.End.CompareTo(y.End) : beginComparison;
            }
        }
    }
}