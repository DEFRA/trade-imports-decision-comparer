using System.Text.Json.Serialization;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Check(
    [property: JsonPropertyName("checkCode")] string CheckCode,
    [property: JsonPropertyName("decisionCode")] string DecisionCode
);
