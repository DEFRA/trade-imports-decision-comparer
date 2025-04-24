using System.Text.Json.Serialization;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Comparison(
    [property: JsonPropertyName("created")] DateTime Created,
    [property: JsonPropertyName("alvsXml")] string? AlvsXml,
    [property: JsonPropertyName("btmsXml")] string? BtmsXml,
    [property: JsonPropertyName("match")] bool Match,
    [property: JsonPropertyName("reasons")] string[]? Reasons
);
