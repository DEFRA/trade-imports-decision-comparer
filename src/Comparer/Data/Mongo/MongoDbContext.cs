using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using MongoDB.Driver;

namespace Defra.TradeImportsDecisionComparer.Comparer.Data.Mongo;

[ExcludeFromCodeCoverage]
public class MongoDbContext : IDbContext
{
    public MongoDbContext(IMongoDatabase database)
    {
        Database = database;
        AlvsDecisions = new MongoCollectionSet<AlvsDecisionEntity>(this);
        BtmsDecisions = new MongoCollectionSet<BtmsDecisionEntity>(this);
        Comparisons = new MongoCollectionSet<ComparisonEntity>(this);
        AlvsOutboundErrors = new MongoCollectionSet<AlvsOutboundErrorEntity>(this);
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

    public IMongoCollectionSet<AlvsDecisionEntity> AlvsDecisions { get; }
    public IMongoCollectionSet<BtmsDecisionEntity> BtmsDecisions { get; }
    public IMongoCollectionSet<ComparisonEntity> Comparisons { get; }
    public IMongoCollectionSet<AlvsOutboundErrorEntity> AlvsOutboundErrors { get; }

    public async Task SaveChangesAsync(CancellationToken cancellation = default)
    {
        if (GetChangedRecordsCount() == 0)
        {
            return;
        }

        if (GetChangedRecordsCount() == 1)
        {
            await InternalSaveChangesAsync(cancellation);
            return;
        }

        using var transaction = await StartTransaction(cancellation);
        try
        {
            await InternalSaveChangesAsync(cancellation);
            await transaction.CommitTransaction(cancellation);
        }
        catch (Exception)
        {
            await transaction.RollbackTransaction(cancellation);
            throw;
        }
    }

    private int GetChangedRecordsCount()
    {
        // This logic needs to be reviewed as it's easy to forget to include any new collection sets
        return AlvsDecisions.PendingChanges + BtmsDecisions.PendingChanges + Comparisons.PendingChanges;
    }

    private async Task InternalSaveChangesAsync(CancellationToken cancellation = default)
    {
        // This logic needs to be reviewed as it's easy to forget to include any new collection sets
        await AlvsDecisions.PersistAsync(cancellation);
        await BtmsDecisions.PersistAsync(cancellation);
        await Comparisons.PersistAsync(cancellation);
    }
}
