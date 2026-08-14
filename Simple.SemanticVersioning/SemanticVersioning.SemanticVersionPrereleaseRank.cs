namespace Simple.SemanticVersioning;

/// <summary>
/// Pre-release rank
/// </summary>
public sealed class SemanticVersionPrereleaseRank
    : IEquatable<SemanticVersionPrereleaseRank>,
      IComparable<SemanticVersionPrereleaseRank> {
  #region Fields and Properties

  /// <summary>
  /// Release
  /// </summary>
  public static SemanticVersionPrereleaseRank Release { get; } = new("", "Release", 0);

  /// <summary>
  /// Unknown
  /// </summary>
  public static SemanticVersionPrereleaseRank Unknown { get; } = new("", "Unknown", 0);

  /// <summary>
  /// Collection of all known suffixes 
  /// </summary>
  public static IReadOnlyList<SemanticVersionPrereleaseRank> Suffixes { get; } = [
    new("dev", "Development build, very unstable", -5),
    new("a", "Alpha. Early, unstable, incomplete features", -4),
    new("alpha", "Alpha. Early, unstable, incomplete features", -4),

    new("b", "Beta. Feature-complete-ish, still testing", -3),
    new("beta", "Beta. Feature-complete-ish, still testing", -3),

    new("preview", "Preview", -2),

    new("pre", "Prerelease", -2),
    new("rc", "Release candidate", -1),

    new("final", "Final", 0),

    new("nightly", "Automated build from latest commit", -5),
    new("canary", "Bleeding-edge, auto-published, higher risk than nightly", -6),
    new("snapshot", "Automated build from latest commit", -5),
    new("next", "Upcoming tag", -1),
    new("exp", "Experimental", -2),

    new("m", "Milestone", -3),
    new("ea", "Early access", -1),
    new("hotfix", "Hot fix", -1),
    new("build", "Build as pre-release", -2),
    new("test", "Test", -6),
    new("qa", "Test QA", -6),
  ];

  /// <summary>
  /// Prefix
  /// </summary>
  public string Prefix { get; }

  /// <summary>
  /// Description
  /// </summary>
  public string Description { get; }

  /// <summary>
  /// Rank
  /// </summary>
  public int Rank { get; }

  /// <summary>
  /// Is Release
  /// </summary>
  public bool IsRelease => ReferenceEquals(this, Release);

  /// <summary>
  /// Is known
  /// </summary>
  public bool IsKnown => !ReferenceEquals(this, Unknown);

  #endregion Fields and Properties

  #region Create

  private SemanticVersionPrereleaseRank(string prefix, string description, int rank) {
    Prefix = prefix;
    Description = description;
    Rank = rank;
  }

  #endregion Create

  #region Public Methods

  /// <summary>
  /// Find rank by suffix
  /// </summary>
  /// <param name="suffix">Suffix to find</param>
  /// <returns>Rank found</returns>
  public static SemanticVersionPrereleaseRank Find(string? suffix) {
    if (string.IsNullOrWhiteSpace(suffix))
      return Release;

    suffix = string.Concat(suffix.Trim().Where(char.IsLetter));

    if (string.IsNullOrWhiteSpace(suffix))
      return Unknown;

    return Suffixes.FirstOrDefault(item => string.Equals(suffix, item.Prefix, StringComparison.OrdinalIgnoreCase)) ?? Unknown;
  }

  /// <summary>
  /// To String
  /// </summary>
  /// <returns>String representation</returns>
  public override string ToString() => Description;

  #endregion Public Methods

  #region IEquatable<SemanticVersionPrereleaseRank>

  /// <summary>
  /// Equals
  /// </summary>
  /// <param name="other">Rank to compare with</param>
  /// <returns>True if ranks are equal, false otherwise</returns>
  public bool Equals(SemanticVersionPrereleaseRank? other) {
    if (ReferenceEquals(this, other))
      return true;

    if (other is null)
      return false;

    return Rank == other.Rank && string.Equals(Description, other.Description, StringComparison.Ordinal);
  }

  /// <summary>
  /// Equals
  /// </summary>
  /// <param name="obj">Object to compare with</param>
  /// <returns>True if ranks are equal, false otherwise</returns>
  public override bool Equals(object? obj) => (obj is SemanticVersionPrereleaseRank other) && Equals(other);

  /// <summary>
  /// Hash code
  /// </summary>
  /// <returns>Hash code</returns>
  public override int GetHashCode() => HashCode.Combine(Rank, Description.GetHashCode(StringComparison.Ordinal));

  #endregion IEquatable<SemanticVersionPrereleaseRank>

  #region IComparable<SemanticVersionPrereleaseRank>

  /// <summary>
  /// Compare two rank instances
  /// </summary>
  /// <param name="left">Left instance</param>
  /// <param name="right">Right instance</param>
  /// <returns>-1 if left is less than right, 0 if left equals to right, +1 if left is more than right</returns>
  public static int Compare(SemanticVersionPrereleaseRank? left, SemanticVersionPrereleaseRank? right) {
    if (ReferenceEquals(left, right))
      return 0;
    if (left is null)
      return -1;
    if (right is null)
      return +1;

    var result = left.Rank.CompareTo(right.Rank);

    if (result != 0)
      return result;

    result = string.Compare(left.Description, right.Description, StringComparison.OrdinalIgnoreCase);

    return result == 0
        ? string.Compare(left.Description, right.Description, StringComparison.Ordinal)
        : result;
  }

  /// <summary>
  /// Compare two rank instances
  /// </summary>
  /// <param name="other">Other instance to compare with</param>
  /// <returns>-1 if this is less than other, 0 if this equals to other, +1 if this is more than other</returns>
  public int CompareTo(SemanticVersionPrereleaseRank? other) => Compare(this, other);

  #endregion IComparable<SemanticVersionPrereleaseRank>
}


