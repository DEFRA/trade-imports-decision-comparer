using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IDecisionService
{
    Task<AlvsDecisionEntity> AppendAlvsDecision(string mrn, Decision decision, CancellationToken cancellationToken);
    Task<BtmsDecisionEntity> AppendBtmsDecision(string mrn, Decision decision, CancellationToken cancellationToken);
}
