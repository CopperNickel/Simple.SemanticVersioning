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

  [Theory]
  [MemberData(nameof(ValidVersions))]
  public void Create_Versions_Created(int[] items)
  {
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
  [InlineData("0.0", false)]
  [InlineData("0.9", false)]
  [InlineData("0.9+Release", false)]
  [InlineData("0.9-Release", false)]

  [InlineData("1.0-alpha", false)]
  [InlineData("1.9-beta+Release", false)]
  [InlineData("1.9+Release", true)]
  [InlineData("1.9-Release", true)]
  [InlineData("1.0-Unknown", false)]
  public void IsRelease_ReleaseWhenNotPrerelease(string version, bool isRelease)
  {
    // Arrange
    var ver = SemanticVersion.Parse(version);

    // Act
    var actual = ver.IsRelease;

    // Assert
    Assert.Equal(isRelease, actual);
  }

  public static TheoryData<int[]> ValidVersions()
  {
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
}
