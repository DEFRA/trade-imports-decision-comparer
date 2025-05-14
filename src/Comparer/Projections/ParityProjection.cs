using Defra.TradeImportsDecisionComparer.Comparer.Comparision;

namespace Defra.TradeImportsDecisionComparer.Comparer.Projections;

public record ParityProjection(Dictionary<string, int> Stats, List<string> MisMatchMrns);
