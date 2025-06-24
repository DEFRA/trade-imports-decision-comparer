using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Comparison(
    [property: JsonPropertyName("created")] DateTime Created,
    [property: JsonPropertyName("alvsXml")] string? AlvsXml,
    [property: JsonPropertyName("btmsXml")] string? BtmsXml,
    [property: JsonPropertyName("match")] ComparisionOutcome Match,
    [property: JsonPropertyName("reasons")] string[]? Reasons
)
{
    public static Comparison Create(string? alvsXml, string? btmsXml, Finalisation finalisation)
    {
        var alvsItems = Item.FromXml(alvsXml);
        var btmsItems = Item.FromXml(btmsXml);
        var outcome = new ComparisionOutcomeEvaluatorContext(
            alvsItems,
            btmsItems,
            finalisation
        ).GetComparisionOutcome();
        return new Comparison(DateTime.UtcNow, alvsXml, btmsXml, outcome, []);
    }
}
