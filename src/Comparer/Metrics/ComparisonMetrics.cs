using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Amazon.CloudWatch.EMF.Model;

namespace Defra.TradeImportsDecisionComparer.Comparer.Metrics;

[ExcludeFromCodeCoverage]
public class ComparisonMetrics : IComparisonMetrics
{
    private readonly Counter<long> _total;
    private readonly Counter<long> _totalBtms;
    private readonly Counter<long> _totalAlvs;

    public ComparisonMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricsConstants.MetricNames.MeterName);

        _total = meter.CreateCounter<long>(
            "ComparisonTotal",
            nameof(Unit.COUNT),
            description: "Total decisions compared"
        );
        _totalBtms = meter.CreateCounter<long>(
            "ComparisonTotalBtms",
            nameof(Unit.COUNT),
            description: "Total BTMS decisions"
        );
        _totalAlvs = meter.CreateCounter<long>(
            "ComparisonTotalAlvs",
            nameof(Unit.COUNT),
            description: "Total ALVS decisions"
        );
    }

    public void BtmsDecision()
    {
        var tagList = BuildTags();

        _total.Add(1, tagList);
        _totalBtms.Add(1, tagList);
    }

    public void AlvsDecision()
    {
        var tagList = BuildTags();

        _total.Add(1, tagList);
        _totalAlvs.Add(1, tagList);
    }

    private static TagList BuildTags()
    {
        return new TagList { { Constants.Tags.Service, Process.GetCurrentProcess().ProcessName } };
    }

    private static class Constants
    {
        public static class Tags
        {
            public const string Service = "ServiceName";
        }
    }
}
