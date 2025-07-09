using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Comparision;

public class OutboundErrorComparisonTests
{
    private const string SampleOutboundError =
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?><NS1:Envelope xmlns:NS1=\"http://www.w3.org/2003/05/soap-envelope\"><NS1:Header><NS2:Security xmlns:NS2=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" NS1:role=\"system\"><NS2:UsernameToken><NS2:Username>username</NS2:Username><NS2:Password>password</NS2:Password></NS2:UsernameToken></NS2:Security></NS1:Header><NS1:Body><NS3:HMRCErrorNotification xmlns:NS3=\"http://uk.gov.hmrc.ITSW2.ws\">&lt;NS2:HMRCErrorNotification xmlns:NS2=&quot;http://www.hmrc.gov.uk/webservices/itsw/ws/hmrcerrornotification&quot;&gt;&lt;NS2:ServiceHeader&gt;&lt;NS2:SourceSystem&gt;ALVS&lt;/NS2:SourceSystem&gt;&lt;NS2:DestinationSystem&gt;CDS&lt;/NS2:DestinationSystem&gt;&lt;NS2:CorrelationId&gt;74227759&lt;/NS2:CorrelationId&gt;&lt;NS2:ServiceCallTimestamp&gt;2025-07-08T12:14:01.321&lt;/NS2:ServiceCallTimestamp&gt;&lt;/NS2:ServiceHeader&gt;&lt;NS2:Header&gt;&lt;NS2:SourceCorrelationId&gt;101&lt;/NS2:SourceCorrelationId&gt;&lt;NS2:EntryReference&gt;MRN&lt;/NS2:EntryReference&gt;&lt;NS2:EntryVersionNumber&gt;1&lt;/NS2:EntryVersionNumber&gt;&lt;/NS2:Header&gt;&lt;NS2:Error&gt;&lt;NS2:ErrorCode&gt;ALVSVALERRORCODE&lt;/NS2:ErrorCode&gt;&lt;NS2:ErrorMessage&gt;Error message&lt;/NS2:ErrorMessage&gt;&lt;/NS2:Error&gt;&lt;/NS2:HMRCErrorNotification&gt;</NS3:HMRCErrorNotification></NS1:Body></NS1:Envelope>";

    private const string LegacyErrorCode1 = "ALVSVAL103";
    private const string ActiveErrorCode1 = "ALVSVAL101";
    private const string ActiveErrorCode2 = "ALVSVAL102";

    [Theory]
    [InlineData("ALVSVAL103")]
    [InlineData("ALVSVAL104")]
    [InlineData("ALVSVAL106")]
    [InlineData("ALVSVAL107")]
    [InlineData("ALVSVAL110")]
    [InlineData("ALVSVAL111")]
    [InlineData("ALVSVAL112")]
    [InlineData("ALVSVAL115")]
    [InlineData("ALVSVAL157")]
    [InlineData("ALVSVAL158")]
    [InlineData("ALVSVAL159")]
    [InlineData("ALVSVAL160")]
    [InlineData("ALVSVAL161")]
    [InlineData("ALVSVAL162")]
    [InlineData("ALVSVAL163")]
    [InlineData("ALVSVAL301")]
    [InlineData("ALVSVAL302")]
    [InlineData("ALVSVAL304")]
    [InlineData("ALVSVAL306")]
    [InlineData("ALVSVAL309")]
    [InlineData("ALVSVAL312")]
    [InlineData("ALVSVAL315")]
    [InlineData("ALVSVAL316")]
    [InlineData("ALVSVAL325")]
    [InlineData("ALVSVAL327")]
    public void WhenLegacyErrorCode_ShouldBeLegacyAlvsErrorCode(string legacyErrorCode)
    {
        var alvsXml = SampleOutboundError.WithErrorCode(legacyErrorCode);
        var btmsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.LegacyAlvsErrorCode);
    }

    [Fact]
    public void WhenActiveErrorCode_ShouldBeMatch()
    {
        var alvsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);
        var btmsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.Match);
    }

    [Fact]
    public void WhenNoAlvsErrorCodes_ShouldBeNoAlvsErrors()
    {
        var comparison = OutboundErrorComparison.Create(null, null);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.NoAlvsErrors);
    }

    [Fact]
    public void WhenNoBtmsErrorCodes_ShouldBeNoBtmsErrors()
    {
        var comparison = OutboundErrorComparison.Create(SampleOutboundError, null);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.NoBtmsErrors);
    }

    [Fact]
    public void WhenBtmsErrorCodeIsDifferent_ShouldBeMismatch()
    {
        var alvsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);
        var btmsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode2).WithEntryReference("DIFFERENT");

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.Mismatch);
    }

    [Fact]
    public void WhenBtmsEntryReferenceIsDifferent_ShouldBeMismatch()
    {
        var alvsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);
        var btmsXml = SampleOutboundError.WithEntryReference("DIFFERENT");

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.Mismatch);
    }

    [Fact]
    public void WhenBtmsEntryVersionNumberIsDifferent_ShouldBeMismatch()
    {
        var alvsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);
        var btmsXml = SampleOutboundError.WithEntryVersionNumber("2");

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.Mismatch);
    }

    [Fact]
    public void WhenAlvsOnlyError_ShouldBeAlvsOnlyError()
    {
        var alvsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);
        var btmsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode2);

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.AlvsOnlyError);
    }

    [Fact]
    public void WhenBtmsOnlyError_ShouldBeAlvsOnlyError()
    {
        var alvsXml = SampleOutboundError.WithErrorCode(ActiveErrorCode1);
        var btmsXml = SampleOutboundError.WithErrorCodes([ActiveErrorCode1, ActiveErrorCode2]);

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.BtmsOnlyError);
    }

    [Fact]
    public void WhenLegacyAlvsErrorCodeWithAValidOne_ShouldBeMatch()
    {
        var alvsXml = SampleOutboundError.WithErrorCodes([LegacyErrorCode1, ActiveErrorCode1]);
        var btmsXml = SampleOutboundError.WithErrorCodes([ActiveErrorCode1]);

        var comparison = OutboundErrorComparison.Create(alvsXml, btmsXml);

        comparison.Match.Should().Be(OutboundErrorComparisonOutcome.Match);
    }
}

public static class FixtureExtensions
{
    public static string WithErrorCode(this string xml, string errorCode)
    {
        return xml.Replace("ALVSVALERRORCODE", errorCode);
    }

    public static string WithErrorCodes(this string xml, string[] errorCodes)
    {
        var errors = errorCodes.Select(x =>
            $"&lt;NS2:Error&gt;&lt;NS2:ErrorCode&gt;{x}&lt;/NS2:ErrorCode&gt;&lt;NS2:ErrorMessage&gt;Error message&lt;/NS2:ErrorMessage&gt;&lt;/NS2:Error&gt;"
        );

        return xml.Replace(
            "&lt;NS2:Error&gt;&lt;NS2:ErrorCode&gt;ALVSVALERRORCODE&lt;/NS2:ErrorCode&gt;&lt;NS2:ErrorMessage&gt;Error message&lt;/NS2:ErrorMessage&gt;&lt;/NS2:Error&gt;",
            string.Join("", errors)
        );
    }

    public static string WithEntryReference(this string xml, string entryReference)
    {
        return xml.Replace(
            "&lt;NS2:EntryReference&gt;MRN&lt;/NS2:EntryReference&gt;",
            $"&lt;NS2:EntryReference&gt;{entryReference}&lt;/NS2:EntryReference&gt;"
        );
    }

    public static string WithEntryVersionNumber(this string xml, string entryVersionNumber)
    {
        return xml.Replace(
            "&lt;NS2:EntryVersionNumber&gt;1&lt;/NS2:EntryVersionNumber&gt;",
            $"&lt;NS2:EntryVersionNumber&gt;{entryVersionNumber}&lt;/NS2:EntryVersionNumber&gt;"
        );
    }
}
