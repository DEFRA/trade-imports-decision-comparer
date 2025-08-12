using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record ServiceHeader([property: JsonPropertyName("serviceCallTimestamp")] DateTime? ServiceCallTimestamp)
{
    private static readonly ServiceHeader s_emptyServiceHeader = new(null as DateTime?);

    public static ServiceHeader FromXml(string? xml)
    {
        if (xml == null)
        {
            return s_emptyServiceHeader;
        }

        using var reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));
        reader.ReadToFollowing(
            ElementNames.ServiceCallTimestamp.LocalName,
            ElementNames.DecisionNotification.NamespaceName
        );

        if (reader.NodeType != XmlNodeType.Element)
        {
            return s_emptyServiceHeader;
        }

        var potentialValue = reader.ReadElementContentAsString();

        return long.TryParse(potentialValue, out var unixTimestamp)
            ? new ServiceHeader(DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime)
            : new ServiceHeader(XmlConvert.ToDateTime(potentialValue, XmlDateTimeSerializationMode.RoundtripKind));
    }
}
