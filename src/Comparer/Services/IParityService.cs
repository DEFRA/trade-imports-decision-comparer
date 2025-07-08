using Defra.TradeImportsDecisionComparer.Comparer.Projections;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IParityService
{
    Task<ParityProjection> Get(DateTime? start, DateTime? end, CancellationToken cancellationToken);
}
