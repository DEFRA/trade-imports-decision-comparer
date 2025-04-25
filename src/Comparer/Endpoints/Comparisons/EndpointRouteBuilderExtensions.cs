using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defra.TradeImportsDecisionComparer.Comparer.Endpoints.Comparisons;

public static class EndpointRouteBuilderExtensions
{
    public static void MapComparisonEndpoints(this IEndpointRouteBuilder app, bool isDevelopment)
    {
        var route = app.MapGet("comparisons/{mrn}/", Get).RequireAuthorization(PolicyNames.Read);
        AllowAnonymousForDevelopment(isDevelopment, route);
    }

    [ExcludeFromCodeCoverage]
    private static void AllowAnonymousForDevelopment(bool isDevelopment, RouteHandlerBuilder route)
    {
        if (isDevelopment)
            route.AllowAnonymous();
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
