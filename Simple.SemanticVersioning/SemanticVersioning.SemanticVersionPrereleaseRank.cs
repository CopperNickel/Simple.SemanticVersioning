namespace Simple.SemanticVersioning;

/// <summary>
/// Pre-release rank
/// </summary>
public sealed class SemanticVersionPrereleaseRank {
  #region Fields and Properties

  /// <summary>
  /// Release
  /// </summary>
  public static SemanticVersionPrereleaseRank Release { get; } = new("", "Release", 0);

  /// <summary>
  /// Unknown
  /// </summary>
  public static SemanticVersionPrereleaseRank Unknown { get; } = new("", "Unknown", 0);

  // Collection of all known suffixes
  private static readonly List<SemanticVersionPrereleaseRank> Suffixes = [
      new("", "Release", 0),
        new("", "Unknown", -6),

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

    suffix = suffix.Trim().TrimStart('-').TrimStart();

    return Suffixes
               .FirstOrDefault(item => IsMatch(item.Prefix, suffix))
           ?? Unknown;

    static bool IsMatch(string prefix, string text) {
      if (prefix.Equals(text, StringComparison.OrdinalIgnoreCase))
        return true;

      if (!text.StartsWith(prefix))
        return false;

      return !char.IsLetter(text[prefix.Length]);
    }
  }

  /// <summary>
  /// To String
  /// </summary>
  /// <returns>String representation</returns>
  public override string ToString() => Description;

  #endregion Public Methods
}


