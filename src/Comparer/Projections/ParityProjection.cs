namespace Defra.TradeImportsDecisionComparer.Comparer.Projections;

public record ParityProjection(
    Dictionary<string, int> ParityStats,
    Dictionary<string, int> DecisionNumberStats,
    List<string> MisMatchMrns,
    List<string> MisMatchDecisionNumberMrns
);
