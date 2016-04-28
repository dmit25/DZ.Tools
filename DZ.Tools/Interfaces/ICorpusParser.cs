namespace DZ.Tools.Interfaces
{
    /// <summary>
    /// Represents training model parser
    /// </summary>
    public interface ICorpusParser<TTag>
    {
        /// <summary>
        /// Parses model string representation into training model
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxTagLength">Maximum tag length</param>
        /// <returns></returns>
        TagsCorpus<TTag> Parse(string input, int maxTagLength = -1);
    }
}