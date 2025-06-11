using System.Diagnostics.CodeAnalysis;
using Amazon.SQS.Model;
using SlimMessageBus;

namespace Defra.TradeImportsDecisionComparer.Comparer.Extensions;

[ExcludeFromCodeCoverage]
public static class MessageBusHeaders
{
    public const string SqsBusMessage = "Sqs_Message";
    public const string ResourceId = "ResourceId";
}

[ExcludeFromCodeCoverage]
public static class ConsumerContextExtensions
{
    public static string GetMessageId(this IConsumerContext consumerContext)
    {
        if (consumerContext.Properties.TryGetValue(MessageBusHeaders.SqsBusMessage, out var sqsMessage))
        {
            return ((Message)sqsMessage).MessageId;
        }

        return string.Empty;
    }

    public static string GetResourceId(this IConsumerContext consumerContext)
    {
        if (consumerContext.Headers.TryGetValue(MessageBusHeaders.ResourceId, out var value))
        {
            return value.ToString()!;
        }

        return string.Empty;
    }
}
