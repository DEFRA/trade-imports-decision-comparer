using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Metrics;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Defra.TradeImportsDecisionComparer.Comparer.Utils;
using Defra.TradeImportsDecisionComparer.Testing.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Services;

public class TrialCutoverOperatingModeStrategyTests
{
    private IComparisonMetrics MockComparisonMetrics { get; } = Substitute.For<IComparisonMetrics>();
    private IRandom MockRandom { get; } = Substitute.For<IRandom>();

    [Fact]
    public void DetermineDecision_WhenMatch_AndDefaultZeroSamplingPercentage_ShouldBeIncomingDecision()
    {
        var subject = CreateSubject();
        var comparison = ComparisonFixtures.MatchComparison();
        var incomingDecision = new Decision(DateTime.UtcNow, "<xml />");

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            incomingDecision
        );

        result.Should().Be(incomingDecision.Xml);
        AssertMetrics(alvsDecision: true);
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
        AssertMetrics(alvsDecision: true, sampled: false);
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
        AssertMetrics(alvsDecision: true, sampled: false);
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
        AssertMetrics(alvsDecision: true, sampled: false);
    }

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(0.5, 0, false)]
    [InlineData(0.1, 10, true)]
    [InlineData(0.09, 10, true)]
    [InlineData(0.101, 10, false)]
    public void DetermineDecision_WhenMatch_AndSamplingPercentage_ShouldBeExpected(
        double nextDouble,
        int samplingPercentage,
        bool expectBtms
    )
    {
        MockRandom.NextDouble().Returns(nextDouble);
        var subject = CreateSubject(new BtmsOptions { DecisionSamplingPercentage = samplingPercentage });
        var comparison = ComparisonFixtures.MatchComparison();
        var incomingDecision = new Decision(DateTime.UtcNow, "<xml />");

        var result = subject.DetermineDecision(
            new ComparisonEntity { Id = "id", Latest = comparison },
            incomingDecision
        );

        if (expectBtms)
        {
            result.Should().Be(comparison.BtmsXml);
            AssertMetrics(btmsDecision: true);
        }
        else
        {
            result.Should().Be(incomingDecision.Xml);
            AssertMetrics(alvsDecision: true);
        }
    }

    private void AssertMetrics(bool btmsDecision = false, bool alvsDecision = false, bool sampled = true)
    {
        if (btmsDecision)
            MockComparisonMetrics.Received(1).BtmsDecision();
        else
            MockComparisonMetrics.DidNotReceive().BtmsDecision();

        if (alvsDecision)
            MockComparisonMetrics.Received(1).AlvsDecision();
        else
            MockComparisonMetrics.DidNotReceive().AlvsDecision();

        MockComparisonMetrics
            .Received(1)
            .Match(Arg.Any<bool>(), Arg.Any<ComparisionOutcome>(), Arg.Any<DecisionNumberMatch?>());

        if (sampled)
            MockComparisonMetrics.Received(1).Sampled(Arg.Any<bool>(), Arg.Any<int>());
        else
            MockComparisonMetrics.DidNotReceive().Sampled(Arg.Any<bool>(), Arg.Any<int>());
    }

    private TrialCutoverOperatingModeStrategy CreateSubject(BtmsOptions? btmsOptions = null)
    {
        return new TrialCutoverOperatingModeStrategy(
            NullLogger<TrialCutoverOperatingModeStrategy>.Instance,
            MockComparisonMetrics,
            new OptionsWrapper<BtmsOptions>(btmsOptions ?? new BtmsOptions()),
            MockRandom
        );
    }
}
