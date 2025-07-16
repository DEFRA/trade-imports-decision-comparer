using System.Diagnostics.CodeAnalysis;
using System.Text;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Data;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defra.TradeImportsDecisionComparer.Comparer.Endpoints.Decisions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapDecisionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("alvs-decisions/{mrn}/", PutAlvs).RequireAuthorization(PolicyNames.Write);
        app.MapPut("btms-decisions/{mrn}/", PutBtms).RequireAuthorization(PolicyNames.Write);
        app.MapGet("decisions/{mrn}/", Get).RequireAuthorization(PolicyNames.Read);
        app.MapGet("decisions/parity", GetParity).RequireAuthorization(PolicyNames.Read);
        app.MapGet("decisions/{mrn}/comparison", GetComparison).RequireAuthorization(PolicyNames.Read);
    }

    [HttpPut]
    private static async Task<IResult> PutAlvs(
        [FromRoute] string mrn,
        HttpContext context,
        [FromServices] IDecisionService decisionService,
        [FromServices] IComparisonManager comparisonManager,
        [FromServices] IOperatingModeStrategy operatingModeStrategy,
        CancellationToken cancellationToken
    ) =>
        await ReadAndSave(
            context,
            async (decision, ct) =>
            {
                await decisionService.AppendAlvsDecision(mrn, decision, ct);

                var comparison = await comparisonManager.CompareLatestDecisions(mrn, null, ct);

                return operatingModeStrategy.DetermineDecision(comparison, decision);
            },
            cancellationToken
        );

    [HttpPut]
    private static async Task<IResult> PutBtms(
        [FromRoute] string mrn,
        HttpContext context,
        [FromServices] IDecisionService decisionService,
        CancellationToken cancellationToken
    ) =>
        await ReadAndSave(
            context,
            async (decision, ct) =>
            {
                await decisionService.AppendBtmsDecision(mrn, decision, ct);

                return decision.Xml;
            },
            cancellationToken
        );

    [HttpGet]
    private static async Task<IResult> Get(
        [FromRoute] string mrn,
        [FromServices] IDecisionService decisionService,
        CancellationToken cancellationToken
    )
    {
        var alvsDecision = await decisionService.GetAlvsDecision(mrn, cancellationToken);
        var btmsDecision = await decisionService.GetBtmsDecision(mrn, cancellationToken);

        return Results.Ok(new { alvsDecision, btmsDecision });
    }

    [HttpGet]
    private static async Task<IResult> GetParity(
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromServices] IParityService parityService,
        [FromQuery] bool? isFinalisation = true,
        CancellationToken cancellationToken = default
    )
    {
        var parity = await parityService.Get(start, end, isFinalisation != false, cancellationToken);

        return Results.Ok(parity);
    }

    [HttpGet]
    private static async Task<IResult> GetComparison(
        [FromRoute] string mrn,
        [FromServices] IComparisonService comparisonService,
        CancellationToken cancellationToken
    )
    {
        var comparison = await comparisonService.Get(mrn, cancellationToken);

        return Results.Ok(comparison);
    }

    [SuppressMessage("SonarLint", "S5131", Justification = "This service cannot be compromised by a malicious user")]
    private static async Task<IResult> ReadAndSave(
        HttpContext context,
        Func<Decision, CancellationToken, Task<string?>> save,
        CancellationToken cancellationToken
    )
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var incomingDecision = await reader.ReadToEndAsync(cancellationToken);
        string? outgoingDecision;

        try
        {
            outgoingDecision = await save(new Decision(DateTime.UtcNow, incomingDecision), cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Results.Conflict();
        }

        return Results.Content(outgoingDecision ?? incomingDecision, "application/xml", Encoding.UTF8);
    }
}
