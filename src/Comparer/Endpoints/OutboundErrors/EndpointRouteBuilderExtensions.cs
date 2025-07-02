using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defra.TradeImportsDecisionComparer.Comparer.Endpoints.OutboundErrors;

public static class EndpointRouteBuilderExtensions
{
    public static void MapOutboundErrorsEndpoints(this IEndpointRouteBuilder app, bool isDevelopment)
    {
        var route = app.MapPut("alvs-outbound-errors/{mrn}/", PutAlvs).RequireAuthorization(PolicyNames.Write);
        AllowAnonymousForDevelopment(isDevelopment, route);

        route = app.MapPut("btms-outbound-errors/{mrn}/", PutBtms).RequireAuthorization(PolicyNames.Write);
        AllowAnonymousForDevelopment(isDevelopment, route);

        route = app.MapGet("outbound-errors/{mrn}/", Get).RequireAuthorization(PolicyNames.Read);
        AllowAnonymousForDevelopment(isDevelopment, route);
    }

    [ExcludeFromCodeCoverage]
    private static void AllowAnonymousForDevelopment(bool isDevelopment, RouteHandlerBuilder route)
    {
        if (isDevelopment)
            route.AllowAnonymous();
    }

    [HttpPut]
    private static async Task<IResult> PutAlvs(
        [FromRoute] string mrn,
        HttpContext context,
        [FromServices] IOutboundErrorService outboundErrorService,
        CancellationToken cancellationToken
    ) =>
        await ReadAndSave(
            context,
            (d, ct) => outboundErrorService.AppendAlvsOutboundError(mrn, d, ct),
            cancellationToken
        );

    [HttpPut]
    private static async Task<IResult> PutBtms(
        [FromRoute] string mrn,
        HttpContext context,
        [FromServices] IOutboundErrorService outboundErrorService,
        CancellationToken cancellationToken
    ) =>
        await ReadAndSave(
            context,
            (d, ct) => outboundErrorService.AppendBtmsOutboundError(mrn, d, ct),
            cancellationToken
        );

    [HttpGet]
    private static async Task<IResult> Get(
        [FromRoute] string mrn,
        [FromServices] IOutboundErrorService outboundErrorService,
        CancellationToken cancellationToken
    )
    {
        var alvsOutboundError = await outboundErrorService.GetAlvsOutboundError(mrn, cancellationToken);
        var btmsOutboundError = await outboundErrorService.GetBtmsOutboundError(mrn, cancellationToken);

        return Results.Ok(new { alvsOutboundError, btmsOutboundError });
    }

    [SuppressMessage("SonarLint", "S5131", Justification = "This service cannot be compromised by a malicious user")]
    [SuppressMessage("SonarLint", "S1481", Justification = "Just testing for now")]
    [SuppressMessage("SonarLint", "S125", Justification = "Just testing for now")]
    private static async Task<IResult> ReadAndSave(
        HttpContext context,
        Func<OutboundError, CancellationToken, Task> save,
        CancellationToken cancellationToken
    )
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var xml = await reader.ReadToEndAsync(cancellationToken);

        try
        {
            // await save(new OutboundError(DateTime.UtcNow, xml), cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Results.Conflict();
        }

        return Results.Conflict();
    }
}
