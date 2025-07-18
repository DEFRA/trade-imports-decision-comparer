using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Projections;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.OutboundErrors;

public class ParityTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    private const string Mrn = "mrn";
    private IParityService MockParityService { get; } = Substitute.For<IParityService>();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddTransient<IParityService>(_ => MockParityService);
    }

    [Fact]
    public async Task Get_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Parity(null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenWriteOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.WriteOnly);

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Parity(null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_WhenAuthorized_ShouldBeOk()
    {
        var client = CreateClient();
        MockParityService
            .GetOutboundError(null, null, Arg.Any<CancellationToken>())
            .Returns(
                new OutboundErrorParityProjection(
                    new Dictionary<string, int> { { nameof(OutboundErrorComparisonOutcome.Mismatch), 3 } },
                    NoAlvsErrorsMrns: [Mrn],
                    NoBtmsErrorsMrns: [Mrn],
                    AlvsOnlyMrns: [Mrn],
                    BtmsOnlyMrns: [Mrn],
                    MismatchMrns: [Mrn],
                    HeaderMismatchMrns: [Mrn]
                )
            );

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Parity(null, null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await VerifyJson(await response.Content.ReadAsStringAsync())
            .UseStrictJson()
            .DontIgnoreEmptyCollections()
            .DontScrubDateTimes();
    }
}
