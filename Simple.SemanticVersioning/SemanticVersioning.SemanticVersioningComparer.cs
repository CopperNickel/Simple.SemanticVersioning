using System.Runtime.CompilerServices;

namespace Simple.SemanticVersioning;

/// <summary>
/// Standard comparers
/// </summary>
public static class SemanticVersioningComparer
{
    #region Public Classes

    /// <summary>
    /// Comparer of versions only
    /// </summary>
    public sealed class VersionOnlyComparer : IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
    {
        /// <summary>
        /// Compare tow version
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>Negative if x less than y, 0 if x equals y, positive if x greater than y</returns>
        public int Compare(SemanticVersion? x, SemanticVersion? y) => CompareVersionOnly(x, y);

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>True if x equals to y</returns>
        public bool Equals(SemanticVersion? x, SemanticVersion? y) => CompareVersionOnly(x, y) == 0;

        /// <summary>
        /// Returns hash code of x
        /// </summary>
        /// <param name="x">Value to return Hash code from</param>
        /// <returns>Hash Code</returns>
        public int GetHashCode(SemanticVersion? x)
        {
            if (x is null)
                return 0;

            return HashCode.Combine(x[0], x[1], x[2], x[3]);
        }
    }

    /// <summary>
    /// Comparer of versions and ranks
    /// </summary>
    public sealed class VersionAndRankComparer : IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
    {
        /// <summary>
        /// Compare tow version
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>Negative if x less than y, 0 if x equals y, positive if x greater than y</returns>
        public int Compare(SemanticVersion? x, SemanticVersion? y) => CompareVersionAndRankOnly(x, y);

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>True if x equals to y</returns>
        public bool Equals(SemanticVersion? x, SemanticVersion? y) => CompareVersionAndRankOnly(x, y) == 0;

        /// <summary>
        /// Returns hash code of x
        /// </summary>
        /// <param name="x">Value to return Hash code from</param>
        /// <returns>Hash Code</returns>
        public int GetHashCode(SemanticVersion? x)
        {
            if (x is null)
                return 0;

            return HashCode.Combine(x[0], x[1], x[2], x[3], x.Rank.Rank);
        }
    }

    /// <summary>
    /// Comparer of all versions parts
    /// </summary>
    public sealed class VersionAllComparer : IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
    {
        /// <summary>
        /// Compare tow version
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>Negative if x less than y, 0 if x equals y, positive if x greater than y</returns>
        public int Compare(SemanticVersion? x, SemanticVersion? y) => CompareVersionAll(x, y);

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>True if x equals to y</returns>
        public bool Equals(SemanticVersion? x, SemanticVersion? y) => CompareVersionAll(x, y) == 0;

        /// <summary>
        /// Returns hash code of x
        /// </summary>
        /// <param name="x">Value to return Hash code from</param>
        /// <returns>Hash Code</returns>
        public int GetHashCode(SemanticVersion? x)
        {
            if (x is null)
                return 0;

            return HashCode.Combine(x[0], x[1], x[2], x[3], x.Prerelease, x.Metadata);
        }
    }

    /// <summary>
    /// Comparer according to semantic versions rule
    /// </summary>
    public sealed class VersionSemanticComparer : IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
    {
        /// <summary>
        /// Compare tow version
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>Negative if x less than y, 0 if x equals y, positive if x greater than y</returns>
        public int Compare(SemanticVersion? x, SemanticVersion? y) => CompareVersionSemantic(x, y);

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Right</param>
        /// <returns>True if x equals to y</returns>
        public bool Equals(SemanticVersion? x, SemanticVersion? y) => CompareVersionSemantic(x, y) == 0;

        /// <summary>
        /// Returns hash code of x
        /// </summary>
        /// <param name="x">Value to return Hash code from</param>
        /// <returns>Hash Code</returns>
        public int GetHashCode(SemanticVersion? x)
        {
            if (x is null)
                return 0;

            return HashCode.Combine(x[0], x[1], x[2], x[3], string.IsNullOrEmpty(x.Prerelease));
        }
    }
    
    #endregion Public Classes

    #region Public

    /// <summary>
    /// Version Only comparer
    /// </summary>
    public static VersionOnlyComparer VersionOnly { get; } = new();

    /// <summary>
    /// Version and Rank comparer
    /// </summary>
    public static VersionAndRankComparer VersionAndRank { get; } = new();

    /// <summary>
    /// All versions attributes comparer
    /// </summary>
    public static VersionAllComparer VersionAll { get; } = new();

    /// <summary>
    /// Semantic rules comparer
    /// </summary>
    public static VersionSemanticComparer VersionSemantic { get; } = new();

    /// <summary>
    /// Compare Version Parts only
    /// </summary>
    /// <param name="left">Left version</param>
    /// <param name="right">Right version</param>
    /// <returns>Positive if left is less than right, 0 if they are equal, positive is left is greater than right</returns>
    public static int CompareVersionOnly(SemanticVersion? left, SemanticVersion? right)
    {
        if (ReferenceEquals(left, right))
            return 0;
        if (left is null)
            return -1;
        if (right is null)
            return +1;

        for (var i = 0; i < Math.Max(left.Parts.Count, right.Parts.Count); ++i)
        {
            var result = left[i].CompareTo(right[i]);

            if (result != 0)
                return result;
        }

        return 0;
    }

    /// <summary>
    /// Compare Version Parts only
    /// </summary>
    /// <param name="left">Left version</param>
    /// <param name="right">Right version</param>
    /// <returns>Positive if left is less than right, 0 if they are equal, positive is left is greater than right</returns>
    public static int CompareVersionAndRankOnly(SemanticVersion? left, SemanticVersion? right)
    {
        if (ReferenceEquals(left, right))
            return 0;
        if (left is null)
            return -1;
        if (right is null)
            return +1;

        var result = CompareVersionOnly(left, right);

        if (result != 0)
            return result;

        return left.Rank.Rank.CompareTo(right.Rank.Rank);
    }

    /// <summary>
    /// Compare Version Parts only
    /// </summary>
    /// <param name="left">Left version</param>
    /// <param name="right">Right version</param>
    /// <returns>Positive if left is less than right, 0 if they are equal, positive is left is greater than right</returns>
    public static int CompareVersionAll(SemanticVersion? left, SemanticVersion? right)
    {
        if (ReferenceEquals(left, right))
            return 0;
        if (left is null)
            return -1;
        if (right is null)
            return +1;

        var result = CompareVersionAndRankOnly(left, right);

        if (result != 0)
            return result;

        result = string.Compare(left.Prerelease, right.Prerelease, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        result = string.Compare(left.Prerelease, right.Prerelease, StringComparison.Ordinal);

        if (result != 0)
            return result;

        result = string.Compare(left.Metadata, right.Metadata, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        return string.Compare(left.Metadata, right.Metadata, StringComparison.Ordinal);
    }

    /// <summary>
    /// Compare according to standard semantic version rules
    /// </summary>
    /// <param name="left">Left version</param>
    /// <param name="right">Right version</param>
    /// <returns>Positive if left is less than right, 0 if they are equal, positive is left is greater than right</returns>
    public static int CompareVersionSemantic(SemanticVersion? left, SemanticVersion? right)
    {
        if (ReferenceEquals(left, right))
            return 0;
        if (left is null)
            return -1;
        if (right is null)
            return +1;

        var result = CompareVersionOnly(left, right);

        if (result != 0)
            return result;

        if (string.IsNullOrEmpty(left.Prerelease))
            return string.IsNullOrEmpty(right.Prerelease) ? 0 : 1;

        return string.IsNullOrEmpty(right.Prerelease) ? -1 : 0;
    }

    #endregion Public
}

