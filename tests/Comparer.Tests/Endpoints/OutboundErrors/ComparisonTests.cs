using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.OutboundErrors;

public class ComparisonTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    private const string Mrn = "mrn";
    private IComparisonService MockComparerService { get; } = Substitute.For<IComparisonService>();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddTransient<IComparisonService>(_ => MockComparerService);
    }

    [Fact]
    public async Task Get_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Comparison(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenWriteOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.WriteOnly);

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Comparison(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_WhenAuthorized_ShouldBeOk()
    {
        var client = CreateClient();
        MockComparerService
            .GetOutboundError(Mrn, Arg.Any<CancellationToken>())
            .Returns(
                new OutboundErrorComparisonEntity
                {
                    Id = Mrn,
                    Created = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Updated = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Latest = new OutboundErrorComparison(
                        new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                        "<xml alvs=\"true\"/>",
                        "<xml btms=\"true\"/>",
                        OutboundErrorComparisonOutcome.LegacyAlvsErrorCode
                    ),
                }
            );

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Comparison(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await VerifyJson(await response.Content.ReadAsStringAsync())
            .UseStrictJson()
            .DontIgnoreEmptyCollections()
            .DontScrubDateTimes();
    }
}
