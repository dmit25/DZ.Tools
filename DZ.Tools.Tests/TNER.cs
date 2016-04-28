using System;
using System.Collections.Generic;
using System.Text;
using DZ.Tools.Interfaces;

namespace DZ.Tools.Tests
{
    /// <summary>
    /// Ner entity type
    /// </summary>
    public enum TNER
    {
        /// <summary>
        /// Undefined type
        /// </summary>
        U,
        /// <summary>
        /// Person type
        /// </summary>
        P,
        /// <summary>
        /// Organization type
        /// </summary>
        O,
        /// <summary>
        /// Product type
        /// </summary>
        R,
        /// <summary>
        /// Event type
        /// </summary>
        E,
        /// <summary>
        /// Location type
        /// </summary>
        L
    }

    /// <summary>
    /// Parses entities from csv corpus
    /// </summary>
    public sealed class CsvEntitiesParser : CsvCorpusParser<TNER>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CsvEntitiesParser()
            : base(GetEntityType, t => t != TNER.U)
        {
        }

        private static TNER GetEntityType(string s)
        {
            switch (s)
            {
                case "I-LOC":
                case "B-LOC":
                case "S-LOC":
                case "E-LOC":
                    return TNER.L;
                case "I-PER":
                case "B-PER":
                case "S-PER":
                case "E-PER":
                    return TNER.P;
                case "I-ORG":
                case "B-ORG":
                case "S-ORG":
                case "E-ORG":
                    return TNER.O;
                case "O":
                case "I-MISC":
                case "B-MISC":
                case "S-MISC":
                case "E-MISC":
                    return TNER.U;
                default:
                    throw new ArgumentException("Unknown type " + s);
            }
        }
    }

    /// <summary>
    /// Contains invariants for NER primitives
    /// </summary>
    public class NERWorker : ITagsWorker<TNER>
    {
        /// <summary>
        /// Types mappings
        /// </summary>
        public static readonly Dictionary<string, TNER> Types = Enumers.GetEnumValuesDictionary<TNER>();
        private static readonly Dictionary<TNER, string> _Strings = Enumers.GetEnumStringValuesDictionary<TNER>();
        private static readonly HtmlRenderer<TNER> _Renderer = new HtmlRenderer<TNER>((e, s) => _Strings[e.Type], (x, y) => x.CompareTo(y));
        private static readonly HtmlCorpusParser<TNER> _Parser = new HtmlEntitiesParser();
        private static readonly List<TNER> _Values = Enumers.Values<TNER>();
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static readonly NERWorker I = new NERWorker();

        static NERWorker()
        {
            Types.Add("NAME", TNER.P);
            Types.Add("ORG", TNER.O);
            Types.Add("GEO", TNER.L);
            Types.Add("ENTR", TNER.E);
            Types.Add("PROD", TNER.R);
        }

        /// <summary>
        /// Tags renderer
        /// </summary>
        public HtmlRenderer<TNER> Renderer { get { return _Renderer; } }

        /// <summary>
        /// Corpus parser
        /// </summary>
        public HtmlCorpusParser<TNER> Parser { get { return _Parser; } }

        /// <summary>
        /// Possible NER Values
        /// </summary>
        public List<TNER> Values { get { return _Values; } }

        /// <summary>
        /// Default undefined value
        /// </summary>
        public TNER Undefined { get { return TNER.U; } }

        /// <summary>
        /// Parses entities from html corpus
        /// </summary>
        sealed class HtmlEntitiesParser : HtmlCorpusParser<TNER>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public HtmlEntitiesParser()
                : base(ParseType)
            {
            }

            /// <summary>
            /// Parses string into ner entity type
            /// </summary>
            /// <param name="tagBuilder"></param>
            /// <returns></returns>
            private static TNER ParseType(StringBuilder tagBuilder)
            {
                TNER res;
                var tag = tagBuilder.ToString().Trim();
                if (!Types.TryGetValue(tag, out res))
                {
                    throw new Exception("Unrecognized token:" + tag);
                }
                return res;
            }
        }
    }
}