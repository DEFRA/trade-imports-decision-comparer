using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defra.TradeImportsDecisionComparer.Comparer.Endpoints.Parity;

public static class EndpointRouteBuilderExtensions
{
    public static void MapParityEndpoints(this IEndpointRouteBuilder app, bool isDevelopment)
    {
        var route = app.MapGet("parity", Get).RequireAuthorization(PolicyNames.Read);
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
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromServices] IParityService parityService,
        CancellationToken cancellationToken
    )
    {
        var comparison = await parityService.Get(start, end, cancellationToken);

        return Results.Ok(comparison);
    }
}
