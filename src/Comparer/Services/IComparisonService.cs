using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IComparisonService
{
    Task<ComparisonEntity?> Get(string mrn, CancellationToken cancellationToken);
    Task<OutboundErrorComparisonEntity?> GetOutboundError(string mrn, CancellationToken cancellationToken);
    Task Save(ComparisonEntity comparison, CancellationToken cancellationToken);
    Task Save(OutboundErrorComparisonEntity comparison, CancellationToken cancellationToken);
}
