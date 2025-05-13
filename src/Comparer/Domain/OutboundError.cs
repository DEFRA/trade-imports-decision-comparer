using System.Text.Json.Serialization;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record OutboundError(
    [property: JsonPropertyName("created")] DateTime Created,
    [property: JsonPropertyName("xml")] string Xml
);
