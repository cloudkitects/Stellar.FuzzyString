using System;
using System.Collections;
using System.IO;

using Newtonsoft.Json;
using NUnit.Framework;

namespace Cloudkitects.Stellar.FuzzyString.Tests
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

        [Test]
        public void Test1()
        {
            var headers = new[]
            {
                "XYZ Loan Number", "Seller Loan Number", "Borrower Number", "First Name", "Middle Name", "Last Name",
                "Suffix", "SSN", "Birth Date", "Marital Status", "US Citizen", "Citizenship Type",
                "Permanent Resident Alien", "Identification Issuer", "Identification Number", "Identification Type",
                "Military Status", "Honorable Discharge", "Intent to Occupy", "First Time Homebuyer", "Email ", "Phone",
                "Fax", "Alternate Name 1", "Alternate Name 2", "Alternate Name 3", "Alternate Name 4",
                "Alternate Name 5", "No of Dependents", "Dependents age 1", "Dependents age 2", "Dependents age 3",
                "Dependents age 4", "I do not wish to furnish this information", "Ethnicity", "Race 1 ", "Race 2 ",
                "Race 3", "Race 4", "Race 5", "Sex", "Present Address Rent Box Checked", "Present Address Street 1",
                "Present Address Street 2", "Present Address City", "Present Address State", "Present Address Zip",
                "Present Address Years in Residence", "Present Address Home Phone", "Present Rent",
                "Present First Mortgage (P&I)", "Present Hazard", "Present HOA Dues", "Present MI", "Present Other",
                "Present Other Financning (P&I)", "Present Real Estate Taxes", "Present Total",
                "Former Address Rent Box Checked", "Former Address Street 1", "Former Address Street 2",
                "Former Address City", "Former Address State", "Former Address Zip",
                "Former Address Years in Residence", "Mailing Address Street 1", "Mailing Address Street 2",
                "Mailing Address City", "Mailing Address State", "Mailing Address Zip", "Employer 1",
                "Employer 1 Address City", "Employer 1 Address State", "Employer 1 Address Street",
                "Employer 1 Address Street 2", "Employer 1 Address Zip", "Employer 1 Mthly Income", "Employer 1 Phone",
                "Employer 1 Position", "Employer 1 Self Employed", "Employer 1 Years in Line of Work",
                "Employer 1 Yrs on Job", "Employer 2", "Employer 2 Address City", "Employer 2 Address State",
                "Employer 2 Address Street", "Employer 2 Address Street 2", "Employer 2 Address Zip",
                "Employer 2 Begin Date", "Employer 2 End Date", "Employer 2 Mthly Income", "Employer 2 Phone",
                "Employer 2 Position", "Employer 2 Self Employed", "[Indicator if Retired?]", "Base Income",
                "Bonuses Income", "Commission Income", "Dividend/Interest Income", "Net Rental Income", "OT Income",
                "Other Income", "total_monthly_income", "borrower_monthly_wage_income", "Equifax Beacon Score",
                "Experian Score", "Trans Union - Empirica Score", "Borrower_Exclude"
            };

            const string column = "Indicator if Retired?";

            var bestMatch = Comparer.BestMatch(column, headers);

            Assert.That(bestMatch.Index == 94 &&
                        Math.Abs(bestMatch.Similarity - 0.94202898550724634) < Epsilon);
        }
    }
}
