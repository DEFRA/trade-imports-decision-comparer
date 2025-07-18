using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Metrics;
using Defra.TradeImportsDecisionComparer.Comparer.Utils;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public class TrialCutoverOperatingModeStrategy(
    ILogger<TrialCutoverOperatingModeStrategy> logger,
    IComparisonMetrics comparisonMetrics,
    IOptions<BtmsOptions> btmsOptions,
    IRandom random
) : IOperatingModeStrategy
{
    public string DetermineDecision(ComparisonEntity comparison, Decision incomingDecision)
    {
        comparisonMetrics.SamplePercentage(btmsOptions.Value.DecisionSamplingPercentage);

        if (DecisionMatches(comparison) && IsSamplingReached())
        {
            return UseBtmsDecision(comparison);
        }

        return UseIncomingDecision(comparison, incomingDecision);
    }

    private bool IsSamplingReached()
    {
        var result =
            btmsOptions.Value.DecisionSamplingPercentage > 0
            && random.NextDouble() * 100 <= btmsOptions.Value.DecisionSamplingPercentage;

        comparisonMetrics.Sampled(result);

        return result;
    }

    private bool DecisionMatches(ComparisonEntity comparison)
    {
        var result =
            comparison.Latest
                is { Match: ComparisionOutcome.ExactMatch, DecisionNumberMatched: DecisionNumberMatch.ExactMatch }
            && !string.IsNullOrEmpty(comparison.Latest.BtmsXml);

        comparisonMetrics.Match(result, comparison.Latest.Match, comparison.Latest.DecisionNumberMatched);

        return result;
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
