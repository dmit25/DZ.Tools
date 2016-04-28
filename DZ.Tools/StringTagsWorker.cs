using System;
using System.Collections.Generic;
using System.Linq;
using DZ.Tools.Interfaces;
using JetBrains.Annotations;

namespace DZ.Tools
{
    /// <summary>
    /// Tags worker that uses string values as types
    /// </summary>
    public class StringTagsWorker : ITagsWorker<string>
    {
        private readonly HtmlRenderer<string> _renderer;
        private readonly HtmlCorpusParser<string> _parser;
        private readonly List<string> _values;
        private readonly string _undefinedTagName;

        /// <summary>
        /// Creates new tags worker
        /// </summary>
        /// <param name="tagsNames">names of supported tags (should also contain undefined tag name)</param>
        /// <param name="undefinedTagName">name of undefined tag</param>
        public StringTagsWorker(
            [NotNull] IEnumerable<string> tagsNames,
            [NotNull] string undefinedTagName)
        {
            _renderer = new HtmlRenderer<string>((t, b) => t.Type, (s1, s2) => string.Compare(s1, s2, StringComparison.Ordinal));
            _values = tagsNames.ToList();
            _undefinedTagName = undefinedTagName;
            _parser = new HtmlParser(new HashSet<string>(_values));
        }

        /// <summary>
        ///     Tags renderer
        /// </summary>
        public ITagsRenderer<string> Renderer { get { return _renderer; } }

        /// <summary>
        ///     Corpus parser
        /// </summary>
        public HtmlCorpusParser<string> Parser { get { return _parser; } }

        /// <summary>
        ///     Possible NER Values
        /// </summary>
        public List<string> Values { get { return _values; } }

        /// <summary>
        ///     Default undefined value
        /// </summary>
        public string Undefined { get { return _undefinedTagName; } }

        /// <summary>
        ///     Parses entities from html corpus
        /// </summary>
        private sealed class HtmlParser : HtmlCorpusParser<string>
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
            /// </summary>
            /// <param name="hashSet"></param>
            public HtmlParser(HashSet<string> hashSet)
                : base(b =>
                {
                    var name = b.ToString();
                    if (hashSet.Contains(name))
                    {
                        return name;
                    }
                    throw new Exception("Unrecognized tag [{0}]".FormatWith(name));
                })
            { }
        }
    }
}