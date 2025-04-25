using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Data;

public interface IDbContext
{
    IMongoCollectionSet<AlvsDecisionEntity> AlvsDecisions { get; }
    IMongoCollectionSet<BtmsDecisionEntity> BtmsDecisions { get; }
    IMongoCollectionSet<ComparisonEntity> Comparisons { get; }
    Task SaveChangesAsync(CancellationToken cancellation = default);
}
