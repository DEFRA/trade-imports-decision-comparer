using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.OutboundErrors;

public class GetTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
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
    public async Task Get_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenWriteOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.WriteOnly);

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_WhenAuthorized_ShouldBeOk()
    {
        var client = CreateClient();
        MockOutboundErrorServiceService
            .GetAlvsOutboundError(Mrn, Arg.Any<CancellationToken>())
            .Returns(
                new AlvsOutboundErrorEntity
                {
                    Id = Mrn,
                    Created = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Updated = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Errors =
                    [
                        new OutboundError(new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc), "<xml error=\"1\"/>"),
                    ],
                }
            );

        MockOutboundErrorServiceService
            .GetBtmsOutboundError(Mrn, Arg.Any<CancellationToken>())
            .Returns(
                new BtmsOutboundErrorEntity
                {
                    Id = Mrn,
                    Created = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Updated = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Errors =
                    [
                        new OutboundError(new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc), "<xml error=\"1\"/>"),
                    ],
                }
            );

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await VerifyJson(await response.Content.ReadAsStringAsync())
            .UseStrictJson()
            .DontIgnoreEmptyCollections()
            .DontScrubDateTimes();
    }
}
