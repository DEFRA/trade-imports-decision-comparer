using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Comparision;

public enum ComparisionOutcome
{
    ExactMatch = 1,
    GroupMatch = 2,
    Mismatch = 3,
}

public record ComparisionOutcomeEvaluatorContext(List<Item> AlvsItems, List<Item> BtmsItems);

public static class ComparisionOutcomeEvaluator
{
    public static ComparisionOutcome GetComparisionOutcome(this ComparisionOutcomeEvaluatorContext context)
    {
        return context.BtmsItems.Compare(context.AlvsItems);
    }
}
