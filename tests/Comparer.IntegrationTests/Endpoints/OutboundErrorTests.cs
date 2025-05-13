using System.Net;
using System.Text;
using System.Text.Json;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using FluentAssertions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Endpoints;

public class OutboundErrorTests : IntegrationTestBase
{
    private static readonly JsonSerializerOptions s_options = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task WhenNoDecisions_ShouldBeNullResults()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsOutboundError.Should().BeNull();
    }

    [Fact]
    public async Task WhenAlvsDecisions_ShouldReturnResults()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Alvs.Put(mrn),
            new StringContent("<xml decision=\"1\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsOutboundError.Should().NotBeNull();
        result.AlvsOutboundError.Errors.Should().HaveCount(1);

        response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Alvs.Put(mrn),
            new StringContent("<xml decision=\"2\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        content = await response.Content.ReadAsStringAsync();
        result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsOutboundError.Should().NotBeNull();
        result.AlvsOutboundError.Errors.Should().HaveCount(2);
    }

    private record GetResponse(AlvsOutboundErrorEntity? AlvsOutboundError);
}
