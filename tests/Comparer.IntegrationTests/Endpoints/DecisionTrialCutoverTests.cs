using System.Net;
using System.Text;
using Defra.TradeImportsDecisionComparer.Comparer.Configuration;
using FluentAssertions;

namespace Defra.TradeImportsDecisionComparer.Comparer.IntegrationTests.Endpoints;

public class DecisionTrialCutoverTests : IntegrationTestBase
{
    private const string Decision = """
        <?xml version="1.0" encoding="UTF-8"?><NS1:Envelope xmlns:NS1="http://www.w3.org/2003/05/soap-envelope">  <NS1:Header>    <NS2:Security xmlns:NS2="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" NS1:role="system">      <NS2:UsernameToken>        <NS2:Username>ibmtest</NS2:Username>        <NS2:Password>password</NS2:Password>      </NS2:UsernameToken>    </NS2:Security>  </NS1:Header>  <NS1:Body>    <NS3:DecisionNotification xmlns:NS3="http://uk.gov.hmrc.ITSW2.ws">&lt;NS2:DecisionNotification xmlns:NS2=&quot;http://www.hmrc.gov.uk/webservices/itsw/ws/decisionnotification&quot;&gt;&lt;NS2:ServiceHeader&gt;&lt;NS2:SourceSystem&gt;ALVS&lt;/NS2:SourceSystem&gt;&lt;NS2:DestinationSystem&gt;CDS&lt;/NS2:DestinationSystem&gt;&lt;NS2:CorrelationId&gt;17522391082192495792&lt;/NS2:CorrelationId&gt;&lt;NS2:ServiceCallTimestamp&gt;2025-07-11T13:05:08.219&lt;/NS2:ServiceCallTimestamp&gt;&lt;/NS2:ServiceHeader&gt;&lt;NS2:Header&gt;&lt;NS2:EntryReference&gt;mrn&lt;/NS2:EntryReference&gt;&lt;NS2:EntryVersionNumber&gt;1&lt;/NS2:EntryVersionNumber&gt;&lt;NS2:DecisionNumber&gt;1&lt;/NS2:DecisionNumber&gt;&lt;/NS2:Header&gt;&lt;NS2:Item&gt;&lt;NS2:ItemNumber&gt;1&lt;/NS2:ItemNumber&gt;&lt;NS2:Check&gt;&lt;NS2:CheckCode&gt;H221&lt;/NS2:CheckCode&gt;&lt;NS2:DecisionCode&gt;X00&lt;/NS2:DecisionCode&gt;&lt;NS2:DecisionReason&gt;A Customs Declaration has been submitted however no matching CVEDA(s) have been submitted to Port Health (for CVEDA number(s) GBCHD2025.1053651). Please correct the CVEDA number(s) entered on your customs declaration.&lt;/NS2:DecisionReason&gt;&lt;/NS2:Check&gt;&lt;/NS2:Item&gt;&lt;/NS2:DecisionNotification&gt;</NS3:DecisionNotification>  </NS1:Body></NS1:Envelope>
        """;

    [Fact]
    public async Task WhenBtmsDecision_AndThenAlvs_BtmsIsChosen()
    {
        var client = CreateClient(OperatingMode.TrialCutover);
        var mrn = Guid.NewGuid().ToString("N");

        var btmsXml = Decision
            .Replace(
                "&lt;NS2:EntryReference&gt;mrn&lt;/NS2:EntryReference&gt;",
                $"&lt;NS2:EntryReference&gt;{mrn}&lt;/NS2:EntryReference&gt;"
            )
            .Replace(
                "&lt;NS2:SourceSystem&gt;ALVS&lt;/NS2:SourceSystem&gt;",
                "&lt;NS2:SourceSystem&gt;BTMS&lt;/NS2:SourceSystem&gt;"
            );
        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent(btmsXml, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var alvsXml = Decision.Replace(
            "&lt;NS2:EntryReference&gt;mrn&lt;/NS2:EntryReference&gt;",
            $"&lt;NS2:EntryReference&gt;{mrn}&lt;/NS2:EntryReference&gt;"
        );
        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent(alvsXml, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("BTMS");
    }

    [Fact]
    public async Task WhenBtmsDecision_AndThenAlvs_ThenAlvsAgain_AlvsIsChosen()
    {
        var client = CreateClient(OperatingMode.TrialCutover);
        var mrn = Guid.NewGuid().ToString("N");

        var btmsXml = Decision
            .Replace(
                "&lt;NS2:EntryReference&gt;mrn&lt;/NS2:EntryReference&gt;",
                $"&lt;NS2:EntryReference&gt;{mrn}&lt;/NS2:EntryReference&gt;"
            )
            .Replace(
                "&lt;NS2:SourceSystem&gt;ALVS&lt;/NS2:SourceSystem&gt;",
                "&lt;NS2:SourceSystem&gt;BTMS&lt;/NS2:SourceSystem&gt;"
            );
        var response = await client.PutAsync(
            Testing.Endpoints.Decisions.Btms.Put(mrn),
            new StringContent(btmsXml, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var alvsXml = Decision.Replace(
            "&lt;NS2:EntryReference&gt;mrn&lt;/NS2:EntryReference&gt;",
            $"&lt;NS2:EntryReference&gt;{mrn}&lt;/NS2:EntryReference&gt;"
        );
        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent(alvsXml, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        alvsXml = alvsXml.Replace(
            "&lt;NS2:DecisionNumber&gt;1&lt;/NS2:DecisionNumber&gt;",
            "&lt;NS2:DecisionNumber&gt;2&lt;/NS2:DecisionNumber&gt;"
        );
        response = await client.PutAsync(
            Testing.Endpoints.Decisions.Alvs.Put(mrn),
            new StringContent(alvsXml, Encoding.UTF8, "application/xml")
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("ALVS");
    }
}
