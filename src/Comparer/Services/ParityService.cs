using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Projections;
using MongoDB.Driver.Linq;

namespace Defra.TradeImportsDecisionComparer.Comparer.Services;

[ExcludeFromCodeCoverage] // see integration tests
public class ParityService(IDbContext dbContext) : IParityService
{
    public async Task<ParityProjection> Get(
        DateTime? start,
        DateTime? end,
        bool isFinalisation,
        CancellationToken cancellationToken
    )
    {
        var query = from c in dbContext.Comparisons select c;

        query = isFinalisation switch
        {
            true => from c in query where c.Latest.IsFinalisation == true || c.Latest.IsFinalisation == null select c,
            false => from c in query where c.Latest.IsFinalisation == false select c,
        };

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

        var decisionNumberCountQuery =
            from c in query
            where c.Latest.DecisionNumberMatched != null
            group c by c.Latest.DecisionNumberMatched.ToString() into grp
            select new { grp.Key, Count = grp.Count() };

        var misMatchDecisionNumberQuery =
            from c in query
            where c.Latest.DecisionNumberMatched == DecisionNumberMatch.Mismatch
            select c.Id;

        var countQueryResults = await countQuery.ToListAsync(cancellationToken);
        var decisionNumberCountQueryResults = await decisionNumberCountQuery.ToListAsync(cancellationToken);

        var stats = new Dictionary<string, int>(
            countQueryResults.Select(x => new KeyValuePair<string, int>(x.Key, x.Count))
        );

        var decisionNumberStats = new Dictionary<string, int>(
            decisionNumberCountQueryResults.Select(x => new KeyValuePair<string, int>(x.Key, x.Count))
        );

        return new ParityProjection(
            stats,
            decisionNumberStats,
            await misMatchMrnQuery.ToListAsync(cancellationToken),
            await misMatchDecisionNumberQuery.ToListAsync(cancellationToken)
        );
    }

    public async Task<OutboundErrorParityProjection> GetOutboundError(
        DateTime? start,
        DateTime? end,
        CancellationToken cancellationToken
    )
    {
        var query = from c in dbContext.OutboundErrorComparisons select c;

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

        var alvsOnlyMrnQuery =
            from c in query
            where c.Latest.Match == OutboundErrorComparisonOutcome.AlvsOnlyError
            select c.Id;

        var btmsOnlyMrnQuery =
            from c in query
            where c.Latest.Match == OutboundErrorComparisonOutcome.BtmsOnlyError
            select c.Id;

        var mismatchMrnQuery =
            from c in query
            where c.Latest.Match == OutboundErrorComparisonOutcome.Mismatch
            select c.Id;

        var headerMismatchMrnQuery =
            from c in query
            where c.Latest.Match == OutboundErrorComparisonOutcome.HeaderMismatch
            select c.Id;

        return new OutboundErrorParityProjection(
            new Dictionary<string, int>(
                (await countQuery.ToListAsync(cancellationToken)).Select(x => new KeyValuePair<string, int>(
                    x.Key,
                    x.Count
                ))
            ),
            await alvsOnlyMrnQuery.ToListAsync(cancellationToken),
            await btmsOnlyMrnQuery.ToListAsync(cancellationToken),
            await mismatchMrnQuery.ToListAsync(cancellationToken),
            await headerMismatchMrnQuery.ToListAsync(cancellationToken)
        );
    }
}
