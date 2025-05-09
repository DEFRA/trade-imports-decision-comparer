using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Defra.TradeImportsDecisionComparer.Comparer.Health;

[ExcludeFromCodeCoverage]
public static class SqsHealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddSqs(
        this IHealthChecksBuilder builder,
        IConfiguration configuration,
        string name,
        Func<IServiceProvider, string> queueNameFunc,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null
    )
    {
        builder.Add(
            new HealthCheckRegistration(
                name,
                sp => new SqsHealthCheck(configuration, queueNameFunc(sp)),
                HealthStatus.Unhealthy,
                tags,
                timeout
            )
        );

        return builder;
    }
}
