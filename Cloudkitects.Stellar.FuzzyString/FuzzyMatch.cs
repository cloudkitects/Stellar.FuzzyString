namespace Cloudkitects.Stellar.FuzzyString
{
    /// <summary>
    /// Comparison result class.
    /// </summary>
    public class FuzzyMatch
    {
        public int Index { get; internal set; }
        public double Similarity { get; internal set; }
        public string Source { get; internal set; }
        public string Target { get; internal set; }
    }
}
