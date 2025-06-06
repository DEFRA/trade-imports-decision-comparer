using WireMock.Server;

namespace Defra.TradeImportsDecisionComparer.Testing;

public class WireMockTestBase<T> : IClassFixture<T>
    where T : WireMockContext, new()
{
    protected WireMockServer WireMock { get; }

    protected WireMockTestBase(WireMockContext context)
    {
        WireMock = context.Server;
        WireMock.Reset();
    }
}
