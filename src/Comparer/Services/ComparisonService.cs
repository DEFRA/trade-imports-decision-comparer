using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public class ComparisonService(IDbContext dbContext) : IComparisonService
{
    public Task<ComparisonEntity?> Get(string mrn, CancellationToken cancellationToken) =>
        dbContext.Comparisons.Find(mrn, cancellationToken);

    public async Task Save(ComparisonEntity comparison, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(comparison.ETag))
            await dbContext.Comparisons.Insert(comparison, cancellationToken);
        else
            await dbContext.Comparisons.Update(comparison, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
