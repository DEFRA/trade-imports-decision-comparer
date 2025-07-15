using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.OutboundErrors.PutTests;

public class DefaultOperatingModeTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    private const string Mrn = "mrn";
    private IOutboundErrorService MockOutboundErrorService { get; } = Substitute.For<IOutboundErrorService>();
    private IComparisonManager MockComparisonManager { get; } = Substitute.For<IComparisonManager>();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddTransient<IOutboundErrorService>(_ => MockOutboundErrorService);
        services.AddTransient<IComparisonManager>(_ => MockComparisonManager);
    }

    protected override void ConfigureHostConfiguration(IConfigurationBuilder config)
    {
        base.ConfigureHostConfiguration(config);

        config.AddInMemoryCollection(
            new Dictionary<string, string?> { ["Btms:OperatingMode"] = ((int)OperatingMode.Default).ToString() }
        );
    }

    [Fact]
    public async Task PutAlvs_WhenComparisonManagerThrows_AndDefaultOperatingMode_ShouldBeError()
    {
        var client = CreateClient();
        MockComparisonManager
            .CompareLatestOutboundErrors(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Unhandled"));

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Alvs.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task PutBtms_WhenComparisonManagerThrows_AndDefaultOperatingMode_ShouldBeError()
    {
        var client = CreateClient();
        MockComparisonManager
            .CompareLatestOutboundErrors(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Throws(new Exception("Unhandled"));

        var response = await client.PutAsync(
            Testing.Endpoints.OutboundErrors.Btms.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
