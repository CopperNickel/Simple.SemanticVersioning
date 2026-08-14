namespace Simple.SemanticVersioning.Test;

public sealed class SemanticVersionPrereleaseRankTest {

  [Fact]
  public void Find_EmptyPrefix_FoundRelease() {
    // Act
    var rank = SemanticVersionPrereleaseRank.Find("");

    // Assert
    Assert.NotNull(rank);

    Assert.True(rank.IsRelease);
  }

  [Theory]
  [MemberData(nameof(KnownPrefixes))]
  public void Find_KnownPrefix_Found(string prefix) {
    // Act
    var rank = SemanticVersionPrereleaseRank.Find(prefix);

    // Assert
    Assert.NotNull(rank);

    Assert.True(rank.IsKnown);
  }

  [Theory]
  [MemberData(nameof(UnknownPrefixes))]
  public void Find_UnknownPrefix_FoundUnknown(string prefix) {
    // Act
    var rank = SemanticVersionPrereleaseRank.Find(prefix);

    // Assert
    Assert.NotNull(rank);

    Assert.False(rank.IsKnown);

    Assert.Equal(SemanticVersionPrereleaseRank.Unknown, rank);
  }

  public static TheoryData<string> KnownPrefixes => [
    "dev",
    "alpha",
    "beta",
    "rc-12",
    "m1",
    "M.2",
    "  M.2 ",
    "m+4",
    "rC178",
    "final"
  ];

  public static TheoryData<string> UnknownPrefixes => [
    "Ma",
    "x",
    " x",
    "finalize",
    "123"
  ];
}
