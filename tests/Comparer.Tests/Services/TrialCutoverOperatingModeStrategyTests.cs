using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Defra.TradeImportsDecisionComparer.Testing.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Services;

public class TrialCutoverOperatingModeStrategyTests
{
    [Fact]
    public void DetermineDecision_WhenMatch_ShouldBeBtmsDecision()
    {
        var subject = new TrialCutoverOperatingModeStrategy(NullLogger<TrialCutoverOperatingModeStrategy>.Instance);
        var comparison = ComparisonFixtures.MatchComparison();

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            new Decision(DateTime.UtcNow, "<xml />")
        );

        result.Should().Be(comparison.BtmsXml);
    }

    [Fact]
    public void DetermineDecision_WhenMatchIsNotExactMatch_ShouldBeIncomingDecision()
    {
        var subject = new TrialCutoverOperatingModeStrategy(NullLogger<TrialCutoverOperatingModeStrategy>.Instance);
        var comparison = ComparisonFixtures.MatchComparison() with { Match = ComparisionOutcome.Mismatch };
        var incomingDecision = new Decision(DateTime.UtcNow, "<xml />");

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            incomingDecision
        );

        result.Should().Be(incomingDecision.Xml);
    }

    [Fact]
    public void DetermineDecision_WhenDecisionNumberMatchedIsNotExactMatch_ShouldBeIncomingDecision()
    {
        var subject = new TrialCutoverOperatingModeStrategy(NullLogger<TrialCutoverOperatingModeStrategy>.Instance);
        var comparison = ComparisonFixtures.MatchComparison() with
        {
            DecisionNumberMatched = DecisionNumberMatch.Mismatch,
        };
        var incomingDecision = new Decision(DateTime.UtcNow, "<xml />");

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            incomingDecision
        );

        result.Should().Be(incomingDecision.Xml);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DetermineDecision_WhenBtmsXmlIsNullOrEmpty_ShouldBeIncomingDecision(string? btmsXml)
    {
        var subject = new TrialCutoverOperatingModeStrategy(NullLogger<TrialCutoverOperatingModeStrategy>.Instance);
        var comparison = ComparisonFixtures.MatchComparison() with { BtmsXml = btmsXml };
        var incomingDecision = new Decision(DateTime.UtcNow, "<xml />");

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            incomingDecision
        );

        result.Should().Be(incomingDecision.Xml);
    }
}
