using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Projections;
using MongoDB.Driver.Linq;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

[ExcludeFromCodeCoverage] // see integration tests
public class ParityService(IDbContext dbContext) : IParityService
{
    public async Task<ParityProjection> Get(DateTime? start, DateTime? end, CancellationToken cancellationToken)
    {
        var query = from c in dbContext.Comparisons select c;

        if (start.HasValue)
        {
            query = from c in query where c.Updated >= start select c;
        }

        if (end.HasValue)
        {
            query = from c in query where c.Updated <= end select c;
        }

        var countQuery =
            from c in query
            group c by c.Latest.Match.ToString() into grp
            select new { grp.Key, Count = grp.Count() };

        var misMatchMrnQuery = from c in query where c.Latest.Match == ComparisionOutcome.Mismatch select c.Id;

        return new ParityProjection(
            new Dictionary<string, int>(
                (await countQuery.ToListAsync(cancellationToken)).Select(x => new KeyValuePair<string, int>(
                    x.Key,
                    x.Count
                ))
            ),
            await misMatchMrnQuery.ToListAsync(cancellationToken)
        );
    }
}
