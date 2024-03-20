using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace FasTnT.IntegrationTests.v2_0;

[TestClass]
public class UnauthorizedEndpointsTests
{
    internal static WebApplicationFactory<Program> TestHost { get; set; }
    internal static HttpClient Client { get; set; }

    [ClassInitialize]
    public static void AssemblyInit(TestContext _)
    {
        TestHost = new FasTnTApplicationFactory($"{nameof(UnauthorizedEndpointsTests)}2");
        Client = TestHost.CreateDefaultClient();
    }

    [TestMethod]
    public void CallAnEndpointWithoutAuthorizationShouldReturnA401StatusCode()
    {
        var response = Client.GetAsync("/epcs").Result;

        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
