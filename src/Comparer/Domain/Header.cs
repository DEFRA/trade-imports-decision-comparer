using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Header([property: JsonPropertyName("decisionNumber")] int? DecisionNumber)
{
    public static Header FromXml(string? xml)
    {
        if (xml == null)
        {
            return new Header(null as int?);
        }

        using var reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));
        reader.ReadToFollowing(ElementNames.DecisionNumber.LocalName, ElementNames.DecisionNotification.NamespaceName);

        return new Header(reader.ReadElementContentAsInt());
    }
}
