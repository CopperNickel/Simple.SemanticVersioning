using System.Text.Encodings.Web;
using System.Text.Json;

namespace Simple.SemanticVersioning.Test;

public sealed class SemanticVersionJsonConverterTest {
  private static readonly JsonSerializerOptions Options = new() {
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
  };

  [Theory]
  [MemberData(nameof(ValidVersions))]
  public void Serialize_Serialized(SemanticVersion version) {
    // Act
    var json = JsonSerializer.Serialize(version, Options);

    // Assert
    var expectedJson = "\"" + version.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

    var actual = JsonSerializer.Deserialize<SemanticVersion>(json);

    Assert.Equal(version, actual);
    Assert.Equal(json, expectedJson);
  }

  [Fact]
  public void Serialize_Null_Serialized() {
    // Act
    var json = JsonSerializer.Serialize((SemanticVersion)null!);

    // Assert
    Assert.Equal("null", json);

    var actual = JsonSerializer.Deserialize<SemanticVersion>(json);

    Assert.Null(actual);
  }

  [Fact]
  public void Serialize_InvalidVersion_ThrowsJsonException() {
    // Arrange
    var invalidVersion = "\"invalid\"";

    // Act & Assert
    var error = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<SemanticVersion>(invalidVersion, Options));

    Assert.Contains("semantic", error.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  public void Serialize_AsDictionary_Serialized() {
    // Arrange
    var dict = new Dictionary<SemanticVersion, int> {
      { new SemanticVersion(1, 2, 3), 42 }
    };

    // Act
    var json = JsonSerializer.Serialize(dict, Options);

    // Assert
    var actual = JsonSerializer.Deserialize<Dictionary<SemanticVersion, int>>(json, Options);

    Assert.Equal(dict, actual);
  }

  [Fact]
  public void Deserialize_InvalidType_ThrowsJsonException() {
    // Arrange
    var json = "[1, 2, 3]";

    // Act & Assert
    var error = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<SemanticVersion>(json, Options));

    Assert.Contains(nameof(SemanticVersion), error.Message);
  }

  public static TheoryData<SemanticVersion> ValidVersions() {
    return
    [
        new SemanticVersion([1, 2, 3, 4, 5], "", "Meta-3+1"),
            new SemanticVersion([1, 2, 3, 4, 5], "RC-1", ""),
            new SemanticVersion([1, 2, 3, 4, 5], "RC-1", "Meta-3+1"),
            new SemanticVersion(1, 2, 3, 4),
            new SemanticVersion(1, 2, 3),
            new SemanticVersion(1, 2),
            new SemanticVersion(1),
            new SemanticVersion(0),
            new SemanticVersion([1, 2], "RC-X", "Meta-3+1"),
            new SemanticVersion([1], "RC-X", "Meta-3+1"),
            new SemanticVersion([1, 0, 5], "RC-X", "Meta-3+1"),
        ];
  }
}

