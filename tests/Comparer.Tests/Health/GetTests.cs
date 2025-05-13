using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Health;

public class GetTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    [Fact]
    public async Task Get_Health_ReturnsAnonymous()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.GetAsync("/health");

        await Verify(response);
        await Verify(await response.Content.ReadAsStringAsync())
            .UseMethodName(nameof(Get_Health_ReturnsAnonymous) + "_content");
    }

    [Fact]
    public async Task Get_HealthAuthorized_ReturnsUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.GetAsync("/health/authorized");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_HealthAuthorized_WhenAuthorized_ReturnsOk()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/health/authorized");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
