using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.Decisions;

public class GetTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    private const string Mrn = "mrn";
    private IDecisionService MockGmrService { get; } = Substitute.For<IDecisionService>();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddTransient<IDecisionService>(_ => MockGmrService);
    }

    [Fact]
    public async Task Get_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenWriteOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.WriteOnly);

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_WhenAuthorized_ShouldBeOk()
    {
        var client = CreateClient();
        MockGmrService
            .GetAlvsDecision(Mrn, Arg.Any<CancellationToken>())
            .Returns(
                new AlvsDecisionEntity
                {
                    Id = Mrn,
                    Created = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Updated = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Decisions =
                    [
                        new Decision(new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc), "<xml decision=\"1\"/>"),
                    ],
                }
            );
        MockGmrService
            .GetBtmsDecision(Mrn, Arg.Any<CancellationToken>())
            .Returns(
                new BtmsDecisionEntity
                {
                    Id = Mrn,
                    Created = new DateTime(2025, 4, 23, 8, 31, 0, DateTimeKind.Utc),
                    Updated = new DateTime(2025, 4, 23, 8, 32, 0, DateTimeKind.Utc),
                    Decisions =
                    [
                        new Decision(new DateTime(2025, 4, 23, 8, 31, 0, DateTimeKind.Utc), "<xml decision=\"1\"/>"),
                        new Decision(new DateTime(2025, 4, 23, 8, 32, 0, DateTimeKind.Utc), "<xml decision=\"2\"/>"),
                    ],
                }
            );

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await VerifyJson(await response.Content.ReadAsStringAsync())
            .UseStrictJson()
            .DontIgnoreEmptyCollections()
            .DontScrubDateTimes();
    }
}
