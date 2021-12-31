namespace Stellar.FuzzyString
{
    /// <summary>
    /// Comparison result class.
    /// </summary>
    public class FuzzyMatch
    {
        /// <summary>
        /// The zero-based position of a match within an array.
        /// </summary>
        public int Index { get; internal set; }
        
        /// <summary>
        /// The computed similarity between source and target.
        /// </summary>
        public double Similarity { get; internal set; }

        /// <summary>
        /// The string to be compared to the target.
        /// </summary>
        public string Source { get; internal set; }

        /// <summary>
        /// The string to be compared to the source.
        /// </summary>
        public string Target { get; internal set; }
    }
}
