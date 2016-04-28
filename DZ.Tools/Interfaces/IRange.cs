namespace DZ.Tools.Interfaces
{
    /// <summary>
    /// Represents positions range abstraction
    /// </summary>
    public interface IRange
    {
        /// <summary>
        /// Start position
        /// </summary>
        int Begin { get; }

        /// <summary>
        /// End position
        /// </summary>
        int End { get; }
    }
}