using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Comparision;

public enum ComparisionOutcome
{
    ExactMatch = 1,
    GroupMatch = 2,
    Mismatch = 3,
    NoAlvsDecision = 4,
    NoBtmsDecision = 5,
    CancelledMrn = 6,
}

public record ComparisionOutcomeEvaluatorContext(
    List<Item> AlvsItems,
    List<Item> BtmsItems,
    Finalisation? Finalisation
);

public static class ComparisionOutcomeEvaluator
{
    public static ComparisionOutcome GetComparisionOutcome(this ComparisionOutcomeEvaluatorContext context)
    {
        if (context.Finalisation?.FinalState is "1" or "2")
        {
            return ComparisionOutcome.CancelledMrn;
        }

        if (context.BtmsItems.Count == 0)
        {
            return ComparisionOutcome.NoBtmsDecision;
        }

        if (context.AlvsItems.Count == 0)
        {
            return ComparisionOutcome.NoAlvsDecision;
        }

        return context.BtmsItems.Compare(context.AlvsItems);
    }
}
