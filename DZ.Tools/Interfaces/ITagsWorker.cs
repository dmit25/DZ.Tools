using System.Collections.Generic;
using JetBrains.Annotations;

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
        [NotNull]
        ITagsRenderer<TType> Renderer { get; }

        /// <summary>
        /// Corpus parser
        /// </summary>
        [NotNull]
        HtmlCorpusParser<TType> Parser { get; }

        /// <summary>
        /// Possible NER Values
        /// </summary>
        [NotNull]
        List<TType> Values { get; }

        /// <summary>
        /// Default undefined value
        /// </summary>
        [NotNull]
        TType Undefined { get; }
    }
}