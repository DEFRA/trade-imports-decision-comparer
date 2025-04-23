using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

[ExcludeFromCodeCoverage] // see integration tests
public class DecisionService(IDbContext dbContext) : IDecisionService
{
    public async Task<AlvsDecisionEntity> AppendAlvsDecision(
        string mrn,
        Decision decision,
        CancellationToken cancellationToken
    )
    {
        var entity = await dbContext.AlvsDecisions.Find(mrn, cancellationToken);
        if (entity == null)
        {
            entity = new AlvsDecisionEntity { Id = mrn, Decisions = [decision] };
            await dbContext.AlvsDecisions.Insert(entity, cancellationToken);
        }
        else
        {
            entity.Decisions.Add(decision);
            await dbContext.AlvsDecisions.Update(entity, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<BtmsDecisionEntity> AppendBtmsDecision(
        string mrn,
        Decision decision,
        CancellationToken cancellationToken
    )
    {
        var entity = await dbContext.BtmsDecisions.Find(mrn, cancellationToken);
        if (entity == null)
        {
            entity = new BtmsDecisionEntity { Id = mrn, Decisions = [decision] };
            await dbContext.BtmsDecisions.Insert(entity, cancellationToken);
        }
        else
        {
            entity.Decisions.Add(decision);
            await dbContext.BtmsDecisions.Update(entity, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<AlvsDecisionEntity?> GetAlvsDecision(string mrn, CancellationToken cancellationToken) =>
        await dbContext.AlvsDecisions.Find(mrn, cancellationToken);

    public async Task<BtmsDecisionEntity?> GetBtmsDecision(string mrn, CancellationToken cancellationToken) =>
        await dbContext.BtmsDecisions.Find(mrn, cancellationToken);
}
