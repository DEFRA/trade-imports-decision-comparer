using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsDecisionComparer.Comparer.Data;

public class MongoDbOptions
{
    public const string SectionName = "Mongo";

    [Required]
    public string? DatabaseUri { get; set; }

    [Required]
    public string? DatabaseName { get; set; }
}
