namespace Defra.TradeImportsDecisionComparer.Comparer.Data;

public interface IDbTransaction : IDisposable
{
    Task Commit(CancellationToken cancellationToken);
}
