using RestEase;
using WireMock.Client;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests;

public class WireMockClient
{
    public WireMockClient()
    {
        WireMockAdminApi.ResetMappingsAsync().GetAwaiter().GetResult();
        WireMockAdminApi.ResetRequestsAsync().GetAwaiter().GetResult();
    }

    public IWireMockAdminApi WireMockAdminApi { get; } = RestClient.For<IWireMockAdminApi>("http://localhost:9090");
}

[CollectionDefinition(nameof(WireMockClientCollection))]
public class WireMockClientCollection : ICollectionFixture<WireMockClient>;
