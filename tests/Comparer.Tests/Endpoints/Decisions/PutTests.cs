using System.Net;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Endpoints.Decisions;

public class PutTests(ComparerWebApplicationFactory factory, ITestOutputHelper outputHelper)
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

    [Fact]
    public async Task PutAlvs_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.PutAsync(Testing.Endpoints.Decisions.Alvs.Put(Mrn), new StringContent("<xml />"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutAlvs_WhenReadOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.ReadOnly);

        var response = await client.PutAsync(Testing.Endpoints.Decisions.Alvs.Put(Mrn), new StringContent("<xml />"));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutAlvs_WhenValid_ShouldBeRequestBodyAsResponse()
    {
        var client = CreateClient();

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        await Verify(content);
    }

    [Fact]
    public async Task PutAlvs_WhenValid_ShouldTriggerAComparison()
    {
        var client = CreateClient();

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await MockComparisonManager.Received().CreateUpdateComparisonEntity(Mrn, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PutAlvs_WhenConcurrencyException_ShouldBeConflict()
    {
        var client = CreateClient();
        MockDecisionService
            .AppendAlvsDecision(Mrn, Arg.Any<Decision>(), Arg.Any<CancellationToken>())
            .Throws(new ConcurrencyException(Mrn, "etag"));

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(Mrn),
            new StringContent("<xml alvs=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PutBtms_WhenUnauthorized_ShouldBeUnauthorized()
    {
        var client = CreateClient(addDefaultAuthorizationHeader: false);

        var response = await client.PutAsync(Testing.Endpoints.Decisions.Btms.Put(Mrn), new StringContent("<xml />"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutBtms_WhenReadOnly_ShouldBeForbidden()
    {
        var client = CreateClient(testUser: TestUser.ReadOnly);

        var response = await client.PutAsync(Testing.Endpoints.Decisions.Btms.Put(Mrn), new StringContent("<xml />"));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutBtms_WhenValid_ShouldBeRequestBodyAsResponse()
    {
        var client = CreateClient();

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(Mrn),
            new StringContent("<xml btms=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        await Verify(content);
    }

    [Fact]
    public async Task PutBtms_WhenConcurrencyException_ShouldBeConflict()
    {
        var client = CreateClient();
        MockDecisionService
            .AppendBtmsDecision(Mrn, Arg.Any<Decision>(), Arg.Any<CancellationToken>())
            .Throws(new ConcurrencyException(Mrn, "etag"));

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(Mrn),
            new StringContent("<xml btms=\"true\" />")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
