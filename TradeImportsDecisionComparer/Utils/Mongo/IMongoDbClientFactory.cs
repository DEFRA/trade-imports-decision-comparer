using MongoDB.Driver;

namespace TradeImportsDecisionComparer.Utils.Mongo;

public interface IMongoDbClientFactory
{
    IMongoClient GetClient();

    IMongoCollection<T> GetCollection<T>(string collection);
}