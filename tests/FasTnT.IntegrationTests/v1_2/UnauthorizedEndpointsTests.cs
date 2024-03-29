using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;

namespace FasTnT.IntegrationTests.v1_2;

[TestClass]
public class UnauthorizedEndpointsTests
{
    internal static WebApplicationFactory<Program> TestHost { get; set; }
    internal static HttpClient Client { get; set; }

    [ClassInitialize]
    public static void AssemblyInit(TestContext _)
    {
        TestHost = new FasTnTApplicationFactory(nameof(UnauthorizedEndpointsTests));
        Client = TestHost.CreateDefaultClient();
    }

    [TestMethod]
    public void CallAnEndpointWithoutAuthorizationShouldReturnA401StatusCode()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1""><soapenv:Body><urn:GetQueryNames /></soapenv:Body></soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public void CallAnEndpointWithInvalidAuthorizationShouldReturnA401StatusCode()
    {
        var request = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:epcglobal:epcis-query:xsd:1""><soapenv:Body><urn:GetQueryNames /></soapenv:Body></soapenv:Envelope>";
        var httpContent = new StringContent(request, Encoding.UTF8, "application/xml");
        httpContent.Headers.TryAddWithoutValidation("Authorization", "Basic invalid");

        var response = Client.PostAsync("/Query.svc", httpContent).Result;

        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
