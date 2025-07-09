using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using MongoDB.Driver;

namespace Defra.TradeImportsDecisionComparer.Comparer.Data.Mongo;

[ExcludeFromCodeCoverage]
public class MongoDbContext : IDbContext
{
    private readonly ILogger<MongoDbContext> _logger;

    public MongoDbContext(IMongoDatabase database, ILogger<MongoDbContext> logger)
    {
        _logger = logger;
        Database = database;
        AlvsDecisions = new MongoCollectionSet<AlvsDecisionEntity>(this);
        BtmsDecisions = new MongoCollectionSet<BtmsDecisionEntity>(this);
        Comparisons = new MongoCollectionSet<ComparisonEntity>(this);
        OutboundErrorComparisons = new MongoCollectionSet<OutboundErrorComparisonEntity>(this);
        AlvsOutboundErrors = new MongoCollectionSet<AlvsOutboundErrorEntity>(this);
        BtmsOutboundErrors = new MongoCollectionSet<BtmsOutboundErrorEntity>(this);
    }

    internal IMongoDatabase Database { get; }
    internal MongoDbTransaction? ActiveTransaction { get; private set; }

    public IMongoCollectionSet<AlvsDecisionEntity> AlvsDecisions { get; }
    public IMongoCollectionSet<BtmsDecisionEntity> BtmsDecisions { get; }
    public IMongoCollectionSet<ComparisonEntity> Comparisons { get; }
    public IMongoCollectionSet<OutboundErrorComparisonEntity> OutboundErrorComparisons { get; }
    public IMongoCollectionSet<AlvsOutboundErrorEntity> AlvsOutboundErrors { get; }
    public IMongoCollectionSet<BtmsOutboundErrorEntity> BtmsOutboundErrors { get; }

    private async Task<IDbTransaction> StartTransaction(CancellationToken cancellationToken)
    {
        var session = await Database.Client.StartSessionAsync(cancellationToken: cancellationToken);
        session.StartTransaction();

        ActiveTransaction = new MongoDbTransaction(session);

        return ActiveTransaction;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        switch (GetChangedRecordsCount())
        {
            case 0:
                return;
            case 1:
                await SaveChanges(cancellationToken);
                break;
            default:
                await SaveChangesWithinTransaction(cancellationToken);
                break;
        }
    }

    private async Task SaveChangesWithinTransaction(CancellationToken cancellationToken)
    {
        using var transaction = await StartTransaction(cancellationToken);

        await SaveChanges(cancellationToken);

        // If the transaction is not committed within the using scope,
        // then it will be rolled back automatically by the transaction
        await transaction.Commit(cancellationToken);
    }

    private int GetChangedRecordsCount()
    {
        // This logic needs to be reviewed as it's easy to forget to include any new collection sets
        return AlvsDecisions.PendingChanges
            + BtmsDecisions.PendingChanges
            + Comparisons.PendingChanges
            + OutboundErrorComparisons.PendingChanges
            + AlvsOutboundErrors.PendingChanges
            + BtmsOutboundErrors.PendingChanges;
    }

    private async Task SaveChanges(CancellationToken cancellationToken)
    {
        try
        {
            // This logic needs to be reviewed as it's easy to forget to include any new collection sets
            await AlvsDecisions.PersistAsync(cancellationToken);
            await BtmsDecisions.PersistAsync(cancellationToken);
            await Comparisons.PersistAsync(cancellationToken);
            await OutboundErrorComparisons.PersistAsync(cancellationToken);
            await AlvsOutboundErrors.PersistAsync(cancellationToken);
            await BtmsOutboundErrors.PersistAsync(cancellationToken);
        }
        catch (MongoCommandException mongoCommandException) when (mongoCommandException.Code == 112)
        {
            const string message = "Mongo write conflict - consumer will retry";
            _logger.LogWarning(mongoCommandException, message);

            // WriteConflict error: this operation conflicted with another operation. Please retry your operation or multi-document transaction
            // - retries are built into consumers of the comparer
            throw new ConcurrencyException(message, mongoCommandException);
        }
        catch (MongoWriteException mongoWriteException) when (mongoWriteException.WriteError.Code == 11000)
        {
            const string message = "Mongo write error - consumer will retry";
            _logger.LogWarning(mongoWriteException, message);

            // A write operation resulted in an error. WriteError: { Category : "DuplicateKey", Code : 11000 }
            // - retries are built into consumers of the comparer
            throw new ConcurrencyException(message, mongoWriteException);
        }
    }
}
