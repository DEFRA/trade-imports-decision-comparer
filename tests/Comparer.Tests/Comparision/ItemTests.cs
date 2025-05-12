using Defra.TradeImportsDecisionComparer.Comparer.Domain;

namespace Defra.TradeImportsDecisionComparer.Comparer.Tests.Comparision;

public class ItemTests
{
    private const string sampleDecision =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">\r\n    <soap:Header>\r\n       <oas:Security soap:role=\"system\" soap:mustUnderstand=\"true\" xmlns:oas=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">\r\n            <oas:UsernameToken>\r\n                <oas:Username>ibmtest</oas:Username>\r\n                <oas:Password>password</oas:Password>\r\n            </oas:UsernameToken>\r\n        </oas:Security>\r\n    </soap:Header>\r\n    <soap:Body>\r\n        <DecisionNotification xmlns=\"http://uk.gov.hmrc.ITSW2.ws\">\r\n            <DecisionNotification xmlns=\"http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification\">\r\n                <ServiceHeader>\r\n                    <SourceSystem>ALVS</SourceSystem>\r\n                    <DestinationSystem>CDS</DestinationSystem>\r\n                    <CorrelationId>000</CorrelationId>\r\n                    <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>\r\n                </ServiceHeader>\r\n                <Header>\r\n                    <EntryReference>23GB1234567890ABC8</EntryReference>\r\n                    <EntryVersionNumber>1</EntryVersionNumber>\r\n                    <DecisionNumber>1</DecisionNumber>\r\n                </Header>\r\n                <Item>\r\n                    <ItemNumber>1</ItemNumber>\r\n                    <Check>\r\n                        <CheckCode>H218</CheckCode>\r\n                        <DecisionCode>C02</DecisionCode>\r\n                        <DecisionValidUntil>202307042359</DecisionValidUntil>\r\n                    </Check>                   \r\n                </Item>               \r\n            </DecisionNotification>\r\n        </DecisionNotification>\r\n    </soap:Body>\r\n</soap:Envelope>";

    [Fact]
    public void WhenXmlIsNull_ReturnEmptyList()
    {
        var result = Item.FromXml(null);

        result.Count.Should().Be(0);
    }

    [Fact]
    public void WhenXmlIsHasValue_ReturnItems()
    {
        var result = Item.FromXml(sampleDecision);

        result.Count.Should().Be(1);
        result[0].ItemNumber.Should().Be(1);
        result[0].Checks.Count.Should().Be(1);
        result[0].Checks[0].CheckCode.Should().Be("H218");
        result[0].Checks[0].DecisionCode.Should().Be("C02");
    }
}
