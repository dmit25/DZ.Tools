using System;

namespace DZ.Tools
{
    /// <summary>
    /// Represents tags match
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public class Match<TType> : TagsPair<Candidate<TType>>
    {
        /// <summary>
        /// Creates new match
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public Match(
            Candidate<TType> expected,
            Candidate<TType> actual)
            : base(expected, actual) { }

        /// <summary>
        /// Returns descriptive string that represents <paramref name="tag"/>
        /// </summary>
        protected override string Render(Candidate<TType> tag, Func<Candidate<TType>, string> renderer)
        {
            return "{0} - {1}, <{2}:{3}> : {4}".FormatWith(
                tag.Tag.Begin,
                tag.Tag.End,
                tag.Tag.Type,
                tag.Score,
                renderer(tag));
        }
    }
}