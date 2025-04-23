using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsDecisionComparer.Comparer.Utils.Logging;

public class TraceHeader
{
    [ConfigurationKeyName("TraceHeader")]
    [Required]
    public required string Name { get; set; }
}
