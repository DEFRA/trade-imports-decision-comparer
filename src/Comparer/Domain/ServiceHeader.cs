using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record ServiceHeader([property: JsonPropertyName("serviceCallTimestamp")] DateTime? ServiceCallTimestamp)
{
    public static ServiceHeader FromXml(string? xml)
    {
        if (xml == null)
        {
            return new ServiceHeader(null as DateTime?);
        }

        using var reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));
        reader.ReadToFollowing(
            ElementNames.ServiceCallTimestamp.LocalName,
            ElementNames.DecisionNotification.NamespaceName
        );

        return new ServiceHeader(reader.ReadElementContentAsDateTime());
    }
}
