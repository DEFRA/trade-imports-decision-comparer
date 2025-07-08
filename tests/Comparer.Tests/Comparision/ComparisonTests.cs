using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Comparision;

public class ComparisonTests
{
    private const string SampleDecision =
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<NS1:Envelope xmlns:NS1=\"http://www.w3.org/2003/05/soap-envelope\">\n  <NS1:Header>\n    <NS2:Security xmlns:NS2=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" NS1:role=\"system\">\n      <NS2:UsernameToken>\n        <NS2:Username>ibmtest</NS2:Username>\n        <NS2:Password>password</NS2:Password>\n      </NS2:UsernameToken>\n    </NS2:Security>\n  </NS1:Header>\n  <NS1:Body>\n    <NS3:DecisionNotification xmlns:NS3=\"http://uk.gov.hmrc.ITSW2.ws\">&lt;NS2:DecisionNotification xmlns:NS2=&quot;http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification&quot;&gt;&lt;NS2:ServiceHeader&gt;&lt;NS2:SourceSystem&gt;ALVS&lt;/NS2:SourceSystem&gt;&lt;NS2:DestinationSystem&gt;CDS&lt;/NS2:DestinationSystem&gt;&lt;NS2:CorrelationId&gt;000&lt;/NS2:CorrelationId&gt;&lt;NS2:ServiceCallTimestamp&gt;2025-05-29T18:57:29.298&lt;/NS2:ServiceCallTimestamp&gt;&lt;/NS2:ServiceHeader&gt;&lt;NS2:Header&gt;&lt;NS2:EntryReference&gt;25GB1HG99NHUJO3999&lt;/NS2:EntryReference&gt;&lt;NS2:EntryVersionNumber&gt;3&lt;/NS2:EntryVersionNumber&gt;&lt;NS2:DecisionNumber&gt;3&lt;/NS2:DecisionNumber&gt;&lt;/NS2:Header&gt;&lt;NS2:Item&gt;&lt;NS2:ItemNumber&gt;1&lt;/NS2:ItemNumber&gt;&lt;NS2:Check&gt;&lt;NS2:CheckCode&gt;H219&lt;/NS2:CheckCode&gt;&lt;NS2:DecisionCode&gt;H02&lt;/NS2:DecisionCode&gt;&lt;/NS2:Check&gt;&lt;/NS2:Item&gt;&lt;/NS2:DecisionNotification&gt;</NS3:DecisionNotification>\n  </NS1:Body>\n</NS1:Envelope>";

    [Fact]
    public void WhenDecisionAreTheSameThenShouldReturnExactMatch()
    {
        var result = Comparison.Create(
            SampleDecision,
            SampleDecision,
            new Finalisation
            {
                FinalState = "3",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.Match.Should().Be(ComparisionOutcome.ExactMatch);
    }

    [Fact]
    public void WhenNoBtmsDecisionThenShouldReturnNoBtmsDecision()
    {
        var result = Comparison.Create(
            SampleDecision,
            null,
            new Finalisation
            {
                FinalState = "3",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.Match.Should().Be(ComparisionOutcome.NoBtmsDecision);
    }

    [Fact]
    public void WhenNoAlvsDecisionThenShouldReturnNoAlvsDecision()
    {
        var result = Comparison.Create(
            null,
            SampleDecision,
            new Finalisation
            {
                FinalState = "3",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.Match.Should().Be(ComparisionOutcome.NoAlvsDecision);
    }

    [Fact]
    public void WhenMrnIsCancelledThenShouldReturnCancelledMrn()
    {
        var result = Comparison.Create(
            SampleDecision,
            SampleDecision,
            new Finalisation
            {
                FinalState = "1",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.Match.Should().Be(ComparisionOutcome.CancelledMrn);
    }

    [Fact]
    public void WhenDecisionNumberMatches_AndDecisionIsExactMatch_DecisionNumberMatchedShouldBeExactMatch()
    {
        var result = Comparison.Create(
            SampleDecision,
            SampleDecision,
            new Finalisation
            {
                FinalState = "3",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.Match.Should().Be(ComparisionOutcome.ExactMatch);
        result.DecisionNumberMatched.Should().Be(DecisionNumberMatch.ExactMatch);
    }

    [Fact]
    public void WhenDecisionNumberMatches_AndDecisionIsGroupMatch_DecisionNumberMatchedShouldBeExactMatch()
    {
        var btmsDecision = SampleDecision.Replace("NS2:DecisionCode&gt;H02&lt", "NS2:DecisionCode&gt;H03&lt");

        var result = Comparison.Create(
            SampleDecision,
            btmsDecision,
            new Finalisation
            {
                FinalState = "3",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.Match.Should().Be(ComparisionOutcome.GroupMatch);
        result.DecisionNumberMatched.Should().Be(DecisionNumberMatch.ExactMatch);
    }

    [Fact]
    public void WhenDecisionNumberDoesNotMatch_DecisionNumberMatchedShouldBeMismatch()
    {
        var btmsDecision = SampleDecision.Replace("NS2:DecisionNumber&gt;3&lt", "NS2:DecisionNumber&gt;2&lt");

        var result = Comparison.Create(
            SampleDecision,
            btmsDecision,
            new Finalisation
            {
                FinalState = "3",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.DecisionNumberMatched.Should().Be(DecisionNumberMatch.Mismatch);
    }

    [Fact]
    public void WhenDecisionsDoNotMatch_ButTheDecisionNumberMatches_DecisionNumberMatchedShouldBeNull()
    {
        var btmsDecision = SampleDecision.Replace("&lt;NS2:CheckCode&gt;H219&lt", "&lt;NS2:CheckCode&gt;H220&lt");

        var result = Comparison.Create(
            SampleDecision,
            btmsDecision,
            new Finalisation
            {
                FinalState = "3",
                ExternalVersion = 1,
                IsManualRelease = false,
            }
        );

        result.DecisionNumberMatched.Should().BeNull();
    }
}
