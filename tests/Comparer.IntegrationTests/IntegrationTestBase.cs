using System.Net.Http.Headers;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests;

[Trait("Category", "IntegrationTest")]
[Collection("Integration Tests")]
public abstract class IntegrationTestBase
{
    protected static HttpClient CreateClient(OperatingMode operatingMode = OperatingMode.ConnectedSilentRunning)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        var port = operatingMode switch
        {
            OperatingMode.ConnectedSilentRunning => 8080,
            OperatingMode.TrialCutover => 8081,
            _ => throw new ArgumentOutOfRangeException(nameof(operatingMode), operatingMode, null),
        };

        var httpClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{port}") };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            // See compose.yml for username, password and scope configuration
            Convert.ToBase64String("IntegrationTests:integration-tests-pwd"u8.ToArray())
        );

        return httpClient;
    }
}
