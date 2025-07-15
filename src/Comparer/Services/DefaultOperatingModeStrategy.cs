using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public class DefaultOperatingModeStrategy : IOperatingModeStrategy
{
    public string DetermineDecision(ComparisonEntity comparison, Decision incomingDecision) =>
        throw new NotImplementedException();
}
