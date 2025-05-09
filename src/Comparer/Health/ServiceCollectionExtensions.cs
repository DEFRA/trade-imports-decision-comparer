using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Defra.TradeImportsDecisionComparer.Comparer.Health;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddMongoDb(
                provider => provider.GetRequiredService<IMongoDatabase>(),
                timeout: TimeSpan.FromSeconds(10),
                tags: [WebApplicationExtensions.Extended]
            )
            .AddSqs(
                configuration,
                "Data events SQS queue",
                sp => sp.GetRequiredService<IOptions<FinalisationsConsumerOptions>>().Value.QueueName,
                tags: [WebApplicationExtensions.Extended],
                timeout: TimeSpan.FromSeconds(10)
            );

        return services;
    }
}
