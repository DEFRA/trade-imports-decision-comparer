using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using SlimMessageBus;

namespace Defra.TradeImportsDecisionComparer.Comparer.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
public class FinalisationsConsumer(IComparisonManager comparisonManager, ILogger<FinalisationsConsumer> logger)
    : IConsumer<JsonElement>,
        IConsumerWithContext
{
    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var message =
            received.Deserialize<ResourceEvent<CustomsDeclaration>>()
            ?? throw new InvalidOperationException("Could not deserialize resource event");

        logger.LogInformation("Received finalisation for {ResourceId}", message.ResourceId);

        var finalisation = message.Resource?.Finalisation;
        await comparisonManager.CompareLatestDecisions(message.ResourceId, finalisation, cancellationToken);
    }

    public required IConsumerContext Context { get; set; }
}
