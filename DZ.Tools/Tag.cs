using System.Diagnostics;
using DZ.Tools.Interfaces;

namespace DZ.Tools
{
    /// <summary>
    /// Represents text tag (typed range in text)
    /// </summary>
    /// <typeparam name="TTag"></typeparam>
    [DebuggerDisplay("{Begin} - {End}, <{Type}>")]
    public class Tag<TTag> : IRange
    {
        /// <summary>
        /// Absolute start index in the text
        /// </summary>
        public int Begin { get; set; }
        /// <summary>
        /// Absolute end index in the text
        /// </summary>
        public int End { get; set; }
        /// <summary>
        /// Type of the tag
        /// </summary>
        public TTag Type { get; set; }
        /// <summary>
        /// Parent of this tag
        /// </summary>
        public Tag<TTag> Parent { get; set; }
        /// <summary>
        /// Creates new Tag
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        public Tag(int start, int end, TTag type)
        {
            Begin = start;
            End = end;
            Type = type;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0} - {1}, <{2}>", Begin, End, Type);
        }
    }
}