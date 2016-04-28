using System.Collections.Generic;
using System.Linq;
using DZ.Tools.Interfaces;

namespace DZ.Tools
{
    /// <summary>
    /// Represents training model data container
    /// </summary>
    public sealed class TagsCorpus<TTag>
    {
        /// <summary>
        /// Creates new instance
        /// </summary>
        public TagsCorpus(List<Tag<TTag>> tags = null)
        {
            Tags = tags ?? new List<Tag<TTag>>();
            ClearedText = string.Empty;
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        public TagsCorpus(params Tag<TTag>[] tags)
        {
            Tags = tags.ToList();
            ClearedText = string.Empty;
        }

        /// <summary>
        /// List of tags in model sorted by position
        /// </summary>
        public List<Tag<TTag>> Tags { get; private set; }

        /// <summary>
        /// Cleared text that should be input for the training
        /// </summary>
        public string ClearedText { get; set; }

        /// <summary>
        /// Renders model into string
        /// </summary>
        /// <returns></returns>
        public string Render(ITagsRenderer<TTag> renderer)
        {
            return renderer.Render(ClearedText, Tags);
        }
    }
}