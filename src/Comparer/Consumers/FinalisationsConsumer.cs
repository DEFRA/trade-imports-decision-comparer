using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using SlimMessageBus;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsDecisionComparer.Comparer.Consumers;

[ExcludeFromCodeCoverage] // see integration tests
// ReSharper disable once ClassNeverInstantiated.Global
public class FinalisationsConsumer(
    IDecisionService decisionService,
    IComparisonService comparisonService,
    ILogger<FinalisationsConsumer> logger
) : IConsumer<JsonElement>, IConsumerWithContext
{
    private string MessageId => Context.GetTransportMessage().MessageId;

    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        logger.LogInformation("Consumed {MessageId}", MessageId);

        var message =
            received.Deserialize<ResourceEvent<CustomsDeclaration>>()
            ?? throw new InvalidOperationException("Could not deserialize resource event");

        logger.LogInformation("Processing {MessageId} for {ResourceId}", MessageId, message.ResourceId);

        var alvsDecision = await decisionService.GetAlvsDecision(message.ResourceId, cancellationToken);
        var btmsDecision = await decisionService.GetBtmsDecision(message.ResourceId, cancellationToken);
        var latestAlvs = alvsDecision?.Decisions.LastOrDefault();
        var latestBtms = btmsDecision?.Decisions.LastOrDefault();

        var comparison =
            await comparisonService.Get(message.ResourceId, cancellationToken)
            ?? new ComparisonEntity { Id = message.ResourceId, Comparisons = [] };

        comparison.Comparisons.Add(Comparison.Create(latestAlvs?.Xml, latestBtms?.Xml));

        await comparisonService.Save(comparison, cancellationToken);
    }

    public required IConsumerContext Context { get; set; }
}
