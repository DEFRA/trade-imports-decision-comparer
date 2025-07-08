using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Comparision;

public enum OutboundErrorComparisonOutcome
{
    LegacyAlvsErrorCode = 1,
}

public record OutboundErrorComparisonOutcomeEvaluatorContext(List<Error> AlvsErrors);

public static class OutboundErrorComparisionOutcomeEvaluator
{
    private static readonly HashSet<string> s_legacyAlvsErrorCodes =
    [
        "ALVSVAL103",
        "ALVSVAL104",
        "ALVSVAL106",
        "ALVSVAL107",
        "ALVSVAL110",
        "ALVSVAL111",
        "ALVSVAL112",
        "ALVSVAL115",
        "ALVSVAL157",
        "ALVSVAL158",
        "ALVSVAL159",
        "ALVSVAL160",
        "ALVSVAL161",
        "ALVSVAL162",
        "ALVSVAL163",
        "ALVSVAL301",
        "ALVSVAL302",
        "ALVSVAL304",
        "ALVSVAL306",
        "ALVSVAL309",
        "ALVSVAL312",
        "ALVSVAL315",
        "ALVSVAL316",
        "ALVSVAL325",
        "ALVSVAL327",
    ];

    public static OutboundErrorComparisonOutcome GetComparisionOutcome(
        this OutboundErrorComparisonOutcomeEvaluatorContext context
    )
    {
        if (context.AlvsErrors.Any(x => x.ErrorCode is not null && s_legacyAlvsErrorCodes.Contains(x.ErrorCode)))
        {
            return OutboundErrorComparisonOutcome.LegacyAlvsErrorCode;
        }

        return OutboundErrorComparisonOutcome.LegacyAlvsErrorCode;
    }
}
