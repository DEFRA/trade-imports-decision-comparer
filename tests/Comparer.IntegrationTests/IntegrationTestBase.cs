namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests;

[Trait("Category", "IntegrationTest")]
[Collection("Integration Tests")]
public abstract class IntegrationTestBase
{
    protected static HttpClient CreateClient() => new() { BaseAddress = new Uri("http://localhost:8080") };
}
