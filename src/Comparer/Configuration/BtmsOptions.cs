using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsDecisionComparer.Comparer.Configuration;

public class BtmsOptions
{
    public const string SectionName = "Btms";

    public OperatingMode OperatingMode { get; init; } = OperatingMode.ConnectedSilentRunning;

    [Range(0, 100)]
    public int DecisionSamplingPercentage { get; init; }
}
