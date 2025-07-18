using System.Text.Json;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using Defra.TradeImportsDecisionComparer.Comparer.Consumers;
using Defra.TradeImportsDecisionComparer.Comparer.Metrics;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Defra.TradeImportsDecisionComparer.Comparer.Utils.Logging;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AmazonSQS;
using SlimMessageBus.Host.Interceptor;
using SlimMessageBus.Host.Serialization.SystemTextJson;

namespace Defra.TradeImportsDecisionComparer.Comparer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        var options = services
            .AddValidateOptions<FinalisationsConsumerOptions>(configuration, FinalisationsConsumerOptions.SectionName)
            .Get();

        if (!options.Enabled)
            return services;

        // Order of interceptors is important here
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(TraceContextInterceptor<>));
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(LoggingInterceptor<>));
        services.AddSingleton<ConsumerMetrics>();
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(MetricsInterceptor<>));

        services.AddSlimMessageBus(smb =>
        {
            smb.AddChildBus(
                "SQS_Finalisations",
                mbb =>
                {
                    mbb.WithProviderAmazonSQS(cfg =>
                    {
                        cfg.TopologyProvisioning.Enabled = false;
                        cfg.ClientProviderFactory = _ => new CdpCredentialsSqsClientProvider(
                            cfg.SqsClientConfig,
                            configuration
                        );
                    });
                    mbb.AddJsonSerializer();
                    mbb.AddServicesFromAssemblyContaining<FinalisationsConsumer>();
                    mbb.Consume<JsonElement>(x =>
                        x.WithConsumer<FinalisationsConsumer>()
                            .Queue(options.QueueName)
                            .Instances(options.ConsumersPerHost)
                    );
                }
            );
        });

        return services;
    }
}
