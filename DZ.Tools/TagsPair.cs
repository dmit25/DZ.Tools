using System;

namespace DZ.Tools
{
    /// <summary>
    /// Container for expected/actual tags pair
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TagsPair<T> where T : class
    {
        /// <summary>
        /// Creates new pair
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public TagsPair(
            T expected,
            T actual = null)
        {
            Expected = expected;
            Actual = actual;
        }

        /// <summary>
        /// Expected tag
        /// </summary>
        public T Expected { get; private set; }
        /// <summary>
        /// Actual tag
        /// </summary>
        public T Actual { get; private set; }

        /// <summary>
        /// Returns descriptive string that represents this pair
        /// </summary>
        /// <param name="renderer">renderer that will be used to get components string representations</param>
        /// <returns></returns>
        public string Render(Func<T, string> renderer)
        {
            return "'{0}' <<>> '{1}'".FormatWith(
                Expected == null ? Const.Null : Render(Expected, renderer),
                Actual == null ? Const.Null : Render(Actual, renderer));
        }

        /// <summary>
        /// Returns descriptive string that represents <paramref name="tag"/>
        /// </summary>
        protected virtual string Render(T tag, Func<T, string> renderer) { return renderer(tag); }
    }
}