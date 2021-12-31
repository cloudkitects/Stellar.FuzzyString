using System;
using System.Collections;
using System.IO;

using Newtonsoft.Json;
using NUnit.Framework;

namespace Stellar.FuzzyString.Tests
{
    [TestFixture]
    public class FuzzyStringTests
    {
        private const double Epsilon = 1E-15;

        /// <summary>
        /// A test case object.
        /// </summary>
        public class TestCase
        {
            public double Average { get; set; }
            public double EditSimilarity { get; set; }
            public double JaccardIndex { get; set; }
            public double JaroWinklerSimilarity { get; set; }
            public double LevenshteinSimilarity { get; set; }
            public double LongestCommonSubsequenceSimilarity { get; set; }
            public double LongestCommonSubstringSimilarity { get; set; }
            public double OverlapCoefficient { get; set; }
            public double RatcliffObershelpSimilarity { get; set; }
            public double SorensenDiceCoefficient { get; set; }
            public string Source { get; set; }
            public string Target { get; set; }
        }

        /// <summary>
        /// Backup field to cache file test cases.
        /// </summary>
        private static IEnumerable _testCases;

        /// <summary>
        /// Property to be used in test attributes.
        /// </summary>
        public static IEnumerable TestCases
        {
            get
            {
                if (_testCases != null)
                {
                    return _testCases;
                }

                var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "testCases.json");

                _testCases = JsonConvert.DeserializeObject<TestCase[]>(File.ReadAllText(file));

                return _testCases;
            }
        }

        /// <summary>
        /// Backup field to cache test headers.
        /// </summary>
        private static string[] _headers;

        /// <summary>
        /// Property to be used in test attributes.
        /// </summary>
        public static string[] Headers
        {
            get
            {
                if (_headers != null)
                {
                    return _headers;
                }

                var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "headers.json");

                _headers = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(file));

                return _headers;
            }
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _testCases = null;
        }

        /// <summary>
        /// Test all metrics against expected results.
        /// </summary>
        /// <param name="testCase">A test case with source and target strings,
        /// along with expected values for metrics.</param>
        /// <remarks>
        /// Some algorithms carry a truncation error that isn't easy to detect.
        /// For all our purposes, 1E-15 tolerance should do.
        /// <see cref="http://stackoverflow.com/questions/2411392"/>
        /// </remarks>
        [TestCaseSource(nameof(TestCases))]
        public void AllMetrics(TestCase testCase)
        {
            var editSimilarity = Comparer.EditSimilarity(testCase.Source, testCase.Target);
            var jaccardIndex = Comparer.JaccardIndex(testCase.Source, testCase.Target);
            var jaroWinklerSimilarity = Comparer.JaroWinklerSimilarity(testCase.Source, testCase.Target);
            var levenshteinSimilarity = Comparer.LevenshteinSimilarity(testCase.Source, testCase.Target);
            var longestCommonSubsequenceSimilarity = Comparer.LongestCommonSubsequenceSimilarity(testCase.Source, testCase.Target);
            var longestCommonSubstringSimilarity = Comparer.LongestCommonSubstringSimilarity(testCase.Source, testCase.Target);
            var overlapCoefficient = Comparer.OverlapCoefficient(testCase.Source, testCase.Target);
            var ratcliffObershelpSimilarity = Comparer.RatcliffObershelpSimilarity(testCase.Source, testCase.Target);
            var sorensenDiceCoefficient = Comparer.SorensenDiceCoefficient(testCase.Source, testCase.Target);
            var sorensenDiceJaccard = Math.Abs(sorensenDiceCoefficient - 2 * jaccardIndex / (jaccardIndex + 1));

            Assert.That(Math.Abs(editSimilarity - testCase.EditSimilarity) < Epsilon);
            Assert.That(Math.Abs(jaccardIndex - testCase.JaccardIndex) < Epsilon);
            Assert.That(Math.Abs(jaroWinklerSimilarity - testCase.JaroWinklerSimilarity) < Epsilon);
            Assert.That(Math.Abs(levenshteinSimilarity - testCase.LevenshteinSimilarity) < Epsilon);
            Assert.That(Math.Abs(longestCommonSubsequenceSimilarity - testCase.LongestCommonSubsequenceSimilarity) < Epsilon);
            Assert.That(Math.Abs(longestCommonSubstringSimilarity - testCase.LongestCommonSubstringSimilarity) < Epsilon);
            Assert.That(Math.Abs(overlapCoefficient - testCase.OverlapCoefficient) < Epsilon);
            Assert.That(Math.Abs(ratcliffObershelpSimilarity - testCase.RatcliffObershelpSimilarity) < Epsilon);
            Assert.That(Math.Abs(sorensenDiceCoefficient - testCase.SorensenDiceCoefficient) < Epsilon);
            Assert.That(sorensenDiceJaccard < Epsilon);
        }

        /// <summary>
        /// Test the average similarity score against the expected result.
        /// </summary>
        /// <param name="testCase">A test case with source and target strings,
        /// along with expected values for metrics.</param>
        /// <remarks>
        /// Some algorithms carry a truncation error that isn't easy to detect.
        /// For all our purposes, 1E-15 tolerance should do.
        /// <see cref="http://stackoverflow.com/questions/2411392"/>
        /// </remarks>
        [TestCaseSource(nameof(TestCases))]
        public void Average(TestCase testCase)
        {
            var average = Comparer.SimilarityScore(testCase.Source, testCase.Target);

            Assert.That(Math.Abs(average - testCase.Average) < Epsilon);
        }

        [TestCase("Indicator if Retired?", 94, 0.942028985507246)]
        [TestCase("Present Other Financing", 55, 0.855652173913043)]
        [TestCase("Present Address Street", 42, 0.95)]
        public void TestHeaders(string source, int index, double similarity)
        {
            var match = Comparer.BestMatch(source, Headers);

            Assert.That(match.Index == index &&
                        Math.Abs(match.Similarity - similarity) < Epsilon);
        }

        [TestCase("Present Address Street", "Present Address Street 1", 0.95)]
        public void TestMatch(string source, string target, double similarity)
        {
            var match = Comparer.BestMatch(source, new []{ target });

            Assert.That(Math.Abs(match.Similarity - similarity) < Epsilon);
        }
    }
}
