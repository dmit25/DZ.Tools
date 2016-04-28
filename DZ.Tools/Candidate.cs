namespace DZ.Tools
{
    /// <summary>
    /// Match candidate class
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public class Candidate<TType>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Candidate(Tag<TType> tag, double score = 1.0)
        {
            Tag = tag;
            Score = score;
        }

        /// <summary>
        /// Candidate tag
        /// </summary>
        public Tag<TType> Tag { get; private set; }
        /// <summary>
        /// Candidate score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString() { return Tag == null ? Const.Null : Tag.ToString(); }
    }
}