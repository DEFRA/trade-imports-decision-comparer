using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public class ConnectedSilentRunningOperatingModeStrategy(ILogger<ConnectedSilentRunningOperatingModeStrategy> logger)
    : IOperatingModeStrategy
{
    public string DetermineDecision(ComparisonEntity comparison, Decision incomingDecision)
    {
        logger.LogInformation(
            "Returning incoming decision for {Mrn}, comparison date {Created:O}",
            comparison.Id,
            comparison.Latest.Created
        );

        return incomingDecision.Xml;
    }
}
