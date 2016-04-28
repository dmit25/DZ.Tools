using System;
using System.Collections.Generic;
using System.Text;
using DZ.Tools.Interfaces;

namespace DZ.Tools
{
    /// <summary>
    /// Training model parser
    /// </summary>
    public class HtmlCorpusParser<TTag> : ICorpusParser<TTag>
    {
        private readonly Func<StringBuilder, TTag> _tagTypeParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public HtmlCorpusParser(Func<StringBuilder, TTag> tagTypeParser)
        {
            _tagTypeParser = tagTypeParser;
        }

        /// <summary>
        /// Parses model string representation into training model
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxTagLength">Maximum tag length</param>
        /// <returns></returns>
        public TagsCorpus<TTag> Parse(string input, int maxTagLength = -1)
        {
            var model = new TagsCorpus<TTag>();
            var context = new Stack<TagPart<TTag>>();
            var state = ParsingState.Text;
            var tagBuilder = new StringBuilder(8);
            var builder = new StringBuilder(input.Length);
            var i = 0;
            var pos = 0;
            var lineNumber = 0;
            for (; i < input.Length; i++)
            {
                try
                {
                    var c = input[i];
                    switch (c)
                    {
                        case '<':
                            switch (state)
                            {
                                case ParsingState.Text:
                                    state = ParsingState.OpenTag;
                                    context.Push(new TagPart<TTag> { Start = pos, State = ParsingState.OpenTag, ModelStart = i, Line = lineNumber });
                                    break;
                                case ParsingState.CloseTag:
                                    throw new ModelParsingException<TTag>("Incorrect markup", lineNumber, pos, i, builder, context);
                                default:
                                    throw new ModelParsingException<TTag>("Incorrect state={0}".FormatWith(state), lineNumber, pos, i, builder, context);
                            }
                            break;
                        case '/':
                            switch (state)
                            {
                                case ParsingState.OpenTag:
                                    state = ParsingState.CloseTag;
                                    context.Peek().State = ParsingState.CloseTag;
                                    break;
                                default:
                                    builder.Append(c);
                                    pos++;
                                    break;
                            }
                            break;
                        case '>':
                            switch (state)
                            {
                                case ParsingState.OpenTag:
                                case ParsingState.CloseTag:
                                case ParsingState.InsideTagAfterTagName:
                                    var last = context.Peek();
                                    last.Type = _tagTypeParser(tagBuilder);
                                    tagBuilder.Clear();
                                    last.End = pos;
                                    state = ParsingState.Text;
                                    TryMergeTags(context, model.Tags, maxTagLength);
                                    break;
                                default:
                                    builder.Append(c);
                                    pos++;
                                    break;
                            }
                            break;
                        default:
                            switch (state)
                            {
                                case ParsingState.OpenTag:
                                case ParsingState.CloseTag:
                                    if (char.IsWhiteSpace(c))
                                    {
                                        state = ParsingState.InsideTagAfterTagName;
                                    }
                                    else
                                    {
                                        tagBuilder.Append(c);
                                    }
                                    break;
                                case ParsingState.InsideTagAfterTagName:
                                    break;
                                default:
                                    if (c == '\n')
                                    {
                                        lineNumber++;
                                    }
                                    builder.Append(c);
                                    pos++;
                                    break;
                            }
                            break;
                    }
                }
                catch (Exception exc)
                {
                    throw new ModelParsingException<TTag>(lineNumber, pos, i, builder, context, exc);
                }
            }
            if (context.Count > 0)
            {
                var last = context.Pop();
                throw new ModelParsingException<TTag>(last, GetText(builder, last.Start));
            }
            model.ClearedText = builder.ToString();
            return model;
        }

        private static string GetText(StringBuilder builder, int start)
        {
            var end = Math.Min(start + 200, builder.Length - 1);
            return builder.ToString(start, end - start);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tags"></param>
        /// <param name="maxTagLength"></param>
        private static void TryMergeTags(Stack<TagPart<TTag>> context, List<Tag<TTag>> tags, int maxTagLength)
        {
            if (context.Count > 1)
            {
                var last = context.Pop();
                var beforeLast = context.Pop();
                if (last.Type.Equals(beforeLast.Type) && last.State == ParsingState.CloseTag &&
                    beforeLast.State == ParsingState.OpenTag)
                {
                    var length = last.End - beforeLast.Start;
                    if (maxTagLength > 0 && length > maxTagLength)
                    {
                        throw new Exception("Tag longer than allowed: " + length);
                    }
                    var entity = new Tag<TTag>(beforeLast.Start, last.End, last.Type);
                    if (beforeLast.Children != null)
                    {
                        foreach (var child in beforeLast.Children)
                        {
                            child.Parent = entity;
                        }
                    }
                    if (context.Count > 0)
                    {
                        var parent = context.Peek();
                        var list = parent.Children ?? (parent.Children = new List<Tag<TTag>>());
                        list.Add(entity);
                    }
                    tags.Add(entity);
                }
                else
                {
                    context.Push(beforeLast);
                    context.Push(last);
                }

            }
        }

        internal class TagPart<TType>
        {
            public int Line { get; set; }
            public int ModelStart { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
            public TType Type { get; set; }
            public ParsingState State { get; set; }
            public List<Tag<TType>> Children { get; set; }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            /// A string that represents the current object.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{0}-{1} <{2}> <{3}>", Start, End, Type, State);
            }
        }

        /// <summary>
        /// Tag parsing state
        /// </summary>
        internal enum ParsingState
        {
            /// <summary>
            /// In the text
            /// </summary>
            Text,
            /// <summary>
            /// Open tag
            /// </summary>
            OpenTag,
            /// <summary>
            /// Closing tag
            /// </summary>
            CloseTag,
            /// <summary>
            /// Inside tag
            /// </summary>
            InsideTagAfterTagName
        }
    }
}