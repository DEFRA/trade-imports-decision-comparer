using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IOperatingModeStrategy
{
    string DetermineDecision(ComparisonEntity comparison, Decision incomingDecision);
}
