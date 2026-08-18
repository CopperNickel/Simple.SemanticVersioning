namespace Simple.SemanticVersioning.Test;

public sealed class SemanticVersioningComparerTest {
    [Fact]
    public void Compare_VersionOnlyComparer_StandardLogic()
    {
        // Arrange
        var version = SemanticVersion.Parse("1.23.36.3663.63-a+w");

        // Act and assert
        Assert.Equal(0, SemanticVersioningComparer.VersionOnly.Compare(version, version));

        Assert.Equal(-1, SemanticVersioningComparer.VersionOnly.Compare(null, version));

        Assert.Equal(+1, SemanticVersioningComparer.VersionOnly.Compare(version, null));

        Assert.True(SemanticVersioningComparer.VersionOnly.Equals(version, version));
        Assert.False(SemanticVersioningComparer.VersionOnly.Equals(version, null));
        Assert.False(SemanticVersioningComparer.VersionOnly.Equals(null, version));

        Assert.Equal(0, SemanticVersioningComparer.VersionOnly.GetHashCode(null));
    }

    [Fact]
    public void Compare_VersionAndRankComparer_StandardLogic()
    {
        // Arrange
        var version = SemanticVersion.Parse("1.23.36.3663.63-a+w");

        // Act and assert
        Assert.Equal(0, SemanticVersioningComparer.VersionAndRank.Compare(version, version));

        Assert.Equal(-1, SemanticVersioningComparer.VersionAndRank.Compare(null, version));

        Assert.Equal(+1, SemanticVersioningComparer.VersionAndRank.Compare(version, null));

        Assert.True(SemanticVersioningComparer.VersionAndRank.Equals(version, version));
        Assert.False(SemanticVersioningComparer.VersionAndRank.Equals(version, null));
        Assert.False(SemanticVersioningComparer.VersionAndRank.Equals(null, version));

        Assert.Equal(0, SemanticVersioningComparer.VersionAndRank.GetHashCode(null));
    }

    [Fact]
    public void Compare_VersionSemanticComparer_StandardLogic()
    {
        // Arrange
        var version = SemanticVersion.Parse("1.23.36.3663.63-a+w");

        // Act and assert
        Assert.Equal(0, SemanticVersioningComparer.VersionSemantic.Compare(version, version));

        Assert.Equal(-1, SemanticVersioningComparer.VersionSemantic.Compare(null, version));

        Assert.Equal(+1, SemanticVersioningComparer.VersionSemantic.Compare(version, null));

        Assert.True(SemanticVersioningComparer.VersionSemantic.Equals(version, version));
        Assert.False(SemanticVersioningComparer.VersionSemantic.Equals(version, null));
        Assert.False(SemanticVersioningComparer.VersionSemantic.Equals(null, version));

        Assert.Equal(0, SemanticVersioningComparer.VersionSemantic.GetHashCode(null));
    }

    [Fact]
    public void Compare_VersionAllComparer_StandardLogic()
    {
        // Arrange
        var version = SemanticVersion.Parse("1.23.36.3663.63-a+w");

        // Act and assert
        Assert.Equal(0, SemanticVersioningComparer.VersionAll.Compare(version, version));

        Assert.Equal(-1, SemanticVersioningComparer.VersionAll.Compare(null, version));

        Assert.Equal(+1, SemanticVersioningComparer.VersionAll.Compare(version, null));

        Assert.True(SemanticVersioningComparer.VersionAll.Equals(version, version));
        Assert.False(SemanticVersioningComparer.VersionAll.Equals(version, null));
        Assert.False(SemanticVersioningComparer.VersionAll.Equals(null, version));

        Assert.Equal(0, SemanticVersioningComparer.VersionAll.GetHashCode(null));
    }

    [Fact]
    public void Compare_VersionTextComparer_StandardLogic()
    {
        // Arrange
        var version = SemanticVersion.Parse("1.23.36.3663.63-a+w");

        // Act and assert
        Assert.Equal(0, SemanticVersioningComparer.VersionText.Compare(version, version));

        Assert.Equal(-1, SemanticVersioningComparer.VersionText.Compare(null, version));

        Assert.Equal(+1, SemanticVersioningComparer.VersionText.Compare(version, null));

        Assert.True(SemanticVersioningComparer.VersionText.Equals(version, version));
        Assert.False(SemanticVersioningComparer.VersionText.Equals(version, null));
        Assert.False(SemanticVersioningComparer.VersionText.Equals(null, version));

        Assert.Equal(0, SemanticVersioningComparer.VersionText.GetHashCode(null));
    }

    [Theory]
    [MemberData(nameof(VersionOnlyData))]
    public void Compare_VersionOnlyComparer_Compared((string left, string right, int expected) item)
    {
        // Arrange
        var versionLeft = SemanticVersion.Parse(item.left);
        var versionRight = SemanticVersion.Parse(item.right);
        var expected = item.expected;

        // Act
        var actual = SemanticVersioningComparer.VersionOnly.Compare(versionLeft, versionRight);
        var reversed = SemanticVersioningComparer.VersionOnly.Compare(versionRight, versionLeft);
        var equals = SemanticVersioningComparer.VersionOnly.Equals(versionLeft, versionRight);
        var leftHash = SemanticVersioningComparer.VersionOnly.GetHashCode(versionLeft);
        var rightHash = SemanticVersioningComparer.VersionOnly.GetHashCode(versionRight);

        // Assert
        Assert.Equal(Math.Sign(expected), Math.Sign(actual));
        Assert.Equal(Math.Sign(expected), -Math.Sign(reversed));
        Assert.Equal(actual == 0, equals);
        Assert.Equal(actual == 0, leftHash == rightHash);
    }

    [Theory]
    [MemberData(nameof(VersionAndRankData))]
    public void Compare_VersionAndRankComparer_Compared((string left, string right, int expected) item)
    {
        // Arrange
        var versionLeft = SemanticVersion.Parse(item.left);
        var versionRight = SemanticVersion.Parse(item.right);
        var expected = item.expected;

        // Act
        var actual = SemanticVersioningComparer.VersionAndRank.Compare(versionLeft, versionRight);
        var reversed = SemanticVersioningComparer.VersionAndRank.Compare(versionRight, versionLeft);
        var equals = SemanticVersioningComparer.VersionAndRank.Equals(versionLeft, versionRight);
        var leftHash = SemanticVersioningComparer.VersionAndRank.GetHashCode(versionLeft);
        var rightHash = SemanticVersioningComparer.VersionAndRank.GetHashCode(versionRight);

        // Assert
        Assert.Equal(Math.Sign(expected), Math.Sign(actual));
        Assert.Equal(Math.Sign(expected), -Math.Sign(reversed));
        Assert.Equal(actual == 0, equals);
        Assert.Equal(actual == 0, leftHash == rightHash);
    }

    [Theory]
    [MemberData(nameof(VersionSemanticData))]
    public void Compare_VersionSemanticComparer_Compared((string left, string right, int expected) item)
    {
        // Arrange
        var versionLeft = SemanticVersion.Parse(item.left);
        var versionRight = SemanticVersion.Parse(item.right);
        var expected = item.expected;

        // Act
        var actual = SemanticVersioningComparer.VersionSemantic.Compare(versionLeft, versionRight);
        var reversed = SemanticVersioningComparer.VersionSemantic.Compare(versionRight, versionLeft);
        var equals = SemanticVersioningComparer.VersionSemantic.Equals(versionLeft, versionRight);
        var leftHash = SemanticVersioningComparer.VersionSemantic.GetHashCode(versionLeft);
        var rightHash = SemanticVersioningComparer.VersionSemantic.GetHashCode(versionRight);

        // Assert
        Assert.Equal(Math.Sign(expected), Math.Sign(actual));
        Assert.Equal(Math.Sign(expected), -Math.Sign(reversed));
        Assert.Equal(actual == 0, equals);
        Assert.Equal(actual == 0, leftHash == rightHash);
    }

    [Theory]
    [MemberData(nameof(VersionAllData))]
    public void Compare_VersionAllComparer_Compared((string left, string right, int expected) item)
    {
        // Arrange
        var versionLeft = SemanticVersion.Parse(item.left);
        var versionRight = SemanticVersion.Parse(item.right);
        var expected = item.expected;

        // Act
        var actual = SemanticVersioningComparer.VersionAll.Compare(versionLeft, versionRight);
        var reversed = SemanticVersioningComparer.VersionAll.Compare(versionRight, versionLeft);
        var comparable = versionLeft.CompareTo(versionRight);
        var equals = SemanticVersioningComparer.VersionAll.Equals(versionLeft, versionRight);
        var leftHash = SemanticVersioningComparer.VersionAll.GetHashCode(versionLeft);
        var rightHash = SemanticVersioningComparer.VersionAll.GetHashCode(versionRight);

        // Assert
        Assert.Equal(Math.Sign(expected), Math.Sign(actual));
        Assert.Equal(Math.Sign(expected), -Math.Sign(reversed));
        Assert.Equal(expected, comparable);

        Assert.Equal(actual == 0, equals);
        Assert.Equal(actual == 0, leftHash == rightHash);

        Assert.Equal(actual < 0, versionLeft < versionRight);
        Assert.Equal(actual <= 0, versionLeft <= versionRight);
        Assert.Equal(actual == 0, versionLeft == versionRight);
        Assert.Equal(actual != 0, versionLeft != versionRight);
        Assert.Equal(actual >= 0, versionLeft >= versionRight);
        Assert.Equal(actual > 0, versionLeft > versionRight);
    }

    [Theory]
    [MemberData(nameof(VersionTextData))]
    public void Compare_VersionTextComparer_Compared((string left, string right, int expected) item)
    {
        // Arrange
        var versionLeft = SemanticVersion.Parse(item.left);
        var versionRight = SemanticVersion.Parse(item.right);
        var expected = item.expected;

        // Act
        var actual = SemanticVersioningComparer.VersionText.Compare(versionLeft, versionRight);
        var reversed = SemanticVersioningComparer.VersionText.Compare(versionRight, versionLeft);
        var equals = SemanticVersioningComparer.VersionText.Equals(versionLeft, versionRight);
        var leftHash = SemanticVersioningComparer.VersionText.GetHashCode(versionLeft);
        var rightHash = SemanticVersioningComparer.VersionText.GetHashCode(versionRight);

        // Assert
        Assert.Equal(Math.Sign(expected), Math.Sign(actual));
        Assert.Equal(Math.Sign(expected), -Math.Sign(reversed));

        Assert.Equal(actual == 0, equals);
        Assert.Equal(actual == 0, leftHash == rightHash);
    }

    public static TheoryData<(string left, string right, int expected)> VersionOnlyData()
    {
        return [
            ("1.5.9", "1.5.9", 0),
            ("2.0.0", "1.99.0", 1),
            ("1.0.0", "1.1.0", -1),
            ("1.0.0", "1.0.2", -1),
            ("1.8.7-a", "1.8.7", 0),
            ("1.5.6+a", "1.5.6", 0),
            ("1.5.6-zeta5", "1.5.6-zeta10", 0),
            ("1.5.6-beta+a", "1.5.6", 0),
            ("1.5.6-alpha", "1.5.6-beta", 0),
            ("1.5.6-m5", "1.5.6-m10", 0),
            ("1.5.6-m+a5", "1.5.6-m+a10", 0),
            ("1.5.6+a5", "1.5.6+a10", 0),
            ("1.5.6-a5", "1.5.6-A5", 0),
            ("1.5.6+a5", "1.5.6+A5", 0),
            ("1.5.6-b7+a5", "1.5.6-b7+A5", 0),
        ];
    }

    public static TheoryData<(string left, string right, int expected)> VersionAndRankData()
    {
        return [
            ("1.5.9", "1.5.9", 0),
            ("2.0.0", "1.99.0", 1),
            ("1.0.0", "1.1.0", -1),
            ("1.0.0", "1.0.2", -1),
            ("1.8.7-a", "1.8.7", -1),
            ("1.5.6+a", "1.5.6", 0),
            ("1.5.6-zeta5", "1.5.6-zeta10", 0),
            ("1.5.6-beta+a", "1.5.6", -1),
            ("1.5.6-alpha", "1.5.6-beta", -1),
            ("1.5.6-m5", "1.5.6-m10", 0),
            ("1.5.6-m+a5", "1.5.6-m+a10", 0),
            ("1.5.6+a5", "1.5.6+a10", 0),
            ("1.5.6-a5", "1.5.6-A5", 0),
            ("1.5.6+a5", "1.5.6+A5", 0),
            ("1.5.6-b7+a5", "1.5.6-b7+A5", 0),
        ];
    }

    public static TheoryData<(string left, string right, int expected)> VersionSemanticData()
    {
        return [
            ("1.5.9", "1.5.9", 0),
            ("2.0.0", "1.99.0", 1),
            ("1.0.0", "1.1.0", -1),
            ("1.0.0", "1.0.2", -1),
            ("1.8.7-a", "1.8.7", -1),
            ("1.5.6+a", "1.5.6", 0),
            ("1.5.6-zeta5", "1.5.6-zeta10", 0),
            ("1.5.6-beta+a", "1.5.6", -1),
            ("1.5.6-alpha", "1.5.6-beta", 0),
            ("1.5.6-m5", "1.5.6-m10", 0),
            ("1.5.6-m+a5", "1.5.6-m+a10", 0),
            ("1.5.6+a5", "1.5.6+a10", 0),
            ("1.5.6-a5", "1.5.6-A5", 0),
            ("1.5.6+a5", "1.5.6+A5", 0),
            ("1.5.6-b7+a5", "1.5.6-b7+A5", 0),
        ];
    }

    public static TheoryData<(string left, string right, int expected)> VersionAllData()
    {
        return [
            ("1.5.9", "1.5.9", 0),
            ("1.0.0", "1.1.0", -1),
            ("2.0.0", "1.99.0", 1),
            ("1.0.0", "1.0.2", -1),
            ("1.8.7-a", "1.8.7", -1),
            ("1.5.6+a", "1.5.6", 1),
            ("1.5.6-beta+a", "1.5.6", -1),
            ("1.5.6-zeta5", "1.5.6-zeta10", -1),
            ("1.5.6-alpha", "1.5.6-beta", -1),
            ("1.5.6-alpha+z", "1.5.6-beta+y", -1),
            ("1.5.6-m5", "1.5.6-m10", -1),
            ("1.5.6-m+a5", "1.5.6-m+a10", -1),
            ("1.5.6+a5", "1.5.6+a10", -1),
            ("1.5.6-a5", "1.5.6-A5", -1),
            ("1.5.6+a5", "1.5.6+A5", -1),
            ("1.5.6-b7+a5", "1.5.6-b7+A5", -1),
        ];
    }

    public static TheoryData<(string left, string right, int expected)> VersionTextData()
    {
        return [
            ("1.5.9", "1.5.9", 0),
            ("2.0.0", "1.99.0", 1),
            ("1.0.0", "1.1.0", -1),
            ("1.0.0", "1.0.2", -1),
            ("1.8.7-a", "1.8.7", +1),
            ("1.5.6+a", "1.5.6", +1),
            ("1.5.6-beta+a", "1.5.6", +1),
            ("1.5.6-zeta5", "1.5.6-zeta10", -1),
            ("1.5.6-alpha", "1.5.6-beta", -1),
            ("1.5.6-alpha+z", "1.5.6-beta+y", -1),
            ("1.5.6-m5", "1.5.6-m10", -1),
            ("1.5.6-m+a5", "1.5.6-m+a10", -1),
            ("1.5.6+a5", "1.5.6+a10", -1),
            ("1.5.6-a5", "1.5.6-A5", -1),
            ("1.5.6+a5", "1.5.6+A5", -1),
            ("1.5.6-b7+a5", "1.5.6-b7+A5", -1),
        ];
    }
}
