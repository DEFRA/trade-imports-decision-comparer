namespace Defra.TradeImportsDecisionComparer.Comparer.Data.Extensions;

public static class StringExtensions
{
    public static string ToHtmlDecodedXml(this string xml)
    {
        // Applies the decoding of some specific HTML encoded characters that we get from ALVS
        return xml.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"");
    }
}