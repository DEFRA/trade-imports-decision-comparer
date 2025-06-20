using System.Xml.Linq;

namespace Defra.TradeImportsDecisionComparer.Comparer.Extensions;

public static class ElementNames
{
    public static readonly string Namespace = "http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification";
    public static readonly XName DecisionNotification = XName.Get(nameof(DecisionNotification), Namespace);
    public static readonly XName Item = XName.Get(nameof(Item), Namespace);
    public static readonly XName Check = XName.Get(nameof(Check), Namespace);
    public static readonly XName ItemNumber = XName.Get(nameof(ItemNumber), Namespace);
    public static readonly XName Header = XName.Get(nameof(Header), Namespace);
    public static readonly XName DecisionNumber = XName.Get(nameof(DecisionNumber), Namespace);
    public static readonly XName DecisionCode = XName.Get(nameof(DecisionCode), Namespace);
    public static readonly XName CheckCode = XName.Get(nameof(CheckCode), Namespace);
    public static readonly XName DecisionValidUntil = XName.Get(nameof(DecisionValidUntil), Namespace);
}
