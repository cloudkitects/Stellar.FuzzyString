# Stellar.FuzzyString

A .net port of Kevin D. Jones' [fuzzystring](https://github.com/kdjones/fuzzystring), "Approximate String Comparison in C#" under the Eclipse Public License Version 1.0 ([EPL v1.0](http://www.eclipse.org/legal/epl-v10.html)) terms.

The `Comparer` class features a method for determining the approximate equality of two strings and many methods implementing various distance and similarity "scores".

A similarity score is a real-valued function that quantifies how close two objects are to each other. In that sense, all similarity measures represent the inverse of distance  (how far apart the two objects are from each other). This library is concerned with the similarity of string objects: two strings with a high similarity score are more similar than two strings with a lower similarity score.

There are many different similarity scores and distance algorithms, with different criteria about similarity. For example, an algorithm that does not take into account the order of characters when comparing two strings is particularly useful for finding anagrams, e.g. yielding a high similarity score between `"Tom Marvolo Riddle"` and `"I am Lord Voldemort"`.

Other algorithms take into account the number of "edits" (additions, replacements and deletions) needed to change one string into another. For example, The Levenshtein distance between `foo` and `bar` is the same as the Levenshtein distance between `beauties` and `beautiful` (`3` edits), despite our intuition that the latter are (at least phonetically) closer.

Some other algorithms give more weight to prefix similarity and length. In the previous example, the Levenshtein similarity score (the 1's complement of the Levenshtein distance to length ratio) between `beauties` and `beautiful` is `0.33`, but the Jaccard Index (the size of the intersection of the two strings divided by the size of their union) is `0.66`.

All similarity scores are normalized to the `[0,1]` range. The library allows averaging of multiple similarity scores in a single call via the options enumeration.

# Usage

Once the package is installed, invoke any `FuzzyString.Comparer` method with two strings as arguments:

```cs
using Cloudkitects.Stellar.FuzzyString;
...
var s = Comparer.RatcliffObershelpSimilarity("beauties", "beautiful"); // s = 0.705882352941177
```

The `ApproximatelyEquals` method takes two additional optional parameters:

```cs
public static bool ApproximatelyEquals(
    string source,
    string target,
    ComparisonOptions options = ComparisonOptions.UseAll,
    double threshold = 0.75)
{
    return SimilarityScore(source, target, options) >= Math.Max(0.01, Math.Min(threshold, 1));
}
```

Comparison options include case sensitivity (on by default) and different string similarity algorithms (all by default). The `SimilarityScore` method returns the mean value of all the algorithms specified in the comparison options.

**IMPORTANT**

There is no evidence to the average of two or more similarity algorithms being a good indicator of similarity, as the approaches differ. For example, the two strings below differ by one character. That yields high Edit similarity, but both the Jaccard index and the Sørensen-Dice coefficient are around the middle: 

```cs
var source = "BI105_PC_QUOTE_DTE_LST_ADD_OR_D";
var target = "BI105_PC_QUOTS_DTE_LST_ADD_OR_D";

var s = SimilarityScore(
    source,
    target,
    ComparisonOptions.UseEditSimilarity |
    ComparisonOptions.UseJaccardIndex |
    ComparisonOptions.UseSorensenDiceCoefficient);

// Edit Similarity = 0.967741935483871
// Jaccard Index = 0.409090909090909
// Sørensen-Dice Coefficient = 0.580645161290323
// s = 0.652492668621701
```
Averaging the three similarity scores yields a score below the default `0.75` similarity threshold&mdash;the `ApproximatelyEquals` method will return false despite the two strings high edit similarity. All in all, use caution and tune the combination of algorithms to best suit your needs&mdash;and refrain from blaming the OCR or fat fingers :)

The Edit distance is the Hamming distance (the number of substitutions to get from source to target when both have equal length) plus any additions. Empirically, this is more useful than the categorical "zero" Hamming distance if strings differ in length, but other than that there is no evidence of it being a better algorithm than the Hamming distance&mdash;it just fit my purposes of matching `ACCTPCTUNITS` to `[ACCTPCTUNITS?]`.
