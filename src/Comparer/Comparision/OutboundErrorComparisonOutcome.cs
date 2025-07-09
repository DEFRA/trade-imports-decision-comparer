using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Comparision;

public enum OutboundErrorComparisonOutcome
{
    LegacyAlvsErrorCode = 1,
    Match = 2,
    Mismatch = 3,
    NoAlvsErrors = 4,
    NoBtmsErrors = 5,
    AlvsOnlyError = 6,
    BtmsOnlyError = 7,
    HeaderMismatch = 9,
}

public record OutboundErrorComparisonOutcomeEvaluatorContext(
    ErrorHeader AlvsHeader,
    List<Error> AlvsErrors,
    ErrorHeader BtmsHeader,
    List<Error> BtmsErrors
);

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
        if (context.AlvsErrors.Count == 0)
        {
            return OutboundErrorComparisonOutcome.NoAlvsErrors;
        }

        if (context.BtmsErrors.Count == 0)
        {
            return OutboundErrorComparisonOutcome.NoBtmsErrors;
        }

        if (context.AlvsErrors.All(x => x.ErrorCode is not null && s_legacyAlvsErrorCodes.Contains(x.ErrorCode)))
        {
            return OutboundErrorComparisonOutcome.LegacyAlvsErrorCode;
        }

        if (
            !(
                context.AlvsHeader.EntryReference is not null
                && context.AlvsHeader.EntryReference.Equals(context.BtmsHeader.EntryReference)
                && context.AlvsHeader.EntryVersionNumber is not null
                && context.AlvsHeader.EntryVersionNumber.Equals(context.BtmsHeader.EntryVersionNumber)
            )
        )
        {
            return OutboundErrorComparisonOutcome.HeaderMismatch;
        }

        var nonLegacyAlvsErrorCodes = context
            .AlvsErrors.Select(x => x.ErrorCode)
            .Where(x => x is not null && !s_legacyAlvsErrorCodes.Contains(x))
            .ToList();

        if (nonLegacyAlvsErrorCodes.SequenceEqual(context.BtmsErrors.Select(x => x.ErrorCode)))
        {
            return OutboundErrorComparisonOutcome.Match;
        }

        if (
            nonLegacyAlvsErrorCodes.Any(e =>
                !context.BtmsErrors.Any(x => x.ErrorCode is not null && x.ErrorCode.Equals(e))
            )
        )
        {
            return OutboundErrorComparisonOutcome.AlvsOnlyError;
        }

        if (context.BtmsErrors.Any(e => !nonLegacyAlvsErrorCodes.Any(x => x is not null && x.Equals(e.ErrorCode))))
        {
            return OutboundErrorComparisonOutcome.BtmsOnlyError;
        }

        return OutboundErrorComparisonOutcome.Mismatch;
    }
}
