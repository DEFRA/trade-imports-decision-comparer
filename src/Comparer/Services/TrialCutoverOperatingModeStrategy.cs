using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Metrics;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public class TrialCutoverOperatingModeStrategy(
    ILogger<TrialCutoverOperatingModeStrategy> logger,
    IComparisonMetrics comparisonMetrics
) : IOperatingModeStrategy
{
    public string DetermineDecision(ComparisonEntity comparison, Decision incomingDecision)
    {
        if (
            comparison.Latest
                is { Match: ComparisionOutcome.ExactMatch, DecisionNumberMatched: DecisionNumberMatch.ExactMatch }
            && !string.IsNullOrEmpty(comparison.Latest.BtmsXml)
        )
        {
            return UseBtmsDecision(comparison);
        }

        return UseIncomingDecision(comparison, incomingDecision);
    }

    private string UseBtmsDecision(ComparisonEntity comparison)
    {
        logger.LogInformation(
            "Returning BTMS decision for {Mrn}, comparison date {Created:O}",
            comparison.Id,
            comparison.Latest.Created
        );

        comparisonMetrics.BtmsDecision();

        return comparison.Latest.BtmsXml!;
    }

    private string UseIncomingDecision(ComparisonEntity comparison, Decision incomingDecision)
    {
        logger.LogInformation(
            "Returning incoming decision for {Mrn}, comparison date {Created:O}",
            comparison.Id,
            comparison.Latest.Created
        );

        comparisonMetrics.AlvsDecision();

        return incomingDecision.Xml;
    }
}
