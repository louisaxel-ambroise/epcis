using FasTnT.IntegrationTests.v1_2.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace FasTnT.IntegrationTests.v1_2;

[TestClass]
public class QueryEndpointsTests
{
    internal static WebApplicationFactory<Program> TestHost { get; set; }
    internal static HttpClient Client { get; set; }

    [ClassInitialize]
    public static void AssemblyInit(TestContext _)
    {
        TestHost = new FasTnTApplicationFactory(nameof(QueryEndpointsTests));
        Client = TestHost.CreateDefaultClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
    }

    [TestMethod]
    public void GetQueryNamesShouldReturnAGetQueryNamesResult()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1""><soapenv:Body><urn:GetQueryNames /></soapenv:Body></soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsNotNull(XmlResponseExtensions.ParseSoap<GetQueryNamesResult>(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void GetStandardVersionShouldReturnAGetStandardVersionResult()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1""><soapenv:Body><urn:GetStandardVersion /></soapenv:Body></soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsNotNull(XmlResponseExtensions.ParseSoap<GetStandardVersionResult>(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void GetVendorVersionShouldReturnAGetVendorVersionResult()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1""><soapenv:Body><urn:GetVendorVersion /></soapenv:Body></soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsNotNull(XmlResponseExtensions.ParseSoap<GetVendorVersionResult>(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void PollQuerySimpleEventQueryShouldReturnAPollResult()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1"">
  <soapenv:Body>
    <urn:Poll>
    	<queryName>SimpleEventQuery</queryName>
    	<params>
    	</params>
	</urn:Poll>
  </soapenv:Body>
</soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsNotNull(XmlResponseExtensions.ParseSoap<QueryResults>(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void PollQuerySimpleMasterdataQueryShouldReturnAPollResult()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1"">
  <soapenv:Body>
    <urn:Poll>
    	<queryName>SimpleMasterdataQuery</queryName>
    	<params>
    	</params>
	</urn:Poll>
  </soapenv:Body>
</soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsNotNull(XmlResponseExtensions.ParseSoap<QueryResults>(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void PollQueryWithAnUnknownQueryNameShouldReturnAFaultResult()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1"">
  <soapenv:Body>
    <urn:Poll>
    	<queryName>UnknownQuery</queryName>
    	<params>
    	</params>
	</urn:Poll>
  </soapenv:Body>
</soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsNotNull(XmlResponseExtensions.ParseSoap<FaultResult>(response.Content.ReadAsStringAsync().Result));
    }

    [TestMethod]
    public void RetrievingTheListOfSubscriptionIDsShouldReturnAGetSubscriptionIDsResult()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1"">
  <soapenv:Body>
  	<urn:GetSubscriptionIDs>
    	<queryName>SimpleEventQuery</queryName>
    </urn:GetSubscriptionIDs>
  </soapenv:Body>
</soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsNotNull(XmlResponseExtensions.ParseSoap<GetSubscriptionIDsResult>(response.Content.ReadAsStringAsync().Result));
    }
}
