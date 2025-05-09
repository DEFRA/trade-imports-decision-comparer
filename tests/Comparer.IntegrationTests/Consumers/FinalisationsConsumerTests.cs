using System.Net;
using System.Text;
using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using FluentAssertions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Consumers;

public class FinalisationsConsumerTests(ITestOutputHelper output) : SqsTestBase(output)
{
    private static readonly JsonSerializerOptions s_options = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task WhenFinalised_AndNoDecisions()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");
        var message = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Update",
        };
        await DrainAllMessages();
        await SendMessage(JsonSerializer.Serialize(message));

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var response = await client.GetAsync($"/comparisons/{mrn}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return false;

                var entity =
                    JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
                    ?? throw new Exception("Failed to deserialize");

                return entity.Comparisons is [{ AlvsXml: null, BtmsXml: null }];
            })
        );
    }

    [Fact]
    public async Task WhenFinalised_AndExistingDecisions()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent("<xml alvs=\"true\" version=\"1\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent("<xml btms=\"true\" version=\"1\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Update",
        };
        await DrainAllMessages();
        await SendMessage(JsonSerializer.Serialize(message));

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                response = await client.GetAsync($"/comparisons/{mrn}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return false;

                var entity =
                    JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
                    ?? throw new Exception("Failed to deserialize");

                return entity.Comparisons
                    is [
                        {
                            AlvsXml: "<xml alvs=\"true\" version=\"1\" />",
                            BtmsXml: "<xml btms=\"true\" version=\"1\" />"
                        },
                    ];
            })
        );

        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent("<xml btms=\"true\" version=\"2\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        message = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Update",
        };
        await DrainAllMessages();
        await SendMessage(JsonSerializer.Serialize(message));

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                response = await client.GetAsync($"/comparisons/{mrn}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return false;

                var entity =
                    JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
                    ?? throw new Exception("Failed to deserialize");

                return entity.Comparisons
                    is [
                        {
                            AlvsXml: "<xml alvs=\"true\" version=\"1\" />",
                            BtmsXml: "<xml btms=\"true\" version=\"1\" />"
                        },
                        {
                            AlvsXml: "<xml alvs=\"true\" version=\"1\" />",
                            BtmsXml: "<xml btms=\"true\" version=\"2\" />"
                        },
                    ];
            })
        );
    }
}
