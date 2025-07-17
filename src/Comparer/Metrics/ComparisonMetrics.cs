using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Amazon.CloudWatch.EMF.Model;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;

namespace Defra.TradeImportsDecisionComparer.Comparer.Metrics;

[ExcludeFromCodeCoverage]
public class ComparisonMetrics : IComparisonMetrics
{
    private readonly Counter<long> _decisions;
    private readonly Counter<long> _btmsDecisions;
    private readonly Counter<long> _alvsDecisions;
    private readonly Counter<long> _sampled;
    private readonly Gauge<int> _samplingPercentage;
    private readonly Counter<long> _match;

    public ComparisonMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricsConstants.MetricNames.MeterName);

        _decisions = meter.CreateCounter<long>(
            "ComparisonDecisions",
            nameof(Unit.COUNT),
            description: "All decisions compared"
        );
        _btmsDecisions = meter.CreateCounter<long>(
            "ComparisonBtmsDecisions",
            nameof(Unit.COUNT),
            description: "BTMS decisions"
        );
        _alvsDecisions = meter.CreateCounter<long>(
            "ComparisonAlvsDecisions",
            nameof(Unit.COUNT),
            description: "ALVS decisions"
        );
        _sampled = meter.CreateCounter<long>("ComparisonSampled", nameof(Unit.COUNT), description: "Sampled decisions");
        _samplingPercentage = meter.CreateGauge<int>(
            "ComparisonSamplingPercentage",
            nameof(Unit.NONE),
            description: "Current sampling percentage"
        );
        _match = meter.CreateCounter<long>("ComparisonMatch", nameof(Unit.COUNT), description: "Matched decisions");
    }

    public void BtmsDecision()
    {
        var tagList = BuildTags();

        IncrementTotal(tagList);
        _btmsDecisions.Add(1, tagList);
    }

    public void AlvsDecision()
    {
        var tagList = BuildTags();

        IncrementTotal(tagList);
        _alvsDecisions.Add(1, tagList);
    }

    public void Match(bool match, ComparisionOutcome comparisionOutcome, DecisionNumberMatch? decisionNumberMatch)
    {
        var tagList = BuildTags();

        tagList.Add(Constants.Tags.Match, match.ToString().ToLower());
        tagList.Add(Constants.Tags.ComparisionOutcome, comparisionOutcome.ToString());

        if (decisionNumberMatch is not null)
            tagList.Add(Constants.Tags.DecisionNumberMatch, decisionNumberMatch.ToString());

        _match.Add(1, tagList);
    }

    public void Sampled(bool sampled, int percentage)
    {
        var tagList = BuildTags();

        tagList.Add(Constants.Tags.Sampled, sampled.ToString().ToLower());

        _sampled.Add(1, tagList);
        _samplingPercentage.Record(percentage, tagList);
    }

    private static TagList BuildTags() => new() { { Constants.Tags.Service, Process.GetCurrentProcess().ProcessName } };

    private void IncrementTotal(TagList tagList) => _decisions.Add(1, tagList);

    private static class Constants
    {
        public static class Tags
        {
            public const string Service = "ServiceName";
            public const string Sampled = "Sampled";
            public const string Match = "Match";
            public const string ComparisionOutcome = "ComparisionOutcome";
            public const string DecisionNumberMatch = "DecisionNumberMatch";
        }
    }
}
