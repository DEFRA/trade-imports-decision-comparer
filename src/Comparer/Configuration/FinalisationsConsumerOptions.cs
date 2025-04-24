using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsDecisionComparer.Comparer.Configuration;

public class FinalisationsConsumerOptions
{
    public const string SectionName = "FinalisationsConsumer";

    [Required]
    public required string QueueName { get; init; }

    // This is only turned off in appsettings.IntegrationTests.json currently
    // as we don't want the consumers running for in memory integration tests.
    public required bool Enabled { get; init; } = true;
}
