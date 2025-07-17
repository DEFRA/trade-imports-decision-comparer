using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Defra.TradeImportsDecisionComparer.Testing.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.Decisions.PutTests;

public class TrialCutoverTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    private const string Mrn = "mrn";
    private IDecisionService MockDecisionService { get; } = Substitute.For<IDecisionService>();
    private IComparisonManager MockComparisonManager { get; } = Substitute.For<IComparisonManager>();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddTransient<IDecisionService>(_ => MockDecisionService);
        services.AddTransient<IComparisonManager>(_ => MockComparisonManager);
    }

    protected override void ConfigureHostConfiguration(IConfigurationBuilder config)
    {
        base.ConfigureHostConfiguration(config);

        config.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{BtmsOptions.SectionName}:{nameof(BtmsOptions.OperatingMode)}"] = (
                    (int)OperatingMode.TrialCutover
                ).ToString(),
                [$"{BtmsOptions.SectionName}:{nameof(BtmsOptions.DecisionSamplingPercentage)}"] = "100",
            }
        );
    }

    [Fact]
    public async Task PutAlvs_WhenComparisonManagerThrows_AndTrialCutover_ShouldBeError()
    {
        var client = CreateClient();
        MockComparisonManager
            .CompareLatestDecisions(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Throws(new Exception("Unhandled"));

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task PutAlvs_WhenValidComparison_ShouldBeBtmsDecision()
    {
        var client = CreateClient();
        MockComparisonManager
            .CompareLatestDecisions(Mrn, null, Arg.Any<CancellationToken>())
            .Returns(new ComparisonEntity { Id = Mrn, Latest = ComparisonFixtures.MatchComparison() });

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        await Verify(content);
    }

    [Fact]
    public async Task PutBtms_WhenDecisionServiceThrows_AndTrialCutover_ShouldBeError()
    {
        var client = CreateClient();
        MockDecisionService
            .AppendBtmsDecision(Arg.Any<string>(), Arg.Any<Decision>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Unhandled"));

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(Mrn),
            new StringContent("<xml btms=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
