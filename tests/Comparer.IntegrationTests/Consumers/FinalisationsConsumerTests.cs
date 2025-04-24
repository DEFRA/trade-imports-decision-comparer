using FluentAssertions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Consumers;

[Collection(nameof(WireMockClientCollection))]
public class FinalisationsConsumerTests(ITestOutputHelper output) : SqsTestBase(output)
{
    [Fact]
    public async Task SendAndReceive()
    {
        await SendMessage("{\"json\": true}");
        await ReceiveMessage();

        true.Should().BeTrue();
    }
}
