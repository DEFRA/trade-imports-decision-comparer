using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defra.TradeImportsDecisionComparer.Comparer.Endpoints.Comparisons;

public static class EndpointRouteBuilderExtensions
{
    public static void MapComparisonEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("comparisons/{mrn}/", Get).RequireAuthorization(PolicyNames.Read);
    }

    [HttpGet]
    private static async Task<IResult> Get(
        [FromRoute] string mrn,
        [FromServices] IComparisonService comparisonService,
        CancellationToken cancellationToken
    )
    {
        var comparison = await comparisonService.Get(mrn, cancellationToken);

        return Results.Ok(comparison);
    }
}
