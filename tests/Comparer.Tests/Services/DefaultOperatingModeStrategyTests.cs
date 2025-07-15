using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Services;

public class DefaultOperatingModeStrategyTests
{
    [Fact]
    public void DetermineDecision_NotImplemented()
    {
        var subject = new DefaultOperatingModeStrategy();

        var act = () =>
            subject.DetermineDecision(
                new ComparisonEntity { Id = "id", Latest = Comparison.Empty },
                new Decision(DateTime.UtcNow, "<xml />")
            );

        act.Should().Throw<NotImplementedException>();
    }
}
