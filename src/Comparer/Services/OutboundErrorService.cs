using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

[ExcludeFromCodeCoverage] // see integration tests
public class OutboundErrorService(IDbContext dbContext) : IOutboundErrorService
{
    public async Task<AlvsOutboundErrorEntity> AppendAlvsOutboundError(
        string mrn,
        OutboundError outboundError,
        CancellationToken cancellationToken
    )
    {
        var entity = await dbContext.AlvsOutboundErrors.Find(mrn, cancellationToken);
        if (entity == null)
        {
            entity = new AlvsOutboundErrorEntity { Id = mrn, Errors = [outboundError] };
            await dbContext.AlvsOutboundErrors.Insert(entity, cancellationToken);
        }
        else
        {
            entity.Errors.Add(outboundError);
            await dbContext.AlvsOutboundErrors.Update(entity, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<BtmsOutboundErrorEntity> AppendBtmsOutboundError(
        string mrn,
        OutboundError outboundError,
        CancellationToken cancellationToken
    )
    {
        var entity = await dbContext.BtmsOutboundErrors.Find(mrn, cancellationToken);
        if (entity == null)
        {
            entity = new BtmsOutboundErrorEntity { Id = mrn, Errors = [outboundError] };
            await dbContext.BtmsOutboundErrors.Insert(entity, cancellationToken);
        }
        else
        {
            entity.Errors.Add(outboundError);
            await dbContext.BtmsOutboundErrors.Update(entity, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public Task<AlvsOutboundErrorEntity?> GetAlvsOutboundError(string mrn, CancellationToken cancellationToken) =>
        dbContext.AlvsOutboundErrors.Find(mrn, cancellationToken);

    public Task<BtmsOutboundErrorEntity?> GetBtmsOutboundError(string mrn, CancellationToken cancellationToken) =>
        dbContext.BtmsOutboundErrors.Find(mrn, cancellationToken);
}
