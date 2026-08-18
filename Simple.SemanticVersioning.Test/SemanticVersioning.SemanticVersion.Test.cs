using System.Globalization;

namespace Simple.SemanticVersioning.Test;

public sealed class SemanticVersionTest {

  [Theory]
  [MemberData(nameof(ValidVersions))]
  public void Create_VersionParts_Created(int[] items) {
    // Arrange and Act
    var version = new SemanticVersion(items);

    // Assert
    Assert.Equal(items.ElementAtOrDefault(0), version.Major);
    Assert.Equal(items.ElementAtOrDefault(1), version.Minor);
    Assert.Equal(items.ElementAtOrDefault(2), version.Patch);
    Assert.Equal(items.ElementAtOrDefault(3), version.Revision);

    var expected = items
        .Reverse()
        .SkipWhile(item => item == 0)
        .Reverse()
        .Select(item => (long)item);

    Assert.True(version.Parts.SequenceEqual(expected));

    Assert.Equal("", version.Metadata);
    Assert.Equal("", version.Prerelease);
  }

  [Fact]
  public void CreateCopy_Created() {
    // Arrange
    var origin = new SemanticVersion([1, 2, 3, 4], "alpha", "test");

    // Act
    var version = new SemanticVersion(origin);

    // Assert
    Assert.Equal(origin, version);
  }

  [Fact]
  public void CreateCopy_Null_Exception() {
    // Arrange
    SemanticVersion origin = null!;

    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => new SemanticVersion(origin));
  }

  [Fact]
  public void Create_WithInit_Created() {
    // Arrange and Act
    var version = new SemanticVersion([1, 2, 3, 4]) {
      Prerelease = "alpha",
      Metadata = "test",
      Major = 9,
      Minor = 7,
      Patch = 5,
      Revision = 0
    };

    // Assert
    Assert.Equal("9.7.5-alpha+test", version.ToString());
  }

  [Fact]
  public void Create_WithInitAndNull_Created() {
    // Arrange and Act
    var version = new SemanticVersion([1, 2, 3, 4], "alpha", "test") {
      Prerelease = null!,
      Metadata = null!,
      Major = 9,
      Minor = 7,
      Patch = 0,
      Revision = 0
    };

    // Assert
    Assert.Equal("9.7", version.ToString());
  }

  [Fact]
  public void Create_WithInitAndShortVersion_Created() {
    // Arrange and Act
    var version = new SemanticVersion([1, 2], "alpha", "test") {
      Revision = 11
    };

    // Assert
    Assert.Equal("1.2.0.11-alpha+test", version.ToString());
  }

  [Fact]
  public void Create_VeryLargeVersionNumbers_Accepted() {
    // Act
    var version = new SemanticVersion([long.MaxValue, long.MaxValue, long.MaxValue]);

    // Assert
    Assert.True(version.Parts.All(item => item == long.MaxValue));
  }

  [Theory]
  [MemberData(nameof(ValidVersions))]
  public void Create_Versions_Created(int[] items) {
    // Arrange
    var origin = new Version(
      items.ElementAtOrDefault(0),
      items.ElementAtOrDefault(1),
      items.ElementAtOrDefault(2),
      items.ElementAtOrDefault(3));

    // Act
    var version = new SemanticVersion(origin);

    // Assert
    Assert.Equal(origin.Major, version.Major);
    Assert.Equal(origin.Minor, version.Minor);
    Assert.Equal(origin.Build, version.Patch);
    Assert.Equal(origin.Revision, version.Revision);
  }

  [Theory]
  [MemberData(nameof(ValidVersions))]
  public void Create_VersionsInLong_Created(int[] intItems) {
    // Arrange
    var items = intItems.Select(item => (long)item).ToArray();

    var origin = new Version(
      intItems.ElementAtOrDefault(0),
      intItems.ElementAtOrDefault(1),
      intItems.ElementAtOrDefault(2),
      intItems.ElementAtOrDefault(3));

    // Act
    var version = new SemanticVersion(items);

    // Assert
    Assert.Equal(origin.Major, version.Major);
    Assert.Equal(origin.Minor, version.Minor);
    Assert.Equal(origin.Build, version.Patch);
    Assert.Equal(origin.Revision, version.Revision);
  }

  [Theory]
  [InlineData("-1.0.0")]
  [InlineData("0.-1.0")]
  [InlineData("0.0.-1")]
  [InlineData("0.0.0.-1")]
  [InlineData("0.0.0.0.-1")]
  public void Create_negativeParts_Exception(string text) {
    // Arrange
    var parts = text
      .Split('.')
      .Select(item => int.Parse(item, NumberStyles.Any, CultureInfo.InvariantCulture))
      .ToList();

    // Act
    var error = Assert.Throws<ArgumentException>(() => new SemanticVersion(parts));

    // Assert
    Assert.Contains("Negative", error.Message);
  }

  [Theory]
  [InlineData("0.0", false)]
  [InlineData("0.9", false)]
  [InlineData("0.9+Release", false)]
  [InlineData("0.9-Release", false)]

  [InlineData("1.0-alpha", false)]
  [InlineData("1.9-beta+Release", false)]
  [InlineData("1.9+Release", true)]
  [InlineData("1.9-Release", true)]
  [InlineData("1.0-Unknown", false)]
  public void IsRelease_ReleaseWhenNotPrerelease(string version, bool isRelease) {
    // Arrange
    var ver = SemanticVersion.Parse(version);

    // Act
    var actual = ver.IsRelease;

    // Assert
    Assert.Equal(isRelease, actual);
  }

  [Theory]
  [InlineData("", "1.20.30.123456.9-beta+test")]
  [InlineData("x", "1.14.1e.1e240.9-beta+test")]
  [InlineData("d3", "001.020.030.123456.009-beta+test")]
  public void ToString_Represented(string format, string expected) {
    // Arrange
    var version = new SemanticVersion([1, 20, 30, 123456, 9], "beta", "test");

    // Act
    var result = version.ToString(format);

    // Assert
    Assert.Equal(expected, result);
  }

  [Theory]
  [InlineData("1", "1", true)]
  [InlineData("1.0", "1.0", true)]
  [InlineData("1.1.0.0", "1.1.0.1", false)]
  [InlineData("1.1.0.1", "1.1.0.0", false)]
  [InlineData("1.1", "1.1.0.0", true)]
  [InlineData("2.0", "1.0", false)]
  [InlineData("2.0+a", "2.0+b", false)]
  [InlineData("2.0-a", "2.0-b", false)]
  [InlineData("2.0-a+b", "2.0-b+a", false)]
  [InlineData("2.1", "1.2", false)]
  [InlineData("2.1.3.5.6", "2.1.3.5.7", false)]
  [InlineData("2.01.3.4.0.0-Alpha+Test", "2.1.03.4-Alpha+Test", true)]
  [InlineData("2.01.3.4.0.0-Beta", "2.1.03.4-BETA", false)]
  [InlineData("2.01.3.4.0.0+Test", "2.1.03.4+test", false)]
  [InlineData("2.01.3.4.0.0-BeTa+Test", "2.1.03.4-BETA+test", false)]
  public void Equals_Computed(string leftText, string rightText, bool expected) {
    // Arrange
    var left = SemanticVersion.Parse(leftText);

    var right = SemanticVersion.Parse(rightText);

    // Act
    var actual = Equals(left, right);

    // Assert
    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Equals_SpecialCases_Equals() {
    // Arrange
    var left = new SemanticVersion([1, 2, 5, 97, 3, 789], "beta", "test");

    var right = left;
    var obj = (object)right;
    var wrongType = (object)"WrongType";

    // Act and assert
    Assert.True(Equals(left, right));
    Assert.True(Equals(left, obj));
    Assert.False(Equals(null, right));

    Assert.False(Equals(left, null));

    Assert.False(left.Equals(null));

    Assert.False(Equals(left, wrongType));
  }

  [Fact]
  public void Zero_ZeroVersion() {
    // Act
    var zero = SemanticVersion.Zero;

    // Assert
    Assert.True(zero.Parts.All(item => item == 0));

    Assert.Empty(zero.Prerelease);

    Assert.Empty(zero.Metadata);

    Assert.False(zero.IsRelease);
  }

  [Theory]
  [MemberData(nameof(ComparisonData))]
  public void Compare_Compared((string? left, string? right, int compare) item) {
    // Arrange
    var left = item.left is null ? null : SemanticVersion.Parse(item.left);
    var right = item.right is null ? null : SemanticVersion.Parse(item.right);
    var expected = item.compare;

    // Act
    var result = Math.Sign(SemanticVersion.Compare(left, right));

    // Assert
    Assert.Equal(expected, result);
  }

  [Theory]
  [MemberData(nameof(ValidVersionsToParse))]
  public void TryParse_ValidText_Parsed(string text) {
    // Act
    var result1 = SemanticVersion.TryParse(text, out var version1);

    var result2 = SemanticVersion.TryParse(text.AsSpan(), out var version2);

    var version3 = SemanticVersion.Parse(text);

    var version4 = SemanticVersion.Parse(text.AsSpan());

    // Assert
    Assert.True(result1);
    Assert.True(result2);

    Assert.Equal(version1, version2);
    Assert.Equal(version2, version3);
    Assert.Equal(version3, version4);

    var expectedText = string.Concat(text.SkipWhile(c => c < '0' || c > '9')).Trim();

    if (!expectedText.Contains('.'))
      expectedText += ".0";

    Assert.Equal(expectedText, version1!.ToString());
  }

  [Theory]
  [MemberData(nameof(InvalidVersionsToParse))]
  public void TryParse_ValidText_FailedToParse(string? text) {
    // Act
    var result1 = SemanticVersion.TryParse(text, out var version1);

    var result2 = SemanticVersion.TryParse(text.AsSpan(), out var version2);

    var error1 = Assert.Throws<FormatException>(() => SemanticVersion.Parse(text));

    var error2 = Assert.Throws<FormatException>(() => SemanticVersion.Parse(text.AsSpan()));

    // Assert
    Assert.False(result1);
    Assert.False(result2);

    Assert.Null(version1);
    Assert.Null(version2);

    Assert.Contains("parsed", error1.Message);
    Assert.Contains("parsed", error2.Message);
  }

  public static TheoryData<int[]> ValidVersions() {
    return [
      new [] {0},
      new [] {1},
      new [] {1, 2},
      new [] {1, 2, 3},
      new [] {1, 2, 3, 4},
      new [] {1, 2, 3, 4, 5},
      new [] {1, 2, 3, 4, 5, 6},
      new [] {1, 2, 0, 0},
      new [] {1, 2, 0, 3},
      new [] {1, 2, 3, 4, 0},
      new [] {0, 0, 1, 2},
      new [] {0, 1, 2, 3},
      new [] {0, 0, 0, 4},
    ];
  }

  public static TheoryData<(string? left, string? right, int compare)> ComparisonData() {
    return [
      ("1.0", "2.0", -1),
      ("1.2", "2.1", -1),
      ("1.2", "1.2.1", -1),
      ("1.2", "1.2.1-rc", -1),
      ("1.2", "1.2.1+rc", -1),
    ];
  }

  public static TheoryData<string> ValidVersionsToParse() {
    return [
      "1",
      "1.2",
      "51.253.63.2",
      "51.253.63.2.6336.363",
      "51.253-zeta",
      "51.253-zeta+theta",
      "51.253+theta",
      "51.253.63.2.6336.363-alpha2-7+fita-23+789",
      "version 1.23.4566.36-a+b",
      "ver 1.23.4566.36-a+b",
      "v 1.23.4566.36-a+b",
      "1.2.3-alpha123beta456",
      "1.2.3-123",
      "1.2.3-Beta!@#$%Alpha",
      "1234567890123456.98745692369621.88825364785963.236695485669"
    ];
  }

  public static TheoryData<string?> InvalidVersionsToParse() {
    return [
      null!,
      "",
      "   ",
      ".",
      "..-a",
      "..-a+b",
      "-1.23.45",
      "a.b.c",
      "-a+b",
      "78.-123.65",
      "version -1.23.4566.36-a+b",
      "ver 1.-23.4566.36-a+b",
      "v 1.23.-4566.36-a+b",
    ];
  }
}
