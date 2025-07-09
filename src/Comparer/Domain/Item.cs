using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;
using Defra.TradeImportsDecisionComparer.Comparer.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Domain;

public record Item(
    [property: JsonPropertyName("itemNumber")] int ItemNumber,
    [property: JsonPropertyName("checks")] List<Check> Checks
)
{
    public static List<Item> FromXml(string? xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            return [];
        }

        using var reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));
        reader.ReadToFollowing(
            ElementNames.DecisionNotification.LocalName,
            ElementNames.DecisionNotification.NamespaceName
        );

        if (reader.NodeType != XmlNodeType.Element)
        {
            return [];
        }

        var element = XElement.Load(reader.ReadSubtree());
        return (
            from item in element.Descendants(ElementNames.Item)
            select new Item(
                item.GetElementIntValue(ElementNames.ItemNumber),
                (
                    from check in item.Descendants(ElementNames.Check)
                    select new Check(
                        check.GetElementStringValue(ElementNames.CheckCode)!,
                        check.GetElementStringValue(ElementNames.DecisionCode)!
                    )
                ).ToList()
            )
        ).ToList();
    }
}
