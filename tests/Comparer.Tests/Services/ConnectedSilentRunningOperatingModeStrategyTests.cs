using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Services;

public class ConnectedSilentRunningOperatingModeStrategyTests
{
    [Fact]
    public void DetermineDecision_ShouldBeIncoming()
    {
        var subject = new ConnectedSilentRunningOperatingModeStrategy(
            NullLogger<ConnectedSilentRunningOperatingModeStrategy>.Instance
        );

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = Comparison.Empty },
            new Decision(DateTime.UtcNow, "<xml />")
        );

        result.Should().Be("<xml />");
    }
}
