using System;

namespace DZ.Tools
{
    /// <summary>
    /// Represents comparison mismatch case
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public class Mismatch<TType> : TagsPair<Tag<TType>>
    {
        private readonly TType _undefinedValue;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="undefinedValue"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public Mismatch(
            TType undefinedValue,
            Tag<TType> expected,
            Tag<TType> actual = null)
            : base(expected, actual) { _undefinedValue = undefinedValue; }

        /// <summary>
        /// Type of expected tag or undefined type
        /// </summary>
        public TType ExpectedType { get { return Expected != null ? Expected.Type : _undefinedValue; } }
        /// <summary>
        /// Type of actual tag or undefined type
        /// </summary>
        public TType ActualType { get { return Actual != null ? Actual.Type : _undefinedValue; } }

        /// <summary>
        /// Returns descriptive string that represents <paramref name="tag"/>
        /// </summary>
        protected override string Render(Tag<TType> tag, Func<Tag<TType>, string> renderer)
        {
            return string.Format("{0} - {1}, <{2}> : {3}", tag.Begin, tag.End, tag.Type, renderer(tag));
        }
    }
}