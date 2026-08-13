using System.Text.Json;
using System.Text.Json.Serialization;

namespace Simple.SemanticVersioning;

internal sealed class SemanticVersionJsonConverter : JsonConverter<SemanticVersion> {
  #region JsonConverter<SemanticVersion>

  public override SemanticVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    if (reader.TokenType == JsonTokenType.Null) {
      return null;
    }

    if (reader.TokenType != JsonTokenType.String) {
      throw new JsonException(
          $"Expected a JSON string for {nameof(SemanticVersion)}, got {reader.TokenType}.");
    }

    return SemanticVersion.TryParse(reader.GetString(), null, out var result)
        ? result
        : throw new JsonException("Invalid semantic version");
  }

  public override void Write(Utf8JsonWriter writer, SemanticVersion value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString());
  }

  public override SemanticVersion ReadAsPropertyName(
      ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    var s = reader.GetString();

    return SemanticVersion.Parse(s, provider: null);
  }

  public override void WriteAsPropertyName(
      Utf8JsonWriter writer, SemanticVersion value, JsonSerializerOptions options) {
    writer.WritePropertyName(value.ToString());
  }

  #endregion JsonConverter<SemanticVersion>
}


