using System.Diagnostics.CodeAnalysis;
using System.Text;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defra.TradeImportsDecisionComparer.Comparer.Endpoints.Decisions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapDecisionEndpoints(this IEndpointRouteBuilder app, bool isDevelopment)
    {
        var route = app.MapPut("alvs-decisions/{mrn}/", PutAlvs).RequireAuthorization(PolicyNames.Write);
        AllowAnonymousForDevelopment(isDevelopment, route);

        route = app.MapPut("btms-decisions/{mrn}/", PutBtms).RequireAuthorization(PolicyNames.Write);
        AllowAnonymousForDevelopment(isDevelopment, route);

        route = app.MapGet("decisions/{mrn}/", Get).RequireAuthorization(PolicyNames.Read);
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
        [FromServices] IDecisionService decisionService,
        CancellationToken cancellationToken
    ) => await ReadAndSave(context, (d, ct) => decisionService.AppendAlvsDecision(mrn, d, ct), cancellationToken);

    [HttpPut]
    private static async Task<IResult> PutBtms(
        [FromRoute] string mrn,
        HttpContext context,
        [FromServices] IDecisionService decisionService,
        CancellationToken cancellationToken
    ) => await ReadAndSave(context, (d, ct) => decisionService.AppendBtmsDecision(mrn, d, ct), cancellationToken);

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

    [SuppressMessage("SonarLint", "S5131", Justification = "This service cannot be compromised by a malicious user")]
    private static async Task<IResult> ReadAndSave(
        HttpContext context,
        Func<Decision, CancellationToken, Task> save,
        CancellationToken cancellationToken
    )
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var xml = await reader.ReadToEndAsync(cancellationToken);

        await save(new Decision(DateTime.UtcNow, xml), cancellationToken);

        return Results.Content(xml, "application/xml", Encoding.UTF8);
    }
}
