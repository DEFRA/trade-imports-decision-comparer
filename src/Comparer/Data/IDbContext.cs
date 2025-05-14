using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Data;

public interface IDbContext
{
    IMongoCollectionSet<AlvsDecisionEntity> AlvsDecisions { get; }
    IMongoCollectionSet<BtmsDecisionEntity> BtmsDecisions { get; }
    IMongoCollectionSet<ComparisonEntity> Comparisons { get; }
    IMongoCollectionSet<AlvsOutboundErrorEntity> AlvsOutboundErrors { get; }
    IMongoCollectionSet<BtmsOutboundErrorEntity> BtmsOutboundErrors { get; }
    Task SaveChangesAsync(CancellationToken cancellation = default);
}
