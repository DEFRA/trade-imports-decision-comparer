using System.Diagnostics.CodeAnalysis;
using System.Text;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
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

    [HttpGet]
    private static async Task<IResult> Get(
        [FromRoute] string mrn,
        [FromServices] IOutboundErrorService outboundErrorService,
        CancellationToken cancellationToken
    )
    {
        var alvsOutboundError = await outboundErrorService.GetAlvsOutboundError(mrn, cancellationToken);

        return Results.Ok(new { alvsOutboundError });
    }

    [SuppressMessage("SonarLint", "S5131", Justification = "This service cannot be compromised by a malicious user")]
    private static async Task<IResult> ReadAndSave(
        HttpContext context,
        Func<OutboundError, CancellationToken, Task> save,
        CancellationToken cancellationToken
    )
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var xml = await reader.ReadToEndAsync(cancellationToken);

        await save(new OutboundError(DateTime.UtcNow, xml), cancellationToken);

        return Results.Content(xml, "application/xml", Encoding.UTF8);
    }
}
