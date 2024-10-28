using System;

namespace Stellar.FuzzyString;

/// <summary>
/// Algorithms and options to use in the comparison.
/// </summary>
/// <remarks>
/// <see cref="http://en.wikipedia.org/wiki/Hamming_distance">Hamming Distance</see>
/// <see cref="http://en.wikipedia.org/wiki/Jaccard_index">Jaccard Distance</see>
/// <see cref="http://en.wikipedia.org/wiki/Jaro-Winkler_distance#Jaro-Winkler_Similarity">Jaro-Winkler Similarity</see>
/// <see cref="http://en.wikipedia.org/wiki/Levenshtein_distance">Levenshtein Distance</see>
/// <see cref="http://en.wikipedia.org/wiki/Sørensen-Dice_coefficient">Sorensen-Dice Coefficient</see>
/// <see cref="http://en.wikipedia.org/wiki/Longest_common_subsequence_problem">Longest Common Subsequence</see>
/// <see cref="http://en.wikipedia.org/wiki/Longest_common_substring">Longest Common Substring</see>
/// <see cref="http://en.wikipedia.org/wiki/Overlap_coefficient">Overlap Coefficient</see>
/// <see cref="http://www.morfoedro.it/doc.php?n=223&lang=en">Ratcliff-Obershelp Similarity</see>
/// <see cref="http://en.wikipedia.org/wiki/Tanimoto_coefficient#Tanimoto_similarity_and_distance">Tanimoto Coefficient</see>
/// </remarks>
[Flags]
public enum ComparisonOptions
{
    UseEditSimilarity = 1,
    UseHammingSimilarity = 2,
    UseJaccardIndex = 4,
    UseJaroWinklerSimilarity = 8,
    UseLevenshteinSimilarity = 16,
    UseLongestCommonSubsequence = 32,
    UseLongestCommonSubstring = 64,
    UseOverlapCoefficient = 128,
    UseRatcliffObershelpSimilarity = 256,
    UseSorensenDiceCoefficient = 512,

    UseDefault = UseJaroWinklerSimilarity | UseLevenshteinSimilarity,

    UseAll = UseEditSimilarity |
    UseHammingSimilarity |
    UseJaccardIndex | UseJaroWinklerSimilarity |
    UseLevenshteinSimilarity | UseLongestCommonSubsequence | UseLongestCommonSubstring |
    UseOverlapCoefficient |
    UseRatcliffObershelpSimilarity |
    UseSorensenDiceCoefficient,
    
    UseCaseInsensitive = 1024
}
