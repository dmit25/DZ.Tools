using System;

namespace DZ.Tools
{
    public abstract class TagsPair<T> where T : class
    {
        public TagsPair(
            T expected,
            T actual = null)
        {
            Expected = expected;
            Actual = actual;
        }

        public T Expected { get; private set; }
        public T Actual { get; private set; }

        /// <summary>
        /// Returns descriptive string that represents this pair
        /// </summary>
        /// <param name="renderer">renderer that will be used to get components string representations</param>
        /// <returns></returns>
        public string Render(Func<T, string> renderer)
        {
            return string.Format(
                "'{0}' <<>> '{1}'",
                Expected == null ? Const.Null : Render(Expected, renderer),
                Actual == null ? Const.Null : Render(Actual, renderer));
        }

        /// <summary>
        /// Returns descriptive string that represents <paramref name="tag"/>
        /// </summary>
        protected virtual string Render(T tag, Func<T, string> renderer) { return renderer(tag); }
    }
}