using System.Xml.Linq;

namespace Defra.TradeImportsDecisionComparer.Comparer.Extensions;

public static class ErrorElementNames
{
    private const string Namespace = "http://www.hmrc.gov.uk/webservices/itsw/ws/hmrcerrornotification";

    // ReSharper disable once InconsistentNaming
    public static readonly XName HMRCErrorNotification = XName.Get(nameof(HMRCErrorNotification), Namespace);
    public static readonly XName Error = XName.Get(nameof(Error), Namespace);
    public static readonly XName ErrorCode = XName.Get(nameof(ErrorCode), Namespace);
    public static readonly XName ErrorMessage = XName.Get(nameof(ErrorMessage), Namespace);
}