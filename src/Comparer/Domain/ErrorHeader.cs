using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record ErrorHeader(
    [property: JsonPropertyName("entryReference")] string? EntryReference = null,
    [property: JsonPropertyName("entryVersionNumber")] int? EntryVersionNumber = null
)
{
    public static ErrorHeader Empty => new();

    public static ErrorHeader FromXml(string? xml)
    {
        if (xml == null)
        {
            return Empty;
        }

        using var reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));

        reader.ReadToFollowing(ErrorElementNames.Header.LocalName, ErrorElementNames.Header.NamespaceName);

        if (reader.NodeType != XmlNodeType.Element)
        {
            return Empty;
        }

        var header = XElement.Load(reader.ReadSubtree());

        var entryReference = header.GetElementStringValue(ErrorElementNames.EntryReference);
        var entryVersionNumber = header.GetElementIntValue(ErrorElementNames.EntryVersionNumber);

        return new ErrorHeader(entryReference, entryVersionNumber);
    }
}
