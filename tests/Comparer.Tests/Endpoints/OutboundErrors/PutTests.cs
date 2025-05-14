using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.OutboundErrors;

public class PutTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    private const string Mrn = "mrn";
    private IOutboundErrorService MockOutboundErrorServiceService { get; } = Substitute.For<IOutboundErrorService>();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddTransient<IOutboundErrorService>(_ => MockOutboundErrorServiceService);
    }

    [Fact]
    public async Task PutAlvs_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Alvs.Put(Mrn),
            new StringContent("<xml />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutAlvs_WhenReadOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.ReadOnly);

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Alvs.Put(Mrn),
            new StringContent("<xml />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutAlvs_WhenValid_ShouldBeRequestBodyAsResponse()
    {
        var client = CreateClient();

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Alvs.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        await Verify(content);
    }

    [Fact]
    public async Task PutBtms_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Btms.Put(Mrn),
            new StringContent("<xml />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutBtms_WhenReadOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.ReadOnly);

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Btms.Put(Mrn),
            new StringContent("<xml />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutBtms_WhenValid_ShouldBeRequestBodyAsResponse()
    {
        var client = CreateClient();

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Btms.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        await Verify(content);
    }
}
