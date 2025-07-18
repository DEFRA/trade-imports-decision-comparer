using Defra.TradeImportsDecisionComparer.Comparer.Comparision;

namespace Defra.TradeImportsDecisionComparer.Comparer.Metrics;

public interface IComparisonMetrics
{
    void BtmsDecision();

    void AlvsDecision();

    void Match(bool match, ComparisionOutcome comparisionOutcome, DecisionNumberMatch? decisionNumberMatch);

    void Sampled(bool sampled, int percentage);
}
