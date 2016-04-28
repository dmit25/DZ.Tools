using System;
using System.Collections.Generic;
using DZ.Tools.Interfaces;
using JetBrains.Annotations;

namespace DZ.Tools
{
    /// <summary>
    /// Contains invariants for NER primitives
    /// </summary>
    [PublicAPI]
    public class EnumsTagWorker<TType> : ITagsWorker<TType> where TType : struct
    {
        private readonly TType _undefinedValue;
        /// <summary>
        /// Type mappings that will be used during conversion from tag name into type
        /// </summary>
        protected readonly Dictionary<string, TType> Types;
        /// <summary>
        /// Type mappings that will be used during conversion from type to tag name
        /// </summary>
        protected readonly Dictionary<TType, string> Strings;
        private readonly HtmlRenderer<TType> _renderer;
        private readonly HtmlCorpusParser<TType> _parser;
        private readonly List<TType> _values;

        /// <summary>
        /// Creates new tags worker
        /// </summary>
        /// <param name="undefinedValue">value that should be used as undefined</param>
        /// <param name="valuesComparer">comparer that will be used to handle enum values</param>
        public EnumsTagWorker(
            TType undefinedValue,
            [NotNull]
            Func<TType, TType, int> valuesComparer
            )
        {
            _undefinedValue = undefinedValue;
            Types = Enumers.GetEnumValuesDictionary<TType>();
            Strings = Enumers.GetEnumStringValuesDictionary<TType>();
            _renderer = new HtmlRenderer<TType>((e, s) => Strings[e.Type], valuesComparer);
            _values = Enumers.Values<TType>();
            _parser = new HtmlEntitiesParser(Types);
        }

        /// <summary>
        /// Tags renderer
        /// </summary>
        public ITagsRenderer<TType> Renderer { get { return _renderer; } }

        /// <summary>
        /// Corpus parser
        /// </summary>
        public HtmlCorpusParser<TType> Parser { get { return _parser; } }

        /// <summary>
        /// Possible NER Values
        /// </summary>
        public List<TType> Values { get { return _values; } }

        /// <summary>
        /// Default undefined value
        /// </summary>
        public TType Undefined { get { return _undefinedValue; } }

        /// <summary>
        /// Parses entities from html corpus
        /// </summary>
        sealed class HtmlEntitiesParser : HtmlCorpusParser<TType>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            /// <param name="types"></param>
            public HtmlEntitiesParser(Dictionary<string, TType> types)
                : base(tagBuilder =>
                {
                    TType res;
                    var tag = tagBuilder.ToString().Trim();
                    if (!types.TryGetValue(tag, out res))
                    {
                        throw new Exception("Unrecognized token:" + tag);
                    }
                    return res;
                })
            { }
        }
    }
}