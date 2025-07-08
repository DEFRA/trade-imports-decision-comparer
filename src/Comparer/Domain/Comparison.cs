using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Comparison(
    [property: JsonPropertyName("created")] DateTime Created,
    [property: JsonPropertyName("alvsXml")] string? AlvsXml,
    [property: JsonPropertyName("btmsXml")] string? BtmsXml,
    [property: JsonPropertyName("match")] ComparisionOutcome Match,
    [property: JsonPropertyName("isFinalisation")] bool? IsFinalisation,
    [property: JsonPropertyName("alvsTimestamp")] DateTime? AlvsTimestamp,
    [property: JsonPropertyName("btmsTimestamp")] DateTime? BtmsTimestamp,
    [property: JsonPropertyName("decisionNumberMatched")] DecisionNumberMatch? DecisionNumberMatched,
    [property: JsonPropertyName("reasons")] string[]? Reasons
)
{
    public static Comparison Create(string? alvsXml, string? btmsXml, Finalisation finalisation)
    {
        var alvsItems = Item.FromXml(alvsXml);
        var alvsTimestamp = ServiceHeader.FromXml(alvsXml).ServiceCallTimestamp;
        var btmsItems = Item.FromXml(btmsXml);
        var btmsTimestamp = ServiceHeader.FromXml(btmsXml).ServiceCallTimestamp;

        var comparisonOutcome = new ComparisionOutcomeEvaluatorContext(
            alvsItems,
            btmsItems,
            finalisation
        ).GetComparisionOutcome();

        return new Comparison(
            DateTime.UtcNow,
            alvsXml,
            btmsXml,
            comparisonOutcome,
            true,
            alvsTimestamp,
            btmsTimestamp,
            GetDecisionNumberMatch(alvsXml, btmsXml, comparisonOutcome),
            []
        );
    }

    private static DecisionNumberMatch? GetDecisionNumberMatch(
        string? alvsXml,
        string? btmsXml,
        ComparisionOutcome comparisionOutcome
    )
    {
        if (comparisionOutcome is not (ComparisionOutcome.ExactMatch or ComparisionOutcome.GroupMatch))
        {
            return null;
        }

        var alvsDecisionNumber = Header.FromXml(alvsXml).DecisionNumber;
        var btmsDecisionNumber = Header.FromXml(btmsXml).DecisionNumber;

        return alvsDecisionNumber != null && btmsDecisionNumber != null && alvsDecisionNumber == btmsDecisionNumber
            ? DecisionNumberMatch.ExactMatch
            : DecisionNumberMatch.Mismatch;
    }
}
