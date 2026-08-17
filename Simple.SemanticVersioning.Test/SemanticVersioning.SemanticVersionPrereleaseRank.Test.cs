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

  [Fact]
  public void Compare_Compared() {
    // Arrange
    var ranks = SemanticVersionPrereleaseRank
      .Ranks
      .Append(SemanticVersionPrereleaseRank.Release)
      .Append(SemanticVersionPrereleaseRank.Unknown)
      .ToList();

    // Act
    var sorted = ranks
      .OrderBy(x => x)
      .ToList();

    // Assert
    var expected = ranks
      .OrderBy(x => x.Rank)
      .ThenBy(x => x.Prefix, StringComparer.OrdinalIgnoreCase)
      .ToList();

    Assert.Equal(expected, sorted);

    Assert.True(sorted.SequenceEqual(expected));
  }

  [Fact]
  public void ToString_ConvertedToString() {
    // Arrange
    var ranks = SemanticVersionPrereleaseRank
      .Ranks
      .Append(SemanticVersionPrereleaseRank.Release)
      .Append(SemanticVersionPrereleaseRank.Unknown)
      .ToList();

    // Act
    var result = ranks
      .All(item => string.Equals(item.ToString(), item.Prefix));

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void Equal_ValidImplementation()
  {
      // Arrange
      var data = SemanticVersionPrereleaseRank
          .Ranks
          .Append(SemanticVersionPrereleaseRank.Release)
          .Append(SemanticVersionPrereleaseRank.Unknown)
          .ToList();

      object otherObject = "Some Class";

      // Act and assert
      for (var i = 0; i < data.Count; ++i)
      {
          Assert.False(data[i].Equals(null));

          Assert.Equal(0, data[i].CompareTo(data[i]));

          Assert.Equal(-1, SemanticVersionPrereleaseRank.Compare(null, data[i]));
          Assert.Equal(+1, SemanticVersionPrereleaseRank.Compare(data[i], null));

          for (var j = i + 1; j < data.Count; ++j)
          {
              var other = (object)data[j];

              Assert.False(data[i].Equals(other));
              Assert.False(data[i].Equals(otherObject));

              Assert.NotEqual(data[i], data[j]);
              Assert.NotEqual(0, data[i].CompareTo(data[j]));
              Assert.NotEqual(data[i].GetHashCode(), data[j].GetHashCode());
          }
      }
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
