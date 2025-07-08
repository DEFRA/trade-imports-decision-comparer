using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

[ExcludeFromCodeCoverage] // see integration tests
public class ComparisonService(IDbContext dbContext) : IComparisonService
{
    public Task<ComparisonEntity?> Get(string mrn, CancellationToken cancellationToken) =>
        dbContext.Comparisons.Find(mrn, cancellationToken);

    public Task<OutboundErrorComparisonEntity?> GetOutboundError(string mrn, CancellationToken cancellationToken) =>
        dbContext.OutboundErrorComparisons.Find(mrn, cancellationToken);

    public async Task Save(ComparisonEntity comparison, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(comparison.ETag))
            await dbContext.Comparisons.Insert(comparison, cancellationToken);
        else
            await dbContext.Comparisons.Update(comparison, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Save(OutboundErrorComparisonEntity comparison, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(comparison.ETag))
            await dbContext.OutboundErrorComparisons.Insert(comparison, cancellationToken);
        else
            await dbContext.OutboundErrorComparisons.Update(comparison, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
