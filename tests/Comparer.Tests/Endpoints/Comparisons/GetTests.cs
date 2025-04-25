using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.Comparisons;

public class GetTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
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

        var response = await client.GetAsync(Testing.Endpoints.Comparisons.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenWriteOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.WriteOnly);

        var response = await client.GetAsync(Testing.Endpoints.Comparisons.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_WhenAuthorized_ShouldBeOk()
    {
        var client = CreateClient();
        MockComparerService
            .Get(Mrn, Arg.Any<CancellationToken>())
            .Returns(
                new ComparisonEntity
                {
                    Id = Mrn,
                    Created = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Updated = new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                    Comparisons =
                    [
                        new Comparison(
                            new DateTime(2025, 4, 23, 8, 30, 0, DateTimeKind.Utc),
                            "<xml alvs=\"true\"/>",
                            "<xml btms=\"true\"/>",
                            false,
                            ["Reason 1", "Reason 2"]
                        ),
                    ],
                }
            );

        var response = await client.GetAsync(Testing.Endpoints.Comparisons.Get(Mrn));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await VerifyJson(await response.Content.ReadAsStringAsync())
            .UseStrictJson()
            .DontIgnoreEmptyCollections()
            .DontScrubDateTimes();
    }
}
