using System;

namespace DZ.Tools
{
    public class Match<TType> : TagsPair<Candidate<TType>>
    {
        public Match(
            Candidate<TType> expected,
            Candidate<TType> actual)
            : base(expected, actual) { }

        protected override string Render(Candidate<TType> tag, Func<Candidate<TType>, string> renderer)
        {
            return string.Format("{0} - {1}, <{2}:{3}> : {4}", tag.Tag.Begin, tag.Tag.End, tag.Tag.Type, tag.Score, renderer(tag));
        }
    }
}