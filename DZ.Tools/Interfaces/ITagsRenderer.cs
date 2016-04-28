using System.Collections.Generic;

namespace DZ.Tools.Interfaces
{
    /// <summary>
    /// Tags renderer interface
    /// </summary>
    /// <typeparam name="TTag"></typeparam>
    public interface ITagsRenderer<TTag>
    {
        /// <summary>
        /// Renders list of tags using input <paramref name="text"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ranges"></param>
        /// <returns></returns>
        string Render(string text, List<Tag<TTag>> ranges);
    }
}