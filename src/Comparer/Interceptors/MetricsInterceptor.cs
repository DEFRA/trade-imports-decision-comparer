using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Metrics;
using SlimMessageBus;
using SlimMessageBus.Host.Interceptor;

namespace Defra.TradeImportsDecisionComparer.Comparer.Interceptors;

[ExcludeFromCodeCoverage]
public class MetricsInterceptor<TMessage>(ConsumerMetrics consumerMetrics) : IConsumerInterceptor<TMessage>
    where TMessage : notnull
{
    public async Task<object> OnHandle(TMessage message, Func<Task<object>> next, IConsumerContext context)
    {
        var startingTimestamp = TimeProvider.System.GetTimestamp();
        var resourceType = context.GetResourceType();
        var subResourceType = context.GetSubResourceType();

        try
        {
            consumerMetrics.Start(context.Path, context.Consumer.GetType().Name, resourceType, subResourceType);
            return await next();
        }
        catch (Exception exception)
        {
            consumerMetrics.Faulted(
                context.Path,
                context.Consumer.GetType().Name,
                resourceType,
                subResourceType,
                exception
            );
            throw;
        }
        finally
        {
            consumerMetrics.Complete(
                context.Path,
                context.Consumer.GetType().Name,
                TimeProvider.System.GetElapsedTime(startingTimestamp).TotalMilliseconds,
                resourceType,
                subResourceType
            );
        }
    }
}
