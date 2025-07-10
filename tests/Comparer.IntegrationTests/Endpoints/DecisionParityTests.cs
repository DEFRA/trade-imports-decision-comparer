using System.Net;
using System.Text;
using System.Text.Json;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDecisionComparer.Comparer.Projections;
using FluentAssertions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Endpoints;

public class DecisionParityTests(ITestOutputHelper output) : SqsTestBase(output)
{
    private static readonly JsonSerializerOptions s_options = new() { PropertyNameCaseInsensitive = true };

    private const string Decision1 =
        @"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"">
                <soap:Header>
                    <oas:Security soap:role=""system"" soap:mustUnderstand=""true"" xmlns:oas=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                        <oas:UsernameToken>
                            <oas:Username>ibmtest</oas:Username>
                            <oas:Password>password</oas:Password>
                        </oas:UsernameToken>
                    </oas:Security>
                </soap:Header>
                <soap:Body>
                    <DecisionNotification xmlns=""http://uk.gov.hmrc.ITSW2.ws"">
                    <DecisionNotification xmlns=""http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification"">
                        <ServiceHeader>
                            <SourceSystem>ALVS</SourceSystem>
                            <DestinationSystem>CDS</DestinationSystem>
                            <CorrelationId>000</CorrelationId>
                            <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>
                        </ServiceHeader>
                        <Header>
                            <EntryReference>23GB1234567890ABC8</EntryReference>
                            <EntryVersionNumber>1</EntryVersionNumber>
                            <DecisionNumber>1</DecisionNumber>
                        </Header>
                        <Item>
                            <ItemNumber>1</ItemNumber>
                            <Check>
                                <CheckCode>H218</CheckCode>
                                <DecisionCode>X00</DecisionCode>
                                <DecisionValidUntil>202307042359</DecisionValidUntil>
                            </Check>
                        </Item>
                    </DecisionNotification>
                    </DecisionNotification>
                </soap:Body>
        </soap:Envelope>";

    private const string Decision2 =
        @"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"">
                <soap:Header>
                    <oas:Security soap:role=""system"" soap:mustUnderstand=""true"" xmlns:oas=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                        <oas:UsernameToken>
                            <oas:Username>ibmtest</oas:Username>
                            <oas:Password>password</oas:Password>
                        </oas:UsernameToken>
                    </oas:Security>
                </soap:Header>
                <soap:Body>
                    <DecisionNotification xmlns=""http://uk.gov.hmrc.ITSW2.ws"">
                    <DecisionNotification xmlns=""http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification"">
                        <ServiceHeader>
                            <SourceSystem>ALVS</SourceSystem>
                            <DestinationSystem>CDS</DestinationSystem>
                            <CorrelationId>000</CorrelationId>
                            <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>
                        </ServiceHeader>
                        <Header>
                            <EntryReference>23GB1234567890ABC8</EntryReference>
                            <EntryVersionNumber>1</EntryVersionNumber>
                            <DecisionNumber>1</DecisionNumber>
                        </Header>
                        <Item>
                            <ItemNumber>1</ItemNumber>
                            <Check>
                                <CheckCode>H218</CheckCode>
                                <DecisionCode>C02</DecisionCode>
                                <DecisionValidUntil>202307042359</DecisionValidUntil>
                            </Check>
                        </Item>
                    </DecisionNotification>
                    </DecisionNotification>
                </soap:Body>
        </soap:Envelope>";

    private const string Decision3 =
        @"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"">
                <soap:Header>
                    <oas:Security soap:role=""system"" soap:mustUnderstand=""true"" xmlns:oas=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                        <oas:UsernameToken>
                            <oas:Username>ibmtest</oas:Username>
                            <oas:Password>password</oas:Password>
                        </oas:UsernameToken>
                    </oas:Security>
                </soap:Header>
                <soap:Body>
                    <DecisionNotification xmlns=""http://uk.gov.hmrc.ITSW2.ws"">
                    <DecisionNotification xmlns=""http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification"">
                        <ServiceHeader>
                            <SourceSystem>ALVS</SourceSystem>
                            <DestinationSystem>CDS</DestinationSystem>
                            <CorrelationId>000</CorrelationId>
                            <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>
                        </ServiceHeader>
                        <Header>
                            <EntryReference>23GB1234567890ABC8</EntryReference>
                            <EntryVersionNumber>1</EntryVersionNumber>
                            <DecisionNumber>1</DecisionNumber>
                        </Header>
                        <Item>
                            <ItemNumber>1</ItemNumber>
                            <Check>
                                <CheckCode>H218</CheckCode>
                                <DecisionCode>C03</DecisionCode>
                                <DecisionValidUntil>202307042359</DecisionValidUntil>
                            </Check>
                        </Item>
                    </DecisionNotification>
                    </DecisionNotification>
                </soap:Body>
        </soap:Envelope>";

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
        result.ParityStats.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task WhenParityExists_ShouldReturnResults()
    {
        await DrainAllMessages();
        var client = CreateClient();
        var start = DateTime.UtcNow;

        var mismatchMrn = Guid.NewGuid().ToString("N");

        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision1, Decision1);
        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision2, Decision3);
        await InsertDecisionsForMrn(mismatchMrn, Decision2, Decision1);

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(start, null));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<ParityProjection>(content, s_options)
            ?? throw new Exception("Failed to deserialize");
        result.MisMatchMrns.Count.Should().Be(1);
        result.MisMatchMrns[0].Should().Be(mismatchMrn);
        result.ParityStats.Count.Should().Be(3);
        result.ParityStats["ExactMatch"].Should().Be(1);
        result.ParityStats["Mismatch"].Should().Be(1);
        result.ParityStats["GroupMatch"].Should().Be(1);
    }

    [Fact]
    public async Task WhenParityExists_AndIsFinalisationIsFalse_ItOnlyCalculatesParityForNonFinalisedRecords()
    {
        await DrainAllMessages();
        var client = CreateClient();
        var start = DateTime.UtcNow;

        var mismatchMrn = Guid.NewGuid().ToString("N");

        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision1, Decision1, false);
        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision2, Decision3, false);
        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision1, Decision1, false);
        await InsertDecisionsForMrn(mismatchMrn, Decision2, Decision1, false);
        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision2, Decision1);

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(start, null, false));
        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<ParityProjection>(content, s_options)
            ?? throw new Exception("Failed to deserialize");

        result.MisMatchMrns.Count.Should().Be(1);
        result.MisMatchMrns[0].Should().Be(mismatchMrn);
        result.ParityStats.Count.Should().Be(3);
        result.ParityStats["ExactMatch"].Should().Be(2);
        result.ParityStats["Mismatch"].Should().Be(1);
        result.ParityStats["GroupMatch"].Should().Be(1);
    }

    [Fact]
    public async Task WhenParityExists_AndWhenDecisionNumberParityExists_ItIsIncludedInTheResults()
    {
        await DrainAllMessages();
        var client = CreateClient();
        var start = DateTime.UtcNow;

        var nonMatchingDecision1 = Decision1.Replace(
            "<DecisionNumber>1</DecisionNumber>",
            "<DecisionNumber>2</DecisionNumber>"
        );

        var mrn1 = Guid.NewGuid().ToString("N");

        await InsertDecisionsForMrn(mrn1, Decision1, nonMatchingDecision1, false);
        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision1, Decision1, false);

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(start, null, false));
        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<ParityProjection>(content, s_options)
            ?? throw new Exception("Failed to deserialize");

        result.DecisionNumberStats["Mismatch"].Should().Be(1);
        result.DecisionNumberStats["ExactMatch"].Should().Be(1);
        result.MisMatchDecisionNumberMrns[0].Should().Be(mrn1);
    }

    [Fact]
    public async Task WhenParityExists_AndWhenDecisionNumberParityExists_TheReceivedOrderIsIncluded()
    {
        await DrainAllMessages();
        var client = CreateClient();
        var start = DateTime.UtcNow;

        var decision1Earlier = Decision1.Replace(
            "<ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>",
            "<ServiceCallTimestamp>2023-06-29T07:34:14.405827</ServiceCallTimestamp>"
        );

        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision1, decision1Earlier, false);
        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), Decision1, decision1Earlier, false);
        await InsertDecisionsForMrn(Guid.NewGuid().ToString("N"), decision1Earlier, Decision1, false);

        var response = await client.GetAsync(Testing.Endpoints.Decisions.Parity(start, null, false));
        var content = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<ParityProjection>(content, s_options)
            ?? throw new Exception("Failed to deserialize");

        result.DecisionNumberStats["countWhereBtmsRespondedFirst"].Should().Be(2);
        result.DecisionNumberStats["countWhereAlvsRespondedFirst"].Should().Be(1);
    }

    private async Task InsertDecisionsForMrn(
        string mrn,
        string alvsDecisionXml,
        string btmsDecisionXml,
        bool isFinalisation = true
    )
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
            Resource = new CustomsDeclaration
            {
                Finalisation = isFinalisation
                    ? new Finalisation
                    {
                        FinalState = "3",
                        ExternalVersion = 1,
                        IsManualRelease = false,
                    }
                    : null,
            },
        };

        await SendMessage(JsonSerializer.Serialize(message));
        await WaitOnMessagesBeingProcessed();
    }
}
