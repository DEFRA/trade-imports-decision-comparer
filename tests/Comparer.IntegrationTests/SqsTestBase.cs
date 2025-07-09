using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests;

public class SqsTestBase(ITestOutputHelper output) : IntegrationTestBase
{
    private const string QueueUrl =
        "http://sqs.eu-west-2.localhost.localstack.cloud:4566/000000000000/trade_imports_data_upserted_decision_comparer";

    private readonly AmazonSQSClient _sqsClient = new(
        new BasicAWSCredentials("test", "test"),
        new AmazonSQSConfig { AuthenticationRegion = "eu-west-2", ServiceURL = "http://localhost:4566" }
    );

    private Task<ReceiveMessageResponse> ReceiveMessage()
    {
        return _sqsClient.ReceiveMessageAsync(QueueUrl, CancellationToken.None);
    }

    private Task<GetQueueAttributesResponse> GetQueueAttributes()
    {
        return _sqsClient.GetQueueAttributesAsync(
            new GetQueueAttributesRequest { AttributeNames = ["ApproximateNumberOfMessages"], QueueUrl = QueueUrl },
            CancellationToken.None
        );
    }

    protected async Task DrainAllMessages()
    {
        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                await ReceiveMessage();
                return (await GetQueueAttributes()).ApproximateNumberOfMessages == 0;
            })
        );
    }

    protected async Task WaitOnMessagesBeingProcessed()
    {
        await AsyncWaiter.WaitForAsync(async () =>
        {
            await Task.Delay(500);
            return (await GetQueueAttributes()).ApproximateNumberOfMessages == 0;
        });
    }

    protected async Task SendMessage(string body, Dictionary<string, MessageAttributeValue>? messageAttributes = null)
    {
        var request = new SendMessageRequest
        {
            MessageAttributes = messageAttributes,
            MessageBody = body,
            QueueUrl = QueueUrl,
        };
        var result = await _sqsClient.SendMessageAsync(request, CancellationToken.None);
        output.WriteLine("Sent {0} to {1}", result.MessageId, QueueUrl);
    }
}
