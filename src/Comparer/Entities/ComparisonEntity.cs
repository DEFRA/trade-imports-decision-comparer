using Defra.TradeImportsDecisionComparer.Comparer.Data.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Entities;

public class ComparisonEntity : IDataEntity
{
    public required string Id { get; set; }

    public string ETag { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public required List<Comparison> Comparisons { get; set; } = [];

    public void OnSave() { }
}
