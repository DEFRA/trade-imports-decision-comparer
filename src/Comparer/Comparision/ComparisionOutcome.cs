using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Comparision;

public enum ComparisionOutcome
{
    ExactMatch = 1,
    GroupMatch = 2,
    Mismatch = 3,
    NoAlvsDecision = 4,
    NoBtmsDecision = 5,
}

public record ComparisionOutcomeEvaluatorContext(List<Item> AlvsItems, List<Item> BtmsItems);

public static class ComparisionOutcomeEvaluator
{
    public static ComparisionOutcome GetComparisionOutcome(this ComparisionOutcomeEvaluatorContext context)
    {
        if (!context.BtmsItems.Any())
        {
            return ComparisionOutcome.NoBtmsDecision;
        }

        if (!context.AlvsItems.Any())
        {
            return ComparisionOutcome.NoAlvsDecision;
        }

        return context.BtmsItems.Compare(context.AlvsItems);
    }
}
