using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Metrics;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Defra.TradeImportsDecisionComparer.Testing.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Services;

public class TrialCutoverOperatingModeStrategyTests
{
    private IComparisonMetrics MockComparisonMetrics { get; } = Substitute.For<IComparisonMetrics>();

    [Fact]
    public void DetermineDecision_WhenMatch_ShouldBeBtmsDecision()
    {
        var subject = CreateSubject();
        var comparison = ComparisonFixtures.MatchComparison();

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            new Decision(DateTime.UtcNow, "<xml />")
        );

        result.Should().Be(comparison.BtmsXml);
        AssertMetrics(btmsDecision: true);
    }

    [Fact]
    public void DetermineDecision_WhenMatchIsNotExactMatch_ShouldBeIncomingDecision()
    {
        var subject = CreateSubject();
        var comparison = ComparisonFixtures.MatchComparison() with { Match = ComparisionOutcome.Mismatch };
        var incomingDecision = new Decision(DateTime.UtcNow, "<xml />");

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            incomingDecision
        );

        result.Should().Be(incomingDecision.Xml);
        AssertMetrics(alvsDecision: true);
    }

    [Fact]
    public void DetermineDecision_WhenDecisionNumberMatchedIsNotExactMatch_ShouldBeIncomingDecision()
    {
        var subject = CreateSubject();
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
        AssertMetrics(alvsDecision: true);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DetermineDecision_WhenBtmsXmlIsNullOrEmpty_ShouldBeIncomingDecision(string? btmsXml)
    {
        var subject = CreateSubject();
        var comparison = ComparisonFixtures.MatchComparison() with { BtmsXml = btmsXml };
        var incomingDecision = new Decision(DateTime.UtcNow, "<xml />");

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            incomingDecision
        );

        result.Should().Be(incomingDecision.Xml);
        AssertMetrics(alvsDecision: true);
    }

    private void AssertMetrics(bool btmsDecision = false, bool alvsDecision = false)
    {
        if (btmsDecision)
            MockComparisonMetrics.Received(1).BtmsDecision();
        else
            MockComparisonMetrics.DidNotReceive().BtmsDecision();

        if (alvsDecision)
            MockComparisonMetrics.Received(1).AlvsDecision();
        else
            MockComparisonMetrics.DidNotReceive().AlvsDecision();
    }

    private TrialCutoverOperatingModeStrategy CreateSubject()
    {
        return new TrialCutoverOperatingModeStrategy(
            NullLogger<TrialCutoverOperatingModeStrategy>.Instance,
            MockComparisonMetrics
        );
    }
}
