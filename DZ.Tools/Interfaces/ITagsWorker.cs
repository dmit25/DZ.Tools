using System.Collections.Generic;

namespace DZ.Tools.Interfaces
{
    /// <summary>
    /// Tags worker interface
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public interface ITagsWorker<TType>
    {
        /// <summary>
        /// Tags renderer
        /// </summary>
        HtmlRenderer<TType> Renderer { get; }

        /// <summary>
        /// Corpus parser
        /// </summary>
        HtmlCorpusParser<TType> Parser { get; }

        /// <summary>
        /// Possible NER Values
        /// </summary>
        List<TType> Values { get; }

        /// <summary>
        /// Default undefined value
        /// </summary>
        TType Undefined { get; }
    }
}