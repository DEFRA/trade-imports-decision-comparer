using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Comparision;

public static class CheckComparer
{
    public static ComparisionOutcome Compare(this Check check, Check against)
    {
        if (check.Equals(against))
        {
            return ComparisionOutcome.ExactMatch;
        }

        if (!check.CheckCode.Equals(against.CheckCode))
        {
            return ComparisionOutcome.Mismatch;
        }

        if (check.DecisionCode.FirstOrDefault() == against.DecisionCode.FirstOrDefault())
        {
            return ComparisionOutcome.GroupMatch;
        }

        return ComparisionOutcome.Mismatch;
    }
}
