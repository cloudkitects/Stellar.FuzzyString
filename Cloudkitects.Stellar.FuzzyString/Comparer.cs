// Fuzzy Logic String Comparer based on Kevin D. Jones'
// fuzzy string, Approximate String Comparison in C#
// Sep 20, 2017, https://github.com/kdjones/fuzzystring, 

//License

// The Eclipse Foundation makes available all content in this plug-in ("Content").
// Unless otherwise indicated below, the Content is provided to you under the terms
// and conditions of the Eclipse Public License Version 1.0 ("EPL"). A copy of the EPL
// is available at http://www.eclipse.org/legal/epl-v10.html. For purposes of the
// EPL, "Program" will mean the Content.

// If you did not receive this Content directly from the Eclipse Foundation, the Content
// is being redistributed by another party ("Re-distributor") and different terms and conditions
// may apply to your use of any object code in the Content. Check the Re-distributor's license
// that was provided with the Content. If no such license exists, contact the Re-distributor.
// Unless otherwise indicated below, the terms and conditions of the EPL still apply to any
// source code in the Content and such source code may be obtained at http://www.eclipse.org.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Cloudkitects.Stellar.FuzzyString
{
    public static class Comparer
    {
        #region public methods

        #region Matches

        /// <summary>
        /// Return the closest fuzzy string comparison
        /// match to a source string given one or
        /// more target strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="targets">The target string collection.</param>
        /// <param name="comparisonOptions">Similarity comparison options.</param>
        /// <returns></returns>
        public static FuzzyMatch BestMatch(
            string source,
            string[] targets,
            ComparisonOptions comparisonOptions = ComparisonOptions.UseJaroWinklerSimilarity |
                                                  ComparisonOptions.UseLevenshteinSimilarity)
        {
            if (targets == null || targets.Length == 0)
            {
                return null;
            }

            return Matches(source, targets, comparisonOptions)
                .Aggregate((a, b) => a.Similarity > b.Similarity ? a : b);
        }

        /// <summary>
        /// Return a collection of fuzzy string comparison
        /// matches between a source string and one or
        /// more target strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="targets">The target string collection.</param>
        /// <param name="comparisonOptions">Similarity comparison options.</param>
        /// <returns></returns>
        public static IEnumerable<FuzzyMatch> Matches(
            string source,
            string[] targets,
            ComparisonOptions comparisonOptions = ComparisonOptions.UseJaroWinklerSimilarity |
                                                  ComparisonOptions.UseJaroWinklerSimilarity)
        {
            if (targets == null || targets.Length == 0)
            {
                return null;
            }

            return targets
                .Select((target, index) => new FuzzyMatch
                {
                    Index = index,
                    Similarity = SimilarityScore(source, target, comparisonOptions),
                    Source = source,
                    Target = target
                });
        }
        #endregion

        #region approximately equals
        /// <summary>
        /// Determine whether a string is similar to another, based on their
        /// similarity score.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The target string.</param>
        /// <param name="options">Algorithms and options to use in the comparison.</param>
        /// <param name="threshold">The value to exceed to consider the two strings similar.</param>
        /// <returns>Whether source is similar to target.</returns>
        [UsedImplicitly]
        public static bool ApproximatelyEquals(
            string source,
            string target,
            ComparisonOptions options = ComparisonOptions.UseAll,
            double threshold = 0.75)
        {
            return SimilarityScore(source, target, options) >= Math.Max(0.01, Math.Min(threshold, 1));
        }
        #endregion

        #region Similarity
        public static double SimilarityScore(
            string source,
            string target,
            ComparisonOptions options = ComparisonOptions.UseAll)
        {
            var results = new List<double>();

            if ((options & ComparisonOptions.CaseInsensitive) != 0)
            {
                source = source.ToUpper();
                target = target.ToUpper();
            }

            if ((options & ComparisonOptions.UseEditSimilarity) != 0)
            {
                results.Add(EditSimilarity(source, target));
            }

            if ((options & ComparisonOptions.UseHammingSimilarity) != 0)
            {
                results.Add(HammingSimilarity(source, target));
            }

            if ((options & ComparisonOptions.UseJaccardIndex) != 0)
            {
                results.Add(JaccardIndex(source, target));
            }

            if ((options & ComparisonOptions.UseJaroWinklerSimilarity) != 0)
            {
                results.Add(JaroWinklerSimilarity(source, target));
            }

            if ((options & ComparisonOptions.UseLevenshteinSimilarity) != 0)
            {
                results.Add(LevenshteinSimilarity(source, target));
            }

            if ((options & ComparisonOptions.UseLongestCommonSubsequence) != 0)
            {
                results.Add(LongestCommonSubsequenceSimilarity(source, target));
            }

            if ((options & ComparisonOptions.UseLongestCommonSubstring) != 0)
            {
                results.Add(LongestCommonSubstringSimilarity(source, target));
            }

            if ((options & ComparisonOptions.UseOverlapCoefficient) != 0)
            {
                results.Add(OverlapCoefficient(source, target));
            }

            if ((options & ComparisonOptions.UseRatcliffObershelpSimilarity) != 0)
            {
                results.Add(RatcliffObershelpSimilarity(source, target));
            }

            if ((options & ComparisonOptions.UseSorensenDiceCoefficient) != 0)
            {
                results.Add(SorensenDiceCoefficient(source, target));
            }

            if (results.Count == 0)
            {
                throw new ArgumentException("You must specify at least one comparison algorithm option.");
            }

            return results.Average();
        }

        /// <summary>
        /// Compute the similarity score between two strings of equal length as
        /// the length's complement of the Edit distance.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The edit similarity between source and target strings as a double.</returns>
        public static double EditSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            var d = EditDistance(source, target);
            var l = Math.Max(source.Length, target.Length);

            return 1 - d / l;
        }

        /// <summary>
        /// Compute the similarity score between two strings of equal length as
        /// the 1's complement of the Hamming distance relative to the strings' length.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The Hamming similarity between source and target strings as a double.</returns>
        public static double HammingSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target) || source.Length != target.Length)
            {
                return 0;
            }

            var d = HammingDistance(source, target);

            return 1 - d / source.Length;
        }

        /// <summary>
        /// Compute the similarity coefficient between two strings as the size of the
        /// intersection divided by the size of the union of the strings.
        /// <see cref="http://en.wikipedia.org/wiki/Jaccard_index">Jaccard Distance</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The Jaccard similarity coefficient (score) between source and target
        /// strings as a double.</returns>
        public static double JaccardIndex(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            if (source.Equals(target))
            {
                return 1;
            }

            double i = source.Intersect(target).Count();

            return i / (source.Length + target.Length - i);
        }

        /// <summary>
        /// Compute the Jaro Similarity between two strings based on the length of the strings,
        /// the number of matching characters m and half the number of transpositions t.
        /// <see cref="http://en.wikipedia.org/wiki/Jaro-Winkler_distance#Jaro_Similarity">Jaro Similarity</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The Jaro similarity score between source and target strings as a double.</returns>
        /// <remarks>In this implementation, characters are considered matching if they're the same,
        /// independently of how far apart they are.</remarks>
        public static double JaroSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            if (source.Equals(target))
            {
                return 1;
            }

            var sl = source.Length;
            var tl = target.Length;

            var r = Math.Max(0, Math.Max(sl, tl) / 2 - 1);

            var sm = new bool[sl];
            var tm = new bool[tl];

            double m = 0;

            for (var i = 0; i < sl; ++i)
            {
                for (var j = Math.Max(0, i - r); j < Math.Min(i + r + 1, tl); ++j)
                {
                    if (tm[j] || source[i] != target[j])
                    {
                        continue;
                    }

                    sm[i] = true;
                    tm[j] = true;

                    ++m;

                    break;
                }
            }

            if (m < 1)
            {
                return 0;
            }

            double t = 0;
            var k = 0;

            for (var i = 0; i < sl; ++i)
            {
                if (!sm[i])
                {
                    continue;
                }

                while (!tm[k])
                {
                    ++k;
                }

                if (source[i] != target[k])
                {
                    ++t;
                }

                ++k;
            }

            t /= 2;

            return (m / sl + m / tl + (m - t) / m) / 3;
        }

        /// <summary>
        /// Compute an edit distance between two sequences as a variant of the Jaro distance metric
        /// using a prefix scale p which gives more favorable ratings to strings that match from
        /// the beginning for a set prefix length l. 
        /// <see cref="http://en.wikipedia.org/wiki/Jaro-Winkler_distance#Jaro-Winkler_Similarity">Jaro-Winkler Similarity</see>
        /// and <see cref="http://stackoverflow.com/questions/19123506"/>.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <param name="p">Prefix scale, clamped to [0, 0.25] with 0.1 default.</param>
        /// <returns>The Jaro-Winkler distance between source and target strings as a double.</returns>
        /// <remarks>In the original paper Winkler only applies the modification to
        /// Jaro similarity values above 0.7, and uses 4 as the maximum prefix length.</remarks>
        public static double JaroWinklerSimilarity(string source, string target, double p = 0.1)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            if (source.Equals(target))
            {
                return 1;
            }

            var s = JaroSimilarity(source, target);

            if (s <= 0.7)
            {
                return s;
            }

            p = Math.Max(0, Math.Min(p, 0.25));

            var m = Math.Min(4, Math.Min(source.Length, target.Length));
            var l = source.Substring(0, m).Where((c, i) => c.Equals(target[i])).Sum(c => 1d);

            return s + l * p * (1 - s);
        }

        /// <summary>
        /// Compute the normalized similarity score as the 1's complement of
        /// the Levenshtein distance.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The normalized similarity score based on the Levenshtein distance between
        /// the two input strings.</returns>
        public static double LevenshteinSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            var d = LevenshteinDistance(source, target);

            return 1 - d / Math.Max(source.Length, target.Length);
        }

        /// <summary>
        /// Compute the Longest Common Subsequence (LCS) Similarity between two strings.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The 1's complement of the LCS length divided by the shortest length
        /// of the two input strings.</returns>
        public static double LongestCommonSubsequenceSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            var lcsLength = Convert.ToDouble(LongestCommonSubsequence(source, target).Length);

            return lcsLength / Math.Min(source.Length, target.Length);
        }

        public static double LongestCommonSubstringSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            if (source.Equals(target))
            {
                return 1;
            }

            var lcsLength = Convert.ToDouble(LongestCommonSubstring(source, target).Length);

            return lcsLength / Math.Min(source.Length, target.Length);
        }

        /// <summary>
        /// Compute the overlap coefficient (or Szymkiewicz–Simpson coefficient),
        /// a measure of the overlap between two strings defined as the size of the
        /// intersection divided by the shortest length.
        /// <see cref="http://en.wikipedia.org/wiki/Overlap_coefficient">Overlap Coefficient</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The overlap between the two input strings.</returns>
        public static double OverlapCoefficient(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            if (source.Equals(target))
            {
                return 1;
            }

            double i = source.Intersect(target).Count();

            return i / Math.Min(source.Length, target.Length);
        }

        /// <summary>
        /// Compute the similarity of two strings as two times the number of matching characters divided
        /// by the total number of characters in the two strings.
        /// <see cref="http://www.morfoedro.it/doc.php?n=223&lang=en">Ratcliff-Obershelp Similarity</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The overlap between the two input strings.</returns>
        public static double RatcliffObershelpSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            if (source.Equals(target))
            {
                return 1;
            }

            var a = AllCommonSubstrings(source, target);

            double i = a.Sum(x => x.Length);

            return 2 * i / (source.Length + target.Length);
        }

        /// <summary>
        /// Compute the Sorensen-Dice coefficient, a semi-metric version of the Jaccard index.
        /// <see cref="http://en.wikipedia.org/wiki/Sørensen-Dice_coefficient">Sorensen-Dice Coefficient</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The similarity coefficient between the two input strings.</returns>
        public static double SorensenDiceCoefficient(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 1
                    : 0;
            }

            if (string.IsNullOrEmpty(target))
            {
                return 0;
            }

            if (source.Equals(target))
            {
                return 1;
            }

            double i = source.Intersect(target).Count();

            return 2 * i / (source.Length + target.Length);
        }
        #endregion

        #region Distance
        /// <summary>
        /// Compute the number of non-matching characters between two strings,
        /// a.k.a. the edit distance, based on the
        /// <see cref="http://en.wikipedia.org/wiki/Hamming_distance">Hamming Distance</see>
        /// up to the smaller length substring and any additions.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The edit distance between source and target strings as a double,
        /// including the number of edits and additions.</returns>
        public static double EditDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 0
                    : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            var n = Math.Min(source.Length, target.Length);
            var s = source.Substring(0, n);
            var t = target.Substring(0, n);

            return s.Where((c, i) => !c.Equals(t[i])).Sum(c => 1d) + Math.Abs(source.Length - target.Length);
        }

        /// <summary>
        /// Compute the Hamming distance between two strings, the number of substitutions
        /// required to convert one string into another.
        /// <see cref="http://en.wikipedia.org/wiki/Hamming_distance">Hamming Distance</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The Hamming distance between source and target strings as a double.</returns>
        public static double HammingDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 0
                    : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            if (source.Length != target.Length)
            {
                return Math.Max(source.Length, target.Length);
            }

            return source.Where((c, i) => !c.Equals(target[i])).Sum(c => 1d);
        }

        /// <summary>
        /// Compute the minimum number of single-character edits needed to
        /// change the source into the target, allowing insertions, deletions,
        /// and substitutions.
        /// <see cref="http://en.wikipedia.org/wiki/Levenshtein_distance">Levenshtein Distance</see> and
        /// <see cref="http://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Levenshtein_distance">
        /// Levenshtein Distance Algorithm Implementation</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The number of edits required to transform the source into the
        /// target, at most the length of the longest string, and at least
        /// the difference in length between the two strings.</returns>
        public static double LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target)
                    ? 0
                    : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            var m = source.Length;
            var n = target.Length;

            var d = new int[m + 1, n + 1];

            for (var i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }

            for (var j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }

            for (var i = 1; i <= m; i++)
            {
                for (var j = 1; j <= n; j++)
                {
                    var c = source[i - 1] != target[j - 1] ? 1 : 0;

                    d[i, j] = Math.Min(
                        Math.Min(
                            d[i - 1, j] + 1,
                            d[i, j - 1] + 1),
                        d[i - 1, j - 1] + c);
                }
            }

            return d[m, n];
        }

        /// <summary>
        /// Compute the Longest Common Subsequence (LCS) in two strings, not to be confused
        /// with Longest Common Substring computation.
        /// <see cref="http://en.wikipedia.org/wiki/Longest_common_subsequence_problem">Longest Common Subsequence</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The Longest Common Subsequence of the two input strings.</returns>
        /// <remarks>
        /// Builds the starting LCS length table on the fly.
        /// </remarks>
        public static string LongestCommonSubsequence(string source, string target)
        {
            if (string.IsNullOrEmpty(source) ||
                string.IsNullOrEmpty(target))
            {
                return string.Empty;
            }

            var m = source.Length;
            var n = target.Length;
            var lcs = new int[m + 1, n + 1];

            for (var i = 1; i <= m; i++)
            {
                for (var j = 1; j <= n; j++)
                {
                    if (source[i - 1].Equals(target[j - 1]))
                    {
                        lcs[i, j] = lcs[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        lcs[i, j] = Math.Max(lcs[i, j - 1], lcs[i - 1, j]);
                    }
                }
            }

            return Backtrack(source, target, lcs, m, n);
        }

        /// <summary>
        /// Compute the longest string that is a substring of two strings. Not to be
        /// confused with longest common subsequence.
        /// <see cref="http://en.wikipedia.org/wiki/Longest_common_substring">Longest Common Substring</see>
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <returns>The longest common substring of the two input strings.</returns>
        public static string LongestCommonSubstring(string source, string target)
        {
            if (string.IsNullOrEmpty(source) ||
                string.IsNullOrEmpty(target))
            {
                return string.Empty;
            }

            var m = source.Length;
            var n = target.Length;

            var lcs = new int[m, n];

            var x = 0;
            var k = 0;

            var b = new StringBuilder();

            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    if (source[i] != target[j])
                    {
                        lcs[i, j] = 0;
                    }
                    else
                    {
                        lcs[i, j] = 1 + (i == 0 || j == 0
                            ? 0
                            : lcs[i - 1, j - 1]);

                        if (lcs[i, j] <= x)
                        {
                            continue;
                        }

                        x = lcs[i, j];

                        var p = i - lcs[i, j] + 1;

                        if (k == p)
                        {
                            b.Append(source[i]);
                        }
                        else
                        {
                            k = p;

                            b.Clear();
                            b.Append(source.Substring(k, i + 1 - k));
                        }
                    }
                }
            }

            return b.ToString();
        }
        #endregion

        #endregion

        #region private methods
        /// <summary>
        /// Backtrack on two strings to fill up the given LCS length table.
        /// </summary>
        /// <param name="source">source string.</param>
        /// <param name="target">target string.</param>
        /// <param name="lcs">LCS length table.</param>
        /// <param name="i">Start row.</param>
        /// <param name="j">Start column.</param>
        /// <returns></returns>
        private static string Backtrack(string source, string target, int[,] lcs, int i, int j)
        {
            while (true)
            {
                if (i == 0 || j == 0)
                {
                    return string.Empty;
                }

                if (source[i - 1].Equals(target[j - 1]))
                {
                    return Backtrack(source, target, lcs, i - 1, j - 1) + source[i - 1];
                }

                if (lcs[i, j - 1] > lcs[i - 1, j])
                {
                    j = j - 1;
                }
                else
                {
                    i = i - 1;
                }
            }
        }

        /// <summary>
        /// Helper for Ratcliff-Obershelp Similarity.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="target">Target string.</param>
        /// <returns>A list of all common substrings (from longest to shortest).</returns>
        private static IEnumerable<string> AllCommonSubstrings(string source, string target)
        {
            var r = new List<string>();
            var s = LongestCommonSubstring(source, target);

            if (string.IsNullOrEmpty(s))
            {
                return r;
            }

            r.Add(s);
            r.AddRange(AllCommonSubstrings(source.Replace(s, string.Empty), target.Replace(s, string.Empty)));

            return r;
        }
        #endregion
    }
}
