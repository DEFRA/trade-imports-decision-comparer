using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;
using static System.Int32;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Header(
    [property: JsonPropertyName("decisionNumber")] int? DecisionNumber,
    [property: JsonPropertyName("entryVersionNumber")] int? EntryVersionNumber
)
{
    public static Header FromXml(string? xml)
    {
        if (xml == null)
        {
            return new Header(null, null);
        }

        using var reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));
        reader.ReadToFollowing(ElementNames.Header.LocalName, ElementNames.DecisionNotification.NamespaceName);

        var xmlElement = XElement
            .Load(reader.ReadSubtree())
            .Elements()
            .ToDictionary(x => x.Name.LocalName, x => x.Value);

        var decisionNumber = TryParse(
            xmlElement.GetValueOrDefault(nameof(ElementNames.DecisionNumber), ""),
            out var decisionNumberParsed
        )
            ? decisionNumberParsed
            : (int?)null;

        var entryVersionNumber = TryParse(
            xmlElement.GetValueOrDefault(nameof(ElementNames.EntryVersionNumber), ""),
            out var entryVersionNumbeParsed
        )
            ? entryVersionNumbeParsed
            : (int?)null;

        return new Header(decisionNumber, entryVersionNumber);
    }
}
