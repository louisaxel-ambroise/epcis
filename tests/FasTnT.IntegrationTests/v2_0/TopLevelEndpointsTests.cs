using FasTnT.Application.Database;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/epcs")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionEventTypesShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/eventtypes")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionDispositionsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/dispositions")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionReadPointsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/readPoints")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionBizStepsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/bizSteps")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionBizLocationsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/bizLocations")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionCaptureShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/capture")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionEventsShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/events")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionEventTypeShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/events/ObjectEvent")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void OptionQueriesShouldReturnASuccessfulStatusCode()
    {
        var response = Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/v2_0/queries")).Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }
}
