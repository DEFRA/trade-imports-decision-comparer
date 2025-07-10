using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Projections;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.Decisions;

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

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenWriteOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.WriteOnly);

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_WhenAuthorized_ShouldBeOk()
    {
        var client = CreateClient();
        MockParityService
            .Get(null, null, true, Arg.Any<CancellationToken>())
            .Returns(
                new ParityProjection(
                    new Dictionary<string, int> { { nameof(ComparisionOutcome.Mismatch), 3 } },
                    new Dictionary<string, int>
                    {
                        { nameof(DecisionNumberMatch.Mismatch), 3 },
                        { "countWhereAlvsRespondedFirst", 1 },
                        { "countWhereBtmsRespondedFirst", 2 },
                    },
                    [Mrn],
                    [Mrn]
                )
            );

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(null, null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await VerifyJson(await response.Content.ReadAsStringAsync())
            .UseStrictJson()
            .DontIgnoreEmptyCollections()
            .DontScrubDateTimes();
    }

    [Fact]
    public async Task Get_WithIsFinalisationFalse_ShouldQueryWithIsFinalisationFalse()
    {
        var client = CreateClient();
        MockParityService
            .Get(null, null, false, Arg.Any<CancellationToken>())
            .Returns(
                new ParityProjection(
                    new Dictionary<string, int> { { nameof(ComparisionOutcome.Mismatch), 3 } },
                    new Dictionary<string, int>(),
                    [Mrn],
                    []
                )
            );

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(null, null, false));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await MockParityService.Received(1).Get(null, null, false, Arg.Any<CancellationToken>());
    }
}
