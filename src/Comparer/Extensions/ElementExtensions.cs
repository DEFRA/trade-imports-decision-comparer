using System.Xml.Linq;

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
}
