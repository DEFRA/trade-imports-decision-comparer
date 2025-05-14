using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IOutboundErrorService
{
    Task<AlvsOutboundErrorEntity> AppendAlvsOutboundError(
        string mrn,
        OutboundError outboundError,
        CancellationToken cancellationToken
    );

    Task<BtmsOutboundErrorEntity> AppendBtmsOutboundError(
        string mrn,
        OutboundError outboundError,
        CancellationToken cancellationToken
    );

    Task<AlvsOutboundErrorEntity?> GetAlvsOutboundError(string mrn, CancellationToken cancellationToken);

    Task<BtmsOutboundErrorEntity?> GetBtmsOutboundError(string mrn, CancellationToken cancellationToken);
}
