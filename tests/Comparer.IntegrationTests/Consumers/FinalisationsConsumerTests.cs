using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDecisionComparer.Comparer.Entities;
using FluentAssertions;
using Xunit.Abstractions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Consumers;

public class FinalisationsConsumerTests(ITestOutputHelper output) : SqsTestBase(output)
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private const string sampleDecision =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">\r\n    <soap:Header>\r\n       <oas:Security soap:role=\"system\" soap:mustUnderstand=\"true\" xmlns:oas=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">\r\n            <oas:UsernameToken>\r\n                <oas:Username>ibmtest</oas:Username>\r\n                <oas:Password>password</oas:Password>\r\n            </oas:UsernameToken>\r\n        </oas:Security>\r\n    </soap:Header>\r\n    <soap:Body>\r\n        <DecisionNotification xmlns=\"http://uk.gov.hmrc.ITSW2.ws\">\r\n            <DecisionNotification xmlns=\"http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification\">\r\n                <ServiceHeader>\r\n                    <SourceSystem>ALVS</SourceSystem>\r\n                    <DestinationSystem>CDS</DestinationSystem>\r\n                    <CorrelationId>000</CorrelationId>\r\n                    <ServiceCallTimestamp>2023-06-30T07:34:14.405827</ServiceCallTimestamp>\r\n                </ServiceHeader>\r\n                <Header>\r\n                    <EntryReference>23GB1234567890ABC8</EntryReference>\r\n                    <EntryVersionNumber>1</EntryVersionNumber>\r\n                    <DecisionNumber>1</DecisionNumber>\r\n                </Header>\r\n                <Item>\r\n                    <ItemNumber>1</ItemNumber>\r\n                    <Check>\r\n                        <CheckCode>H218</CheckCode>\r\n                        <DecisionCode>C02</DecisionCode>\r\n                        <DecisionValidUntil>202307042359</DecisionValidUntil>\r\n                    </Check>                   \r\n                </Item>               \r\n            </DecisionNotification>\r\n        </DecisionNotification>\r\n    </soap:Body>\r\n</soap:Envelope>";

    [Fact]
    public async Task WhenFinalised_AndNoDecisions()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");
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
        await DrainAllMessages();
        await SendMessage(JsonSerializer.Serialize(message));

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var response = await client.GetAsync($"/comparisons/{mrn}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return false;

                var entity =
                    JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
                    ?? throw new Exception("Failed to deserialize");

                return entity.Latest is { AlvsXml: null, BtmsXml: null };
            })
        );
    }

    [Fact]
    public async Task WhenFinalised_AndExistingDecisions()
    {
        var client = CreateClient();
        var mrn = Guid.NewGuid().ToString("N");

        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent(sampleDecision, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent(sampleDecision, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

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
        await DrainAllMessages();
        await SendMessage(JsonSerializer.Serialize(message));

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                response = await client.GetAsync($"/comparisons/{mrn}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return false;

                var entity =
                    JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
                    ?? throw new Exception("Failed to deserialize");

                return entity.Latest is { AlvsXml: sampleDecision, BtmsXml: sampleDecision };
            })
        );

        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent(sampleDecision, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        message = new ResourceEvent<CustomsDeclaration>
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
        await DrainAllMessages();
        await SendMessage(JsonSerializer.Serialize(message));

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                response = await client.GetAsync($"/comparisons/{mrn}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return false;

                var entity =
                    JsonSerializer.Deserialize<ComparisonEntity>(content, s_options)
                    ?? throw new Exception("Failed to deserialize");

                return entity
                    is {
                        History: [{ AlvsXml: sampleDecision, BtmsXml: sampleDecision }],
                        Latest: { AlvsXml: sampleDecision, BtmsXml: sampleDecision }
                    };
            })
        );
    }
}
