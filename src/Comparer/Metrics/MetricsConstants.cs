using System.Diagnostics.CodeAnalysis;

namespace Defra.TradeImportsDecisionComparer.Comparer.Metrics;

[ExcludeFromCodeCoverage]
public static class MetricsConstants
{
    public static class MetricNames
    {
        public const string MeterName = "Defra.TradeImportsDecisionComparer.Comparer";
    }

    public static class RequestTags
    {
        public const string Service = nameof(Service);
        public const string HttpMethod = nameof(HttpMethod);
        public const string RequestPath = nameof(RequestPath);
        public const string StatusCode = nameof(StatusCode);
        public const string ExceptionType = nameof(ExceptionType);
    }
}
