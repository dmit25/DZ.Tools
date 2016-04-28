using System;
using System.Collections.Generic;
using System.Text;

namespace DZ.Tools
{
    /// <summary>
    /// model parsing exception
    /// </summary>
    public class ModelParsingException<TTag> : Exception
    {
        internal ModelParsingException(HtmlCorpusParser<TTag>.TagPart<TTag> last, string lastText)
            : base(
                "Unclosed tags:" + last + "\nline:" + last.Line + "\ntext:" + lastText)
        {
            TextPos = last.Start;
            Pos = last.ModelStart;
            Line = last.Line;
        }

        internal ModelParsingException(int line, int textPos, int pos, StringBuilder text, Stack<HtmlCorpusParser<TTag>.TagPart<TTag>> context, Exception exc)
            : base("Error:'{0}'\nline:{1}, textPos:{2}, pos:{3}\ntext:{4}\ncontext:{5}".FormatWith(
                exc.Message,
                line,
                textPos,
                pos,
                GetLastText(text),
                RenderContext(context)), exc)
        {
            TextPos = textPos;
            Pos = pos;
            Line = line;
        }

        internal ModelParsingException(string message, int line, int textPos, int pos, StringBuilder text, Stack<HtmlCorpusParser<TTag>.TagPart<TTag>> context)
            : base("Error:'{0}'\nline:{1}, textPos:{2}, pos:{3}\ntext:{4}\ncontext:{5}".FormatWith(
                message,
                line,
                textPos,
                pos,
                GetLastText(text),
                RenderContext(context)))
        {
            TextPos = textPos;
            Pos = pos;
            Line = line;
        }

        private static string RenderContext(Stack<HtmlCorpusParser<TTag>.TagPart<TTag>> context)
        {
            return context.AggregateString((acc, p) => acc.AppendLine(p.ToString()));
        }

        private static string GetLastText(StringBuilder text)
        {
            if (text.Length < 200)
            {
                return text.ToString();
            }
            return text.ToString(text.Length - 201, 200);
        }

        /// <summary>
        /// Position in text where error occured
        /// </summary>
        public int TextPos { get; set; }
        /// <summary>
        /// Line in text/model where error occurred
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// Position in model where error occured
        /// </summary>
        public int Pos { get; set; }
        /// <summary>
        /// Error context text
        /// </summary>
        public string LastText { get; set; }
    }
}