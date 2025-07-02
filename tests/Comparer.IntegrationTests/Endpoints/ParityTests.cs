using System.Net;
using System.Text;
using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDecisionComparer.Comparer.Comparision;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using Defra.TradeImportsDecisionComparer.Comparer.Projections;
using FluentAssertions;
using MongoDB.Driver;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Endpoints;

public class ParityTests(ITestOutputHelper output) : SqsTestBase(output)
{
    private static readonly JsonSerializerOptions s_options = new() { PropertyNameCaseInsensitive = true };

    private const string decision1 =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">\r\n    <soap:Header>\r\n       <oas:Security soap:role=\"system\" soap:mustUnderstand=\"true\" xmlns:oas=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">\r\n            <oas:UsernameToken>\r\n                <oas:Username>ibmtest</oas:Username>\r\n                <oas:Password>password</oas:Password>\r\n            </oas:UsernameToken>\r\n        </oas:Security>\r\n    </soap:Header>\r\n    <soap:Body>\r\n        <DecisionNotification xmlns=\"http://uk.gov.hmrc.ITSW2.ws\">\r\n            <DecisionNotification xmlns=\"http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification\">\r\n                <ServiceHeader>\r\n                    <SourceSystem>ALVS</SourceSystem>\r\n                    <DestinationSystem>CDS</DestinationSystem>\r\n                    <CorrelationId>000</CorrelationId>\r\n                    <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>\r\n                </ServiceHeader>\r\n                <Header>\r\n                    <EntryReference>23GB1234567890ABC8</EntryReference>\r\n                    <EntryVersionNumber>1</EntryVersionNumber>\r\n                    <DecisionNumber>1</DecisionNumber>\r\n                </Header>\r\n                <Item>\r\n                    <ItemNumber>1</ItemNumber>\r\n                    <Check>\r\n                        <CheckCode>H218</CheckCode>\r\n                        <DecisionCode>X00</DecisionCode>\r\n                        <DecisionValidUntil>202307042359</DecisionValidUntil>\r\n                    </Check>                   \r\n                </Item>               \r\n            </DecisionNotification>\r\n        </DecisionNotification>\r\n    </soap:Body>\r\n</soap:Envelope>";

    private const string decision2 =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">\r\n    <soap:Header>\r\n       <oas:Security soap:role=\"system\" soap:mustUnderstand=\"true\" xmlns:oas=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">\r\n            <oas:UsernameToken>\r\n                <oas:Username>ibmtest</oas:Username>\r\n                <oas:Password>password</oas:Password>\r\n            </oas:UsernameToken>\r\n        </oas:Security>\r\n    </soap:Header>\r\n    <soap:Body>\r\n        <DecisionNotification xmlns=\"http://uk.gov.hmrc.ITSW2.ws\">\r\n            <DecisionNotification xmlns=\"http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification\">\r\n                <ServiceHeader>\r\n                    <SourceSystem>ALVS</SourceSystem>\r\n                    <DestinationSystem>CDS</DestinationSystem>\r\n                    <CorrelationId>000</CorrelationId>\r\n                    <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>\r\n                </ServiceHeader>\r\n                <Header>\r\n                    <EntryReference>23GB1234567890ABC8</EntryReference>\r\n                    <EntryVersionNumber>1</EntryVersionNumber>\r\n                    <DecisionNumber>1</DecisionNumber>\r\n                </Header>\r\n                <Item>\r\n                    <ItemNumber>1</ItemNumber>\r\n                    <Check>\r\n                        <CheckCode>H218</CheckCode>\r\n                        <DecisionCode>C02</DecisionCode>\r\n                        <DecisionValidUntil>202307042359</DecisionValidUntil>\r\n                    </Check>                   \r\n                </Item>               \r\n            </DecisionNotification>\r\n        </DecisionNotification>\r\n    </soap:Body>\r\n</soap:Envelope>";

    private const string decision3 =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">\r\n    <soap:Header>\r\n       <oas:Security soap:role=\"system\" soap:mustUnderstand=\"true\" xmlns:oas=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">\r\n            <oas:UsernameToken>\r\n                <oas:Username>ibmtest</oas:Username>\r\n                <oas:Password>password</oas:Password>\r\n            </oas:UsernameToken>\r\n        </oas:Security>\r\n    </soap:Header>\r\n    <soap:Body>\r\n        <DecisionNotification xmlns=\"http://uk.gov.hmrc.ITSW2.ws\">\r\n            <DecisionNotification xmlns=\"http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification\">\r\n                <ServiceHeader>\r\n                    <SourceSystem>ALVS</SourceSystem>\r\n                    <DestinationSystem>CDS</DestinationSystem>\r\n                    <CorrelationId>000</CorrelationId>\r\n                    <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>\r\n                </ServiceHeader>\r\n                <Header>\r\n                    <EntryReference>23GB1234567890ABC8</EntryReference>\r\n                    <EntryVersionNumber>1</EntryVersionNumber>\r\n                    <DecisionNumber>1</DecisionNumber>\r\n                </Header>\r\n                <Item>\r\n                    <ItemNumber>1</ItemNumber>\r\n                    <Check>\r\n                        <CheckCode>H218</CheckCode>\r\n                        <DecisionCode>C03</DecisionCode>\r\n                        <DecisionValidUntil>202307042359</DecisionValidUntil>\r\n                    </Check>                   \r\n                </Item>               \r\n            </DecisionNotification>\r\n        </DecisionNotification>\r\n    </soap:Body>\r\n</soap:Envelope>";

    [Fact]
    public async Task WhenNoComparisons_ShouldBeNullResults()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.GetAsync(Testing.Endpoints.OutboundErrors.Get(mrn));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<ParityProjection>(content, s_options)
            ?? throw new Exception("Failed to deserialize");
        result.MisMatchMrns.Should().BeNullOrEmpty();
        result.Stats.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task WhenParityExists_ShouldReturnResults()
    {
        await DrainAllMessages();
        var client = CreateClient();
        var start = DateTime.UtcNow;
        await InsertDecisionsForMrn("parity-mrn1", decision1, decision1);
        await InsertDecisionsForMrn("parity-mrn2", decision2, decision3);
        await InsertDecisionsForMrn("parity-mrn3", decision2, decision1);

        var response = await client.GetAsync(Testing.Endpoints.Parity.Get(start, null));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<ParityProjection>(content, s_options)
            ?? throw new Exception("Failed to deserialize");
        result.MisMatchMrns.Count.Should().Be(1);
        result.MisMatchMrns[0].Should().Be("parity-mrn3");
        result.Stats.Count.Should().Be(3);
        result.Stats["ExactMatch"].Should().Be(1);
        result.Stats["Mismatch"].Should().Be(1);
        result.Stats["GroupMatch"].Should().Be(1);
    }

    private async Task InsertDecisionsForMrn(string mrn, string alvsDecisionXml, string btmsDecisionXml)
    {
        var client = CreateClient();

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent(alvsDecisionXml, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent(btmsDecisionXml, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Console.Write(alvsDecisionXml);
        Console.Write(btmsDecisionXml);
        var message = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Update",
            Resource = new CustomsDeclaration()
            {
                Finalisation = new Finalisation()
                {
                    FinalState = "3",
                    ExternalVersion = 1,
                    IsManualRelease = false,
                },
            },
        };

        await SendMessage(JsonSerializer.Serialize(message));
        await WaitOnMessagesBeingProcessed();
    }
}
