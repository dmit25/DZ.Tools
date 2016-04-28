using System;

namespace DZ.Tools
{
    public class Mismatch<TType> : TagsPair<Tag<TType>>
    {
        private readonly TType _undefinedValue;

        public Mismatch(
            TType undefinedValue,
            Tag<TType> expected,
            Tag<TType> actual = null)
            : base(expected, actual) { _undefinedValue = undefinedValue; }

        public TType ExpectedType { get { return Expected != null ? Expected.Type : _undefinedValue; } }
        public TType ActualType { get { return Actual != null ? Actual.Type : _undefinedValue; } }

        protected override string Render(Tag<TType> tag, Func<Tag<TType>, string> renderer)
        {
            return string.Format("{0} - {1}, <{2}> : {3}", tag.Begin, tag.End, tag.Type, renderer(tag));
        }
    }
}