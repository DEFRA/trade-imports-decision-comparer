using System.Linq.Expressions;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Entities;

namespace Defra.TradeImportsDecisionComparer.Comparer.Data;

public interface IMongoCollectionSet<T> : IQueryable<T>
    where T : IDataEntity
{
    int PendingChanges { get; }
    Task<T?> Find(string id, CancellationToken cancellationToken = default);
    Task<T?> Find(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default);

    Task Insert(T item, CancellationToken cancellationToken = default);

    Task Update(T item, CancellationToken cancellationToken = default);

    Task Update(List<T> items, CancellationToken cancellationToken = default);

    Task Update(T item, string etag, CancellationToken cancellationToken = default);

    Task PersistAsync(CancellationToken cancellationToken);
}
