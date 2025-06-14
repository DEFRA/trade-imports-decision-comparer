using System.Diagnostics.CodeAnalysis;
using Amazon.SQS.Model;
using SlimMessageBus;

namespace Defra.TradeImportsDecisionComparer.Comparer.Extensions;

[ExcludeFromCodeCoverage]
public static class MessageBusHeaders
{
    public const string ResourceType = nameof(ResourceType);
    public const string SubResourceType = nameof(SubResourceType);
    public const string SqsBusMessage = "Sqs_Message";
    public const string ResourceId = nameof(ResourceId);
}

[ExcludeFromCodeCoverage]
public static class ConsumerContextExtensions
{
    public static string GetMessageId(this IConsumerContext consumerContext)
    {
        return consumerContext.Properties.TryGetValue(MessageBusHeaders.SqsBusMessage, out var sqsMessage)
            ? ((Message)sqsMessage).MessageId
            : string.Empty;
    }

    public static string GetResourceType(this IConsumerContext consumerContext)
    {
        return consumerContext.Headers.TryGetValue(MessageBusHeaders.ResourceType, out var value)
            ? value.ToString()!
            : string.Empty;
    }

    public static string GetSubResourceType(this IConsumerContext consumerContext)
    {
        return consumerContext.Headers.TryGetValue(MessageBusHeaders.SubResourceType, out var value)
            ? value.ToString()!
            : string.Empty;
    }

    public static string GetResourceId(this IConsumerContext consumerContext)
    {
        return consumerContext.Headers.TryGetValue(MessageBusHeaders.ResourceId, out var value)
            ? value.ToString()!
            : string.Empty;
    }
}
