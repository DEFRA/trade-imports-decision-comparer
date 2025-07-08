using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defra.TradeImportsDecisionComparer.Comparer.Endpoints.OutboundErrors;

public static class EndpointRouteBuilderExtensions
{
    public static void MapOutboundErrorsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("alvs-outbound-errors/{mrn}/", PutAlvs).RequireAuthorization(PolicyNames.Write);
        app.MapPut("btms-outbound-errors/{mrn}/", PutBtms).RequireAuthorization(PolicyNames.Write);
        app.MapGet("outbound-errors/{mrn}/", Get).RequireAuthorization(PolicyNames.Read);
        app.MapGet("outbound-errors/{mrn}/comparison", GetComparison).RequireAuthorization(PolicyNames.Read);
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

    [HttpGet]
    private static async Task<IResult> GetComparison(
        [FromRoute] string mrn,
        [FromServices] IComparisonService comparisonService,
        CancellationToken cancellationToken
    )
    {
        var comparison = await comparisonService.GetOutboundError(mrn, cancellationToken);

        return Results.Ok(comparison);
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

        try
        {
            await save(new OutboundError(DateTime.UtcNow, xml), cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Results.Conflict();
        }

        return Results.Ok();
    }
}
