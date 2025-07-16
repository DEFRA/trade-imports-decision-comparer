using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Testing.Fixtures;

public static class ComparisonFixtures
{
    public static Comparison MatchComparison() =>
        new(
            DateTime.UtcNow,
            "<xml alvs=\"true\" />",
            "<xml btms=\"true\" />",
            ComparisionOutcome.ExactMatch,
            null,
            null,
            null,
            DecisionNumberMatch.ExactMatch,
            null
        );
}
