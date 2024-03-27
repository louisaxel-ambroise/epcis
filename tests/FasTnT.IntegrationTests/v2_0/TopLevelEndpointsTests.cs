using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;

namespace FasTnT.IntegrationTests.v2_0;

[TestClass]
public class TopLevelEndpointsTests
{
    internal static WebApplicationFactory<Program> TestHost { get; set; }
    internal static HttpClient Client { get; set; }

    [ClassInitialize]
    public static void AssemblyInit(TestContext _)
    {
        TestHost = new FasTnTApplicationFactory(nameof(TopLevelEndpointsTests));
        Client = TestHost.CreateDefaultClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
    }

    [TestMethod]
    public void OptionEpcListShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/epcs")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionEventTypesShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/eventtypes")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionDispositionsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/dispositions")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionReadPointsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/readPoints")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionBizStepsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/bizSteps")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionBizLocationsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/bizLocations")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionCaptureShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/capture")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionEventsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/events")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionEventTypeShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/events/ObjectEvent")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionQueriesShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/queries")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }
}
