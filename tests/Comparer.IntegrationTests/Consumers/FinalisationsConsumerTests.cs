using System.Net;
using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using FluentAssertions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Consumers;

[Collection(nameof(WireMockClientCollection))]
public class FinalisationsConsumerTests(ITestOutputHelper output) : SqsTestBase(output)
{
    [Fact]
    public async Task SendAndReceive()
    {
        var mrn = Guid.NewGuid().ToString("N");
        var message = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Update",
        };
        await SendMessage(JsonSerializer.Serialize(message));

        var client = CreateClient();

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var response = await client.GetAsync($"/comparisons/{mrn}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var content = await response.Content.ReadAsStringAsync();

                return content.Contains("alvsXml");
            })
        );
    }
}
