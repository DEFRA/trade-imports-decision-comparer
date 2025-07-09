using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Error(
    [property: JsonPropertyName("errorCode")] string? ErrorCode,
    [property: JsonPropertyName("errorMessage")] string? ErrorMessage
)
{
    public static List<Error> Empty => [];

    public static List<Error> FromXml(string? xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            return Empty;
        }

        using var reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));
        reader.ReadToFollowing(
            ErrorElementNames.HMRCErrorNotification.LocalName,
            ErrorElementNames.HMRCErrorNotification.NamespaceName
        );

        if (reader.NodeType != XmlNodeType.Element)
        {
            return Empty;
        }

        var element = XElement.Load(reader.ReadSubtree());

        return (
            from error in element.Descendants(ErrorElementNames.Error)
            select new Error(
                error.GetElementStringValue(ErrorElementNames.ErrorCode),
                error.GetElementStringValue(ErrorElementNames.ErrorMessage)
            )
        ).ToList();
    }
}
