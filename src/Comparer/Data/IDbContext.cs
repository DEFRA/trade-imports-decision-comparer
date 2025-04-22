namespace Defra.TradeImportsDecisionComparer.Comparer.Data;

public interface IDbContext
{
    Task SaveChangesAsync(CancellationToken cancellation = default);
}
