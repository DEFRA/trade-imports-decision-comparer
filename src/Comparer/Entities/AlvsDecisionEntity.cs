using System.Text.Json.Serialization;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Entities;

public class AlvsDecisionEntity : IDataEntity
{
    public required string Id { get; set; }

    public string ETag { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public required List<Decision> Decisions { get; set; } = [];

    public void OnSave() { }
}
