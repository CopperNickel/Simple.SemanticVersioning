using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Simple.SemanticVersioning;

/// <summary>
/// Semantic version see <see href="https://semver.org/">semver.org </see> for details
/// </summary>
[JsonConverter(typeof(SemanticVersionJsonConverter))]
public sealed class SemanticVersion :
    IFormattable,
    IEquatable<SemanticVersion>,
    IComparable<SemanticVersion>,
    ISpanParsable<SemanticVersion> {
  #region Private fields and properties

  private readonly List<BigInteger> m_Parts = [];

  #endregion Private fields and properties

  #region Public properties and methods

  /// <summary>
  /// Version parts (Major, Minor etc.), rightmost zeroes are trimmed
  /// </summary>
  public IReadOnlyList<BigInteger> Parts => m_Parts;

  /// <summary>
  /// Version part
  /// </summary>
  /// <param name="index">Index</param>
  /// <returns>Version part or 0</returns>
  public BigInteger this[int index] {
    get => index >= 0 && index < m_Parts.Count ? m_Parts[index] : 0;
    private set {
      ArgumentOutOfRangeException.ThrowIfNegative(index);
      ArgumentOutOfRangeException.ThrowIfNegative(value);

      if (index < m_Parts.Count) {
        m_Parts[index] = value;

        if (value == 0 && index == m_Parts.Count - 1) {
          for (var i = m_Parts.Count - 1; i >= 0 && m_Parts[i] == BigInteger.Zero; --i)
            m_Parts.RemoveAt(i);
        }
      }
      else if (value > 0) {
        m_Parts.AddRange(Enumerable.Repeat(BigInteger.Zero, index - m_Parts.Count));

        m_Parts.Add(value);
      }
    }
  }

  /// <summary>
  /// Major
  /// </summary>
  public BigInteger Major {
    get => this[0];
    init => this[0] = value;
  }

  /// <summary>
  /// Minor
  /// </summary>
  public BigInteger Minor {
    get => this[1];
    init => this[1] = value;
  }

  /// <summary>
  /// Patch
  /// </summary>
  public BigInteger Patch {
    get => this[2];
    init => this[2] = value;
  }

  /// <summary>
  /// Revision
  /// </summary>
  public BigInteger Revision {
    get => this[3];
    init => this[3] = value;
  }

  /// <summary>
  /// Prerelease
  /// </summary>
  public string Prerelease {
    get;
    init {
      field = value ?? "";

      Rank = SemanticVersionPrereleaseRank.Find(field);
    }
  }

  /// <summary>
  /// Metadata
  /// </summary>
  public string Metadata {
    get;
    init {
      field = value ?? "";
    }
  }

  /// <summary>
  /// Is version a release version
  /// </summary>
  public bool IsRelease => this[0] > 0 && Rank.IsRelease;

  /// <summary>
  /// Prerelease rank
  /// </summary>
  public SemanticVersionPrereleaseRank Rank { get; private set; } = SemanticVersionPrereleaseRank.Release;

  #endregion Public properties and methods

  #region Create

  /// <summary>
  /// Zero version
  /// </summary>
  public static SemanticVersion Zero { get; } = new(0, 0);

  /// <summary>
  /// Standard constructor
  /// </summary>
  /// <param name="parts"></param>
  /// <param name="prerelease">Prerelease, if any</param>
  /// <param name="metadata">Metadata, if any</param>
  public SemanticVersion(IEnumerable<BigInteger> parts, string? prerelease, string? metadata) {
    ArgumentNullException.ThrowIfNull(parts);

    m_Parts = [.. parts];

    if (m_Parts.Any(item => item < 0))
      throw new ArgumentException("Negative version parts are not allowed", nameof(parts));

    var removeFrom = 0;

    for (var i = m_Parts.Count - 1; i >= 0; --i)
      if (m_Parts[i] != 0) {
        removeFrom = i + 1;

        break;
      }

    m_Parts.RemoveRange(removeFrom, m_Parts.Count - removeFrom);

    Prerelease = prerelease?.Trim() ?? "";
    Metadata = metadata?.Trim() ?? "";
  }

  /// <summary>
  /// Standard constructor
  /// </summary>
  /// <param name="parts"></param>
  /// <param name="prerelease">Prerelease, if any</param>
  /// <param name="metadata">Metadata, if any</param>
  public SemanticVersion(IEnumerable<int> parts, string? prerelease, string? metadata)
    : this(parts.Select(item => (BigInteger)item), prerelease, metadata) { }

  /// <summary>
  /// Standard constructor
  /// </summary>
  /// <param name="parts"></param>
  public SemanticVersion(params IEnumerable<BigInteger> parts)
    : this(parts, null, null) { }

  /// <summary>
  /// Standard constructor
  /// </summary>
  /// <param name="parts"></param>
  public SemanticVersion(params IEnumerable<int> parts)
    : this(parts.Select(item => (BigInteger)item), null, null) { }

  /// <summary>
  /// Constructor from another semantic version (constructor copy)
  /// </summary>
  /// <param name="origin">Original semantic version</param>
  public SemanticVersion(SemanticVersion origin) {
    ArgumentNullException.ThrowIfNull(origin);

    m_Parts = [.. origin.m_Parts];
    Prerelease = origin.Prerelease;
    Metadata = origin.Metadata;
  }

  /// <summary>
  /// Standard constructor 
  /// </summary>
  /// <param name="version">Version</param>
  public SemanticVersion(Version version)
    : this(version.Major, version.Minor, version.Build, version.Revision) { }

  /// <summary>
  /// Create semantic version from parts
  /// </summary>
  /// <typeparam name="T">Arbitrary precision integer type</typeparam>
  /// <param name="source">Version Parts</param>
  /// <returns>Semantic version</returns>
  public static SemanticVersion Create<T>(params IEnumerable<T> source) where T : IBinaryInteger<T> {
    return Create(source, null, null);
  }

  /// <summary>
  /// Create semantic version from parts
  /// </summary>
  /// <typeparam name="T">Arbitrary precision integer type</typeparam>
  /// <param name="source">Version Parts</param>
  /// <param name="prerelease">Prerelease, if any</param>
  /// <param name="metadata">Metadata, if any</param>
  /// <returns>Semantic version</returns>
  public static SemanticVersion Create<T>(IEnumerable<T> source, string? prerelease = null, string? metadata = null) where T : IBinaryInteger<T> {
    ArgumentNullException.ThrowIfNull(source);

    return new SemanticVersion(source.Select(BigInteger.CreateSaturating), prerelease, metadata);
  }

  #endregion Create

  #region IFormattable

  /// <summary>
  /// To string
  /// </summary>
  /// <param name="format">Format, if any</param>
  /// <param name="formatProvider">Format provider, if any</param>
  /// <returns>Formatted version</returns>
  public string ToString(string? format, IFormatProvider? formatProvider) {
    formatProvider ??= CultureInfo.InvariantCulture;

    var result = string.Join(".", Enumerable.Range(0, Math.Max(m_Parts.Count, 2))
        .Select(i => this[i].ToString(format, formatProvider)));

    if (!string.IsNullOrWhiteSpace(Prerelease))
      result += "-" + Prerelease;

    if (!string.IsNullOrWhiteSpace(Metadata))
      result += "+" + Metadata;

    return result;
  }

  /// <summary>
  /// To string
  /// </summary>
  /// <param name="format">Format, if any</param>
  /// <returns>Formatted version</returns>
  public string ToString(string? format) => ToString(format, null);

  /// <summary>
  /// To string
  /// </summary>
  /// <returns>Formatted version</returns>
  public override string ToString() => ToString(null, null);

  #endregion IFormattable

  #region IEquatable<SemanticVersion>

  /// <summary>
  /// Equals
  /// </summary>
  /// <param name="other">Other version to compare with</param>
  /// <returns>True if equals to other, false otherwise</returns>
  public bool Equals(SemanticVersion? other) {
    if (other is null)
      return false;

    for (var i = 0; i < Math.Max(m_Parts.Count, other.m_Parts.Count); ++i) {
      var left = i < m_Parts.Count ? m_Parts[i] : BigInteger.Zero;
      var right = i < other.m_Parts.Count ? other.m_Parts[i] : BigInteger.Zero;

      if (left != right)
        return false;
    }

    return string.Equals(Prerelease, other.Prerelease, StringComparison.Ordinal) &&
           string.Equals(Metadata, other.Metadata, StringComparison.Ordinal);
  }

  /// <summary>
  /// Equals
  /// </summary>
  /// <param name="o">Other version to compare with</param>
  /// <returns>True if equals to other, false otherwise</returns>
  public override bool Equals(object? o) => o is SemanticVersion other && Equals(other);

  /// <summary>
  /// Hash code
  /// </summary>
  /// <returns>Hash code</returns>
  public override int GetHashCode() => HashCode.Combine(this[0], this[1], this[2], this[3]);

  #endregion IEquatable<SemanticVersion>

  #region IComparable<SemanticVersion>

  /// <summary>
  /// Compare 
  /// </summary>
  /// <param name="left">Left value to compare</param>
  /// <param name="right">Right value to compare</param>
  /// <returns>+1, if left than right, -1 if left less than right, 0 if left equals to right</returns>
  public static int Compare(SemanticVersion? left, SemanticVersion? right) {
    return SemanticVersioningComparer.VersionAll.Compare(left, right);
  }

  /// <summary>
  /// Compare To
  /// </summary>
  /// <param name="other">Other version to compare to</param>
  /// <returns>+1, if larger than other, -1 if less than other, 0 if is equal</returns>
  public int CompareTo(SemanticVersion? other) => Compare(this, other);

  #endregion IComparable<SemanticVersion>

  #region ISpanParsable<SemanticVersion>

  /// <summary>
  /// Parse span into semantic version
  /// </summary>
  /// <param name="s">Span to parse</param>
  /// <param name="provider">Provider to use</param>
  /// <returns>Parsed semantic version</returns>
  /// <exception cref="FormatException">If span is of invalid format</exception>
  public static SemanticVersion Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
    TryParse(s, provider, out var result)
      ? result
      : throw new FormatException("Span can't be parsed into semantic version");

  /// <summary>
  /// Parse span into semantic version
  /// </summary>
  /// <param name="s">Span to parse</param>
  /// <returns>Parsed semantic version</returns>
  /// <exception cref="FormatException">If span is of invalid format</exception>
  public static SemanticVersion Parse(ReadOnlySpan<char> s) => Parse(s, null);

  /// <summary>
  /// Try parse span into semantic version
  /// </summary>
  /// <param name="s">Span to parse</param>
  /// <param name="provider">Provider to use</param>
  /// <param name="result">Parsed semantic version, if span is of valid format, null otherwise</param>
  /// <returns>True, if span has been parsed, false otherwise</returns>
  public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out SemanticVersion result) {
    result = null;
    provider ??= CultureInfo.InvariantCulture;

    s = s.Trim();

    if (s.StartsWith("version", StringComparison.CurrentCultureIgnoreCase))
      s = s["version".Length..];
    else if (s.StartsWith("ver", StringComparison.CurrentCultureIgnoreCase))
      s = s["ver".Length..];
    else if (s.StartsWith("v", StringComparison.CurrentCultureIgnoreCase))
      s = s["v".Length..];

    s = s.Trim();

    var pPlus = s.IndexOf('+');
    var pMinus = s.IndexOf('-');
    var pMeta = pPlus;
    var pPrerelease = -1;

    if (pPlus < 0 || pPlus >= pMinus) {
      pPrerelease = pMinus;

      if (pPrerelease >= 0) {
        pMeta = s[pPrerelease..].IndexOf('+');

        if (pMeta >= 0)
          pMeta += pPrerelease;
      }
    }

    string? metadata = null;
    string? prerelease = null;

    if (pMeta >= 0)
      metadata = s[(pMeta + 1)..].ToString();

    if (pPrerelease >= 0)
      prerelease = pMeta >= 0
        ? s.Slice(pPrerelease + 1, pMeta - pPrerelease - 1).ToString()
        : s[(pPrerelease + 1)..].ToString();

    if (pPrerelease >= 0 || pMeta >= 0)
      s = s[..Math.Min(pPrerelease < 0 ? int.MaxValue : pPrerelease, pMeta < 0 ? int.MaxValue : pMeta)];

    using var en = s.Split('.');

    var parts = new List<BigInteger>(5);

    while (en.MoveNext()) {
      var span = s[en.Current.Start.Value..en.Current.End.Value];

      if (BigInteger.TryParse(span, NumberStyles.Any, provider, out var part))
        parts.Add(part);
      else
        return false;
    }

    if (parts.Count <= 0)
      return false;

    result = new SemanticVersion(parts, prerelease, metadata);

    return true;
  }

  /// <summary>
  /// Try parse span into semantic version
  /// </summary>
  /// <param name="s">Span to parse</param>
  /// <param name="result">Parsed semantic version, if span is of valid format, null otherwise</param>
  /// <returns>True, if span has been parsed, false otherwise</returns>
  public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out SemanticVersion result) => TryParse(s, null, out result);

  /// <summary>
  /// Parse into semantic version
  /// </summary>
  /// <param name="s">String to parse</param>
  /// <param name="provider">Provider to use (invariant culture by default)</param>
  /// <returns>Parsed semantic version</returns>
  /// <exception cref="FormatException">If span is of invalid format</exception>
  public static SemanticVersion Parse(string? s, IFormatProvider? provider) =>
    TryParse(s, provider, out var result)
      ? result
      : throw new FormatException("String can't be parsed into semantic version");

  /// <summary>
  /// Parse into semantic version
  /// </summary>
  /// <param name="s">String to parse</param>
  /// <returns>Parsed semantic version</returns>
  /// <exception cref="FormatException">If span is of invalid format</exception>
  public static SemanticVersion Parse(string? s) => Parse(s, null);

  /// <summary>
  /// Try parse string into semantic version
  /// </summary>
  /// <param name="s">String to parse</param>
  /// <param name="provider">Provider to use</param>
  /// <param name="result">Parsed semantic version, if string is of valid format, null otherwise</param>
  /// <returns>True, if string has been parsed, false otherwise</returns>
  public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SemanticVersion result) {
    if (s is null) {
      result = null;

      return false;
    }

    return TryParse(s.AsSpan(), provider, out result);
  }

  /// <summary>
  /// Try parse string into semantic version
  /// </summary>
  /// <param name="s">String to parse</param>
  /// <param name="result">Parsed semantic version, if string is of valid format, null otherwise</param>
  /// <returns>True, if string has been parsed, false otherwise</returns>
  public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out SemanticVersion result) =>
      TryParse(s, null, out result);

  #endregion ISpanParsable<SemanticVersion>

  #region Operators

  #region Comparison

  /// <summary>
  /// Equal operator
  /// </summary>
  /// <param name="left">Left operand</param>
  /// <param name="right">Right operand</param>
  /// <returns>True, if the operands are equal, false otherwise</returns>
  public static bool operator ==(SemanticVersion left, SemanticVersion right) => Compare(left, right) == 0;

  /// <summary>
  /// Not equal operator
  /// </summary>
  /// <param name="left">Left operand</param>
  /// <param name="right">Right operand</param>
  /// <returns>True, if the operands are not equal, false otherwise</returns>
  public static bool operator !=(SemanticVersion left, SemanticVersion right) => Compare(left, right) != 0;

  /// <summary>
  /// Less than operator
  /// </summary>
  /// <param name="left">Left operand</param>
  /// <param name="right">Right operand</param>
  /// <returns>True, if the left operand is less than the right operand, false otherwise</returns>
  public static bool operator <(SemanticVersion left, SemanticVersion right) => Compare(left, right) < 0;

  /// <summary>
  /// Greater than operator
  /// </summary>
  /// <param name="left">Left operand</param>
  /// <param name="right">Right operand</param>
  /// <returns>True, if the left operand is greater than the right operand, false otherwise</returns>
  public static bool operator >(SemanticVersion left, SemanticVersion right) => Compare(left, right) > 0;

  /// <summary>
  /// Less than or equal operator
  /// </summary>
  /// <param name="left">Left operand</param>
  /// <param name="right">Right operand</param>
  /// <returns>True, if the left operand is less than or equal to the right operand, false otherwise</returns>
  public static bool operator <=(SemanticVersion left, SemanticVersion right) => Compare(left, right) <= 0;

  /// <summary>
  /// Greater than or equal operator
  /// </summary>
  /// <param name="left">Left operand</param>
  /// <param name="right">Right operand</param>
  /// <returns>True, if the left operand is greater than or equal to the right operand, false otherwise</returns>
  public static bool operator >=(SemanticVersion left, SemanticVersion right) => Compare(left, right) >= 0;

  #endregion Comparison

  #endregion Operators
}

