using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests;

[Trait("Category", "IntegrationTest")]
[Collection("Integration Tests")]
public abstract class IntegrationTestBase
{
    protected static HttpClient CreateClient() => new() { BaseAddress = new Uri("http://localhost:8080") };

    protected static IMongoDatabase CreateMongoDatabase() =>
        new MongoClient("mongodb://127.0.0.1:27017").GetDatabase("trade-imports-decision-comparer");

    protected static void ClearDatabase()
    {
        var db = CreateMongoDatabase();
        db.GetCollection<BsonDocument>(nameof(ComparisonEntity)).DeleteMany(FilterDefinition<BsonDocument>.Empty);
        db.GetCollection<BsonDocument>(nameof(BtmsOutboundErrorEntity))
            .DeleteMany(FilterDefinition<BsonDocument>.Empty);
        db.GetCollection<BsonDocument>(nameof(AlvsOutboundErrorEntity))
            .DeleteMany(FilterDefinition<BsonDocument>.Empty);
        db.GetCollection<BsonDocument>(nameof(BtmsDecisionEntity)).DeleteMany(FilterDefinition<BsonDocument>.Empty);
        db.GetCollection<BsonDocument>(nameof(AlvsDecisionEntity)).DeleteMany(FilterDefinition<BsonDocument>.Empty);
    }
}
