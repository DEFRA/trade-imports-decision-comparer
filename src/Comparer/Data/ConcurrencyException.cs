namespace Defra.TradeImportsDecisionComparer.Comparer.Data;

public class ConcurrencyException(string entityId, string entityEtag)
    : Exception($"Failed up update {entityId} with etag {entityEtag}")
{
    public string EntityId { get; } = entityId;

    public string EntityEtag { get; } = entityEtag;
}
