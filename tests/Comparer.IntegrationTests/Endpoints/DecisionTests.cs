using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using FluentAssertions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Endpoints;

public class DecisionTests : IntegrationTestBase
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    [Fact]
    public async Task WhenNoDecisions_ShouldBeNullResults()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsDecision.Should().BeNull();
        result.BtmsDecision.Should().BeNull();
    }

    [Fact]
    public async Task WhenAlvsDecisions_ShouldReturnResults()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent("<xml decision=\"1\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.Decisions.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsDecision.Should().NotBeNull();
        result.AlvsDecision.Decisions.Should().HaveCount(1);
        result.BtmsDecision.Should().BeNull();

        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent("<xml decision=\"2\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.Decisions.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        content = await response.Content.ReadAsStringAsync();
        result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsDecision.Should().NotBeNull();
        result.AlvsDecision.Decisions.Should().HaveCount(2);
        result.BtmsDecision.Should().BeNull();
    }

    [Fact]
    public async Task WhenPutAlvsDecision_ItShouldCreateAComparison()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        const string decisionOne = "<xml decision=\"1\" />";
        const string decisionTwo = "<xml decision=\"2\" />";

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent(decisionOne, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.Decisions.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.Decisions.Comparison(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        var result =
            JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
            ?? throw new Exception("Failed to deserialize");
        result.Latest.AlvsXml.Should().Be(decisionOne);
        result.Latest.IsFinalisation.Should().BeFalse();

        await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent(decisionTwo, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.Decisions.Comparison(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content = await response.Content.ReadAsStringAsync();

        result =
            JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
            ?? throw new Exception("Failed to deserialize");
        result.Latest.AlvsXml.Should().Be(decisionTwo);
        result.History.Count.Should().Be(1);
    }

    [Fact]
    public async Task WhenBtmsDecisions_ShouldReturnResults()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent("<xml decision=\"1\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.Decisions.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsDecision.Should().BeNull();
        result.BtmsDecision.Should().NotBeNull();
        result.BtmsDecision.Decisions.Should().HaveCount(1);

        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent("<xml decision=\"2\" />", Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.GetAsync(Testing.Endpoints.Decisions.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        content = await response.Content.ReadAsStringAsync();
        result =
            JsonSerializer.Deserialize<GetResponse>(content, s_options) ?? throw new Exception("Failed to deserialize");
        result.AlvsDecision.Should().BeNull();
        result.BtmsDecision.Should().NotBeNull();
        result.BtmsDecision.Decisions.Should().HaveCount(2);
    }

    private record GetResponse(AlvsDecisionEntity? AlvsDecision, BtmsDecisionEntity? BtmsDecision);
}
