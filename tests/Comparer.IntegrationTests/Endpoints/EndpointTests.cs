using FluentAssertions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Endpoints;

public class EndpointTests : IntegrationTestBase
{
    [Fact]
    public void True() => true.Should().BeTrue();
}
