using System.Text.Json.Serialization;
using Defra.TradeImportsDecisionComparer.Comparer.Authentication;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Endpoints.Decisions;
using Defra.TradeImportsDecisionComparer.Comparer.Endpoints.OutboundErrors;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Health;
using Defra.TradeImportsDecisionComparer.Comparer.Metrics;
using Defra.TradeImportsDecisionComparer.Comparer.Services;
using Defra.TradeImportsDecisionComparer.Comparer.Utils;
using Defra.TradeImportsDecisionComparer.Comparer.Utils.Logging;
using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console(new EcsTextFormatter()).CreateBootstrapLogger();

try
{
    var app = CreateWebApplication(args);
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}

return;

static WebApplication CreateWebApplication(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureWebApplication(builder, args);

    return BuildWebApplication(builder);
}

static void ConfigureWebApplication(WebApplicationBuilder builder, string[] args)
{
    var integrationTest = args.Contains("--integrationTest=true");

    builder.Configuration.AddJsonFile(
        $"appsettings.cdp.{Environment.GetEnvironmentVariable("ENVIRONMENT")?.ToLower()}.json",
        integrationTest
    );
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    // Must happen before Mongo and Http client connections
    builder.Services.AddCustomTrustStore();

    builder.ConfigureLoggingAndTracing(integrationTest);

    builder.Services.ConfigureHttpClientDefaults(options =>
    {
        var resilienceOptions = new HttpStandardResilienceOptions { Retry = { UseJitter = true } };
        resilienceOptions.Retry.DisableForUnsafeHttpMethods();

        options.ConfigureHttpClient(c =>
        {
            // Disable the HttpClient timeout to allow the resilient pipeline below
            // to handle all timeouts
            c.Timeout = Timeout.InfiniteTimeSpan;
        });

        options.AddResilienceHandler(
            "All",
            builder =>
            {
                builder
                    .AddTimeout(resilienceOptions.TotalRequestTimeout)
                    .AddRetry(resilienceOptions.Retry)
                    .AddTimeout(resilienceOptions.AttemptTimeout);
            }
        );
    });
    builder.Services.AddOptions<BtmsOptions>().BindConfiguration("Btms").ValidateOptions();
    builder.Services.AddProblemDetails();
    builder.Services.AddHealthChecks();
    builder.Services.AddHealth(builder.Configuration);
    builder.Services.AddHttpClient();
    builder.Services.AddDbContext(builder.Configuration, integrationTest);
    builder.Services.AddAuthenticationAuthorization();
    builder.Services.AddConsumers(builder.Configuration);

    builder.Services.AddTransient<IParityService, ParityService>();
    builder.Services.AddTransient<IOutboundErrorService, OutboundErrorService>();
    builder.Services.AddTransient<IDecisionService, DecisionService>();
    builder.Services.AddTransient<IComparisonService, ComparisonService>();
    builder.Services.AddTransient<IComparisonManager, ComparisonManager>();

    builder.Services.AddTransient<MetricsMiddleware>();
    builder.Services.AddSingleton<RequestMetrics>();
}

static WebApplication BuildWebApplication(WebApplicationBuilder builder)
{
    var app = builder.Build();

    app.UseEmfExporter();
    app.UseHeaderPropagation();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<MetricsMiddleware>();
    app.MapHealth();
    app.MapDecisionEndpoints();
    app.MapOutboundErrorsEndpoints();
    app.UseStatusCodePages();
    app.UseExceptionHandler(
        new ExceptionHandlerOptions
        {
            AllowStatusCode404Response = true,
            ExceptionHandler = async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var error = exceptionHandlerFeature?.Error;
                string? detail = null;

                if (error is BadHttpRequestException badHttpRequestException)
                {
                    context.Response.StatusCode = badHttpRequestException.StatusCode;
                    detail = badHttpRequestException.Message;
                }

                var btmsOptions = context.RequestServices.GetRequiredService<IOptions<BtmsOptions>>().Value;
                if (btmsOptions.ConnectedSilentRunning && exceptionHandlerFeature is not null && error is not null)
                {
                    var httpMethodMetadata =
                        exceptionHandlerFeature.Endpoint?.Metadata.GetMetadata<HttpMethodMetadata>();
                    if (httpMethodMetadata != null && httpMethodMetadata.HttpMethods.Contains("PUT"))
                    {
                        var logger = context
                            .RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("ExceptionHandler");

                        logger.LogWarning(
                            error,
                            "Exception from endpoint {Endpoint} during connected silent running",
                            exceptionHandlerFeature.Endpoint?.DisplayName
                        );

                        context.Response.StatusCode = StatusCodes.Status200OK;
                    }
                }

                await context
                    .RequestServices.GetRequiredService<IProblemDetailsService>()
                    .WriteAsync(
                        new ProblemDetailsContext
                        {
                            HttpContext = context,
                            AdditionalMetadata = exceptionHandlerFeature?.Endpoint?.Metadata,
                            ProblemDetails = { Status = context.Response.StatusCode, Detail = detail },
                        }
                    );
            },
        }
    );

    return app;
}

#pragma warning disable S2094
namespace Defra.TradeImportsDecisionComparer.Comparer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program;
}
#pragma warning restore S2094
