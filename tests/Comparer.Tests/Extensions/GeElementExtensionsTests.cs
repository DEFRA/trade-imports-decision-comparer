namespace Defra.TradeImportsDecisionComparer.Comparer.Extensions;

public class GeElementExtensionsTests
{
    [Fact]
    public void Get_DecisionNumber()
    {
        string xml =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope\r\n\txmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">\r\n\t\r\n\t<soap:Header>\r\n\t\t\r\n\t\t<oas:Security soap:role=\"system\" soap:mustUnderstand=\"true\"\r\n\t\t              xmlns:oas=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">\r\n\t\t\t\r\n\t\t\t<oas:UsernameToken>\r\n\t\t\t\t\r\n\t\t\t\t<oas:Username>ibmtest</oas:Username>\r\n\t\t\t\t<oas:Password>password</oas:Password>\r\n\t\t\t</oas:UsernameToken>\r\n\t\t</oas:Security>\r\n\t</soap:Header>\r\n\t<soap:Body>\r\n\t\t\r\n\t\t<DecisionNotification\r\n\t\t\txmlns=\"http://uk.gov.hmrc.ITSW2.ws\">\r\n\t\t\t\r\n\t\t\t<DecisionNotification\r\n\t\t\t\txmlns=\"http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification\">\r\n\t\t\t\t\r\n\t\t\t\t<ServiceHeader>\r\n\t\t\t\t\t\r\n\t\t\t\t\t<SourceSystem>ALVS</SourceSystem>\r\n\t\t\t\t\t<DestinationSystem>CDS</DestinationSystem>\r\n\t\t\t\t\t<CorrelationId>000</CorrelationId>\r\n\t\t\t\t\t<ServiceCallTimestamp>1746477693229</ServiceCallTimestamp>\r\n\t\t\t\t</ServiceHeader>\r\n\t\t\t\t<Header>\r\n\t\t\t\t\t\r\n\t\t\t\t\t<EntryReference>25GB4TOTEN4GR4IAR1</EntryReference>\r\n\t\t\t\t\t<EntryVersionNumber>1</EntryVersionNumber>\r\n\t\t\t\t\t<DecisionNumber>4</DecisionNumber>\r\n\t\t\t\t</Header>\r\n\t\t\t\t<Item>\r\n\t\t\t\t\t\r\n\t\t\t\t\t<ItemNumber>1</ItemNumber>\r\n\t\t\t\t\t<Check>\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t<CheckCode>H219</CheckCode>\r\n\t\t\t\t\t\t<DecisionCode>C03</DecisionCode>\r\n\t\t\t\t\t</Check>\r\n\t\t\t\t</Item>\r\n\t\t\t\t<Item>\r\n\t\t\t\t\t\r\n\t\t\t\t\t<ItemNumber>2</ItemNumber>\r\n\t\t\t\t\t<Check>\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t<CheckCode>H219</CheckCode>\r\n\t\t\t\t\t\t<DecisionCode>C03</DecisionCode>\r\n\t\t\t\t\t</Check>\r\n\t\t\t\t</Item>\r\n\t\t\t</DecisionNotification>\r\n\t\t</DecisionNotification>\r\n\t</soap:Body>\r\n</soap:Envelope>";

        var decisionNumber = xml.GetDecisionNumber();

        decisionNumber.Should().Be(4);
    }
}
