using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;

namespace Defra.TradeImportsDecisionComparer.Comparer.Data.Mongo;

[ExcludeFromCodeCoverage]
public class MongoDbContext : IDbContext
{
    public MongoDbContext(IMongoDatabase database)
    {
        Database = database;
    }

    internal IMongoDatabase Database { get; }
    internal MongoDbTransaction? ActiveTransaction { get; private set; }

    public async Task<IMongoDbTransaction> StartTransaction(CancellationToken cancellationToken = default)
    {
        var session = await Database.Client.StartSessionAsync(cancellationToken: cancellationToken);
        session.StartTransaction();
        ActiveTransaction = new MongoDbTransaction(session);
        return ActiveTransaction;
    }

    public async Task SaveChangesAsync(CancellationToken cancellation = default)
    {
        if (GetChangedRecordsCount() == 0)
        {
            return;
        }

        if (GetChangedRecordsCount() == 1)
        {
            await InternalSaveChangesAsync();
            return;
        }

        using var transaction = await StartTransaction(cancellation);
        try
        {
            await InternalSaveChangesAsync();
            await transaction.CommitTransaction(cancellation);
        }
        catch (Exception)
        {
            await transaction.RollbackTransaction(cancellation);
            throw;
        }
    }

    private const int ChangedRecordsCount = 0;

    private static int GetChangedRecordsCount()
    {
        // This logic needs to be reviewed as it's easy to forget to include any new collection sets
        return ChangedRecordsCount;
    }

    private static Task InternalSaveChangesAsync()
    {
        // This logic needs to be reviewed as it's easy to forget to include any new collection sets
        return Task.CompletedTask;
    }
}
