using System.Text.Json;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsDecisionComparer.Comparer.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
public class FinalisationsConsumer(ILogger<FinalisationsConsumer> logger) : IConsumer<JsonElement>, IConsumerWithContext
{
    private string MessageId => Context.GetTransportMessage().MessageId;

    public Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        logger.LogInformation("Consumed {MessageId}", MessageId);

        return Task.CompletedTask;
    }

    public required IConsumerContext Context { get; set; }
}
