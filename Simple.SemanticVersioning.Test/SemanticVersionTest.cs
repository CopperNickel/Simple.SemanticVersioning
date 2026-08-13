namespace Simple.SemanticVersioning.Test;

public sealed class SemanticVersionTest {

  [Theory]
  [InlineData(1, 2, 3, 4)]
  [InlineData(1, 2, 5, 0)]
  [InlineData(17, 1, 0, 0)]
  [InlineData(18, 0, 0, 0)]
  [InlineData(0, 0, 0, 0)]
  [InlineData(0, 5, 0, 0)]
  [InlineData(0, 0, 6, 0)]
  [InlineData(0, 0, 0, 9)]
  public void Create_Created(long major, long minor, long patch, long revision) {
    // Arrange and Act
    var version = new SemanticVersion(major, minor, patch, revision);

    // Assert
    Assert.Equal(major, version.Major);
    Assert.Equal(minor, version.Minor);
    Assert.Equal(patch, version.Patch);
    Assert.Equal(revision, version.Revision);
  }
}
