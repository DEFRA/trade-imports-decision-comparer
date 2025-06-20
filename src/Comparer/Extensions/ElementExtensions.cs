using System.Xml;
using System.Xml.Linq;
using Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;

namespace Defra.TradeImportsDecisionComparer.Comparer.Extensions;

public static class ElementExtensions
{
    public static string? GetElementStringValue(this XElement element, XName name)
    {
        var ele = element.Element(name);

        if (ele is null)
        {
            return null;
        }

        return ele.Value;
    }

    public static int GetElementIntValue(this XElement element, XName name)
    {
        var ele = element.Element(name);

        if (ele is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return int.Parse(ele.Value);
    }

    public static int GetDecisionNumber(this string xml)
    {
        using XmlReader reader = XmlReader.Create(new StringReader(xml.ToHtmlDecodedXml()));
        reader.ReadToFollowing(
            ElementNames.DecisionNotification.LocalName,
            ElementNames.DecisionNotification.NamespaceName
        );

        XElement element = XElement.Load(reader.ReadSubtree());
        var decisionNumberElement = element
            .Descendants(ElementNames.Header)
            .Descendants(ElementNames.DecisionNumber)
            .FirstOrDefault();
        if (decisionNumberElement?.Value != null)
            return Int32.Parse(decisionNumberElement.Value);

        return 0;
    }
}
