using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

public interface IComparisonManager
{
    public Task CreateUpdateComparisonEntity(
        string mrn,
        Finalisation? finalisation,
        CancellationToken cancellationToken
    );
}
