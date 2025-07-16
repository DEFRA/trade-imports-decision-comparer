using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IComparisonManager
{
    public Task<ComparisonEntity> CompareLatestDecisions(
        string mrn,
        Finalisation? finalisation,
        CancellationToken cancellationToken
    );

    public Task CompareLatestOutboundErrors(string mrn, CancellationToken cancellationToken);
}
