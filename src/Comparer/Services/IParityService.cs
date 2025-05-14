using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Projections;
using Polly;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IParityService
{
    Task<ParityProjection> Get(DateTime? start, DateTime? end, CancellationToken cancellationToken);
}
