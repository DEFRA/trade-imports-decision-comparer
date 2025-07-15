using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

[ExcludeFromCodeCoverage] // see integration tests
public class ComparisonManager(
    IDecisionService decisionService,
    IComparisonService comparisonService,
    IOutboundErrorService outboundErrorService
) : IComparisonManager
{
    public async Task<ComparisonEntity> CompareLatestDecisions(
        string mrn,
        Finalisation? finalisation,
        CancellationToken cancellationToken
    )
    {
        var alvsDecision = await decisionService.GetAlvsDecision(mrn, cancellationToken);
        var btmsDecision = await decisionService.GetBtmsDecision(mrn, cancellationToken);
        var latestAlvs = alvsDecision?.Decisions.OrderBy(x => x.Xml.GetDecisionNumber()).LastOrDefault();
        var latestBtms = btmsDecision?.Decisions.OrderBy(x => x.Xml.GetDecisionNumber()).LastOrDefault();

        var comparison = Comparison.Create(latestAlvs?.Xml, latestBtms?.Xml, finalisation);
        var comparisonEntity = await comparisonService.Get(mrn, cancellationToken);

        if (comparisonEntity is null)
        {
            comparisonEntity = new ComparisonEntity { Id = mrn, Latest = comparison };
        }
        else
        {
            comparisonEntity.History.Add(comparisonEntity.Latest);
            comparisonEntity.Latest = comparison;
        }

        return await comparisonService.Save(comparisonEntity, cancellationToken);
    }

    public async Task CompareLatestOutboundErrors(string mrn, CancellationToken cancellationToken)
    {
        var alvsOutboundError = await outboundErrorService.GetAlvsOutboundError(mrn, cancellationToken);
        var btmsOutboundError = await outboundErrorService.GetBtmsOutboundError(mrn, cancellationToken);
        var latestAlvs = alvsOutboundError?.Errors.OrderBy(x => x.Xml.GetErrorEntryVersionNumber()).LastOrDefault();
        var latestBtms = btmsOutboundError?.Errors.OrderBy(x => x.Xml.GetErrorEntryVersionNumber()).LastOrDefault();

        var comparison = OutboundErrorComparison.Create(latestAlvs?.Xml, latestBtms?.Xml);
        var comparisonEntity = await comparisonService.GetOutboundError(mrn, cancellationToken);

        if (comparisonEntity is null)
        {
            comparisonEntity = new OutboundErrorComparisonEntity { Id = mrn, Latest = comparison };
        }
        else
        {
            comparisonEntity.History.Add(comparisonEntity.Latest);
            comparisonEntity.Latest = comparison;
        }

        await comparisonService.Save(comparisonEntity, cancellationToken);
    }
}
