using System.Text.Json.Serialization;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record OutboundErrorComparison(
    [property: JsonPropertyName("created")] DateTime Created,
    [property: JsonPropertyName("alvsXml")] string? AlvsXml,
    [property: JsonPropertyName("btmsXml")] string? BtmsXml,
    [property: JsonPropertyName("match")] OutboundErrorComparisonOutcome Match
)
{
    public static OutboundErrorComparison Create(string? alvsXml, string? btmsXml)
    {
        var alvsHeader = ErrorHeader.FromXml(alvsXml);
        var alvsErrors = Error.FromXml(alvsXml);
        var btmsHeader = ErrorHeader.FromXml(btmsXml);
        var btmsErrors = Error.FromXml(btmsXml);

        var context = new OutboundErrorComparisonOutcomeEvaluatorContext(
            alvsHeader,
            alvsErrors,
            btmsHeader,
            btmsErrors
        );

        return new OutboundErrorComparison(DateTime.UtcNow, alvsXml, btmsXml, context.GetComparisionOutcome());
    }
}
