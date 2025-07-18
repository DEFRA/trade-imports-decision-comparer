namespace Defra.TradeImportsDecisionComparer.Comparer.Projections;

public record OutboundErrorParityProjection(
    Dictionary<string, int> Stats,
    List<string> NoAlvsErrorsMrns,
    List<string> NoBtmsErrorsMrns,
    List<string> AlvsOnlyMrns,
    List<string> BtmsOnlyMrns,
    List<string> MismatchMrns,
    List<string> HeaderMismatchMrns
);
