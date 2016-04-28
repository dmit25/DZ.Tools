using System;
using System.Collections.Generic;
using System.Text;
using DZ.Tools.Interfaces;

namespace DZ.Tools
{
    /// <summary>
    /// Html renderer that converts list of text ranges and input text into marked html text
    /// </summary>
    /// <typeparam name="TTag">tag type</typeparam>
    public sealed class HtmlRenderer<TTag> : ITagsRenderer<TTag>
    {
        private readonly Func<Tag<TTag>, bool, string> _tagGenerator;
        private readonly StartPositionComparer _startComparer;
        private readonly EndPositionComparer _endComparer;

        /// <summary>
        /// Creates new tag renderer
        /// </summary>
        /// <param name="tagGenerator">function that will be used to generate tag inner text for each tag
        /// first argument - tag source
        /// second argument - flag that indicates whenever tag is starting (start - true, false - ending tag)</param>
        /// <param name="typeComparer">compares types of tags</param>
        public HtmlRenderer(Func<Tag<TTag>, bool, string> tagGenerator, Func<TTag, TTag, int> typeComparer)
        {
            _tagGenerator = tagGenerator.ThrowIfNull("tagGenerator");
            _startComparer = new StartPositionComparer(typeComparer);
            _endComparer = new EndPositionComparer(typeComparer);
        }

        /// <summary>
        /// Renders
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public string Render(string text, List<Tag<TTag>> ranges)
        {
            var resultBuilder = new StringBuilder(text.Length * 2);
            var startsDictionary = new Dictionary<int, List<Tag<TTag>>>();
            var endsDictionary = new Dictionary<int, List<Tag<TTag>>>();
            InitializeDictionaries(startsDictionary, endsDictionary, ranges);
            for (int i = 0; i <= text.Length; i++)
            {
                List<Tag<TTag>> list;

                while (endsDictionary.TryGetValue(i, out list))
                {
                    resultBuilder.Append("</").Append(_tagGenerator(list[0], false)).Append(">");
                    list.RemoveAt(0);
                    if (list.Count == 0)
                    {
                        endsDictionary.Remove(i);
                    }
                }
                while (startsDictionary.TryGetValue(i, out list))
                {

                    resultBuilder.Append("<").Append(_tagGenerator(list[0], true)).Append(">");
                    list.RemoveAt(0);
                    if (list.Count == 0)
                    {
                        startsDictionary.Remove(i);
                    }
                }
                if (i < text.Length)
                {
                    resultBuilder.Append(text[i]);
                }
            }
            return resultBuilder.ToString();
        }

        private void InitializeDictionaries(
            Dictionary<int, List<Tag<TTag>>> startsDictionary,
            Dictionary<int, List<Tag<TTag>>> endsDictionary,
            List<Tag<TTag>> ranges)
        {
            for (int i = 0; i < ranges.Count; i++)
            {
                var entity = ranges[i];
                List<Tag<TTag>> list;
                if (startsDictionary.TryGetValue(entity.Begin, out list))
                {
                    list.Add(entity);
                    list.Sort(_endComparer);
                }
                else
                {
                    startsDictionary.Add(entity.Begin, new List<Tag<TTag>> { entity });
                }

                if (endsDictionary.TryGetValue(entity.End, out list))
                {
                    list.Add(entity);
                    list.Sort(_startComparer);
                }
                else
                {
                    endsDictionary.Add(entity.End, new List<Tag<TTag>> { entity });
                }
            }
        }

        private class EndPositionComparer : IComparer<Tag<TTag>>
        {
            private readonly Func<TTag, TTag, int> _typeComparer;

            public EndPositionComparer(Func<TTag, TTag, int> typeComparer)
            {
                _typeComparer = typeComparer;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(Tag<TTag> x, Tag<TTag> y)
            {
                var res = y.End.CompareTo(x.End);
                if (res == 0)
                {
                    return _typeComparer(y.Type, x.Type);
                }
                return res;
            }
        }

        private class StartPositionComparer : IComparer<Tag<TTag>>
        {
            private readonly Func<TTag, TTag, int> _typeComparer;

            public StartPositionComparer(Func<TTag, TTag, int> typeComparer)
            {
                _typeComparer = typeComparer;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(Tag<TTag> x, Tag<TTag> y)
            {
                var res = y.Begin.CompareTo(x.Begin);
                if (res == 0)
                {
                    return _typeComparer(x.Type, y.Type);
                }
                return res;
            }
        }
    }
}