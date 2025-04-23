using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public class DecisionService : IDecisionService
{
    public Task<AlvsDecisionEntity> AppendAlvsDecision(
        string mrn,
        Decision decision,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(new AlvsDecisionEntity { Id = mrn, Decisions = [decision] });
    }

    public Task<BtmsDecisionEntity> AppendBtmsDecision(
        string mrn,
        Decision decision,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(new BtmsDecisionEntity { Id = mrn, Decisions = [decision] });
    }
}
